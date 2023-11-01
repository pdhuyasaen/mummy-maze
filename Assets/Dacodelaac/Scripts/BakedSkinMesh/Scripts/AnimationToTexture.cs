using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class AnimationToTexture : MonoBehaviour
{
    [SerializeField] MeshFilter mirror;
    [SerializeField] private string appendName;

    [Header("Config")]
    [SerializeField] int framesPerSecond = 60;
    [SerializeField] AnimData[] animDatas;

    string TEXTURE_PATH => $"Assets/Dacodelaac/BakedSkinMesh/Texture/baked_anim_texture{appendName}.asset";
    const string MESH_PATH = "Assets/Dacodelaac/BakedSkinMesh/MeshUV2/baked_anim_mesh_{0}.asset";
    const string DATA_PATH = "Assets/Dacodelaac/BakedSkinMesh/Data/baked_anim_data_{0}.asset";

    [ContextMenu("Create")]
    public void Do()
    {
        StartCoroutine(Sample());
    }

    public IEnumerator Sample()
    {
        var totalData = new SampledMeshData();
        int maxVertices = -1;
        foreach (var anim in animDatas)
        {
            var mesh = anim.target.GetComponentInChildren<SkinnedMeshRenderer>();

            anim.clipDatas = new List<ClipData>();
            anim.data = new SampledMeshData();

            for (int i = 0; i < anim.clips.Length; i++)
            {
                yield return SampleClip(anim.target, anim.clips[i], mesh, anim.data, anim.clipDatas, totalData.frames.Count, anim.posOffset, anim.rotOffset, anim.scale);
            }

            anim.magnitude = anim.data.Normalized();

            totalData.Append(anim.data);

            if (maxVertices < anim.data.frames[0].vertices.Length)
            {
                maxVertices = anim.data.frames[0].vertices.Length;
            }

            yield return null;
        }

        var h = Mathf.NextPowerOfTwo(Mathf.Max(totalData.frames.Count));
        var w = Mathf.NextPowerOfTwo(Mathf.Max(maxVertices));

        var tex = CreateTexture(totalData, w, h);

        foreach (var anim in animDatas)
        {
            var mesh = anim.target.GetComponentInChildren<SkinnedMeshRenderer>();

            var uv2 = CreateUV2(anim.data.frames[0], w);

            var bakedMesh = CreateMesh(mesh, uv2, anim.dataName, anim.meshScale, anim.rotOffset);

            var bakedData = ScriptableObject.CreateInstance<BakedAnimationData>();

            if (anim.clipDatas.Count > 0)
            {
                anim.clipDatas[0].SetDefault(true);
            }

            bakedData.SetData(bakedMesh, tex, anim.clipDatas, anim.magnitude);

#if UNITY_EDITOR
            AssetDatabase.CreateAsset(bakedMesh, string.Format(MESH_PATH, anim.dataName));
            AssetDatabase.CreateAsset(bakedData, string.Format(DATA_PATH, anim.dataName));

            EditorUtility.SetDirty(bakedMesh);
            EditorUtility.SetDirty(bakedData);
#endif
        }
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(tex, TEXTURE_PATH);

        EditorUtility.SetDirty(tex);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.LogFormat("Done");

        EditorApplication.isPlaying = false;
#endif        
    }

    public IEnumerator SampleClip(GameObject target, AnimationClipData clipData, SkinnedMeshRenderer mesh, SampledMeshData data, List<ClipData> clipDatas, int prevOffset, Vector3 posOffset, Vector3 rotOffset, float scale)
    {
        var clip = clipData.clip;
        var duration = clip.length;

        int frames = (int)(framesPerSecond * duration);
        for (int i = 0; i <= frames; i++)
        {
            Debug.LogFormat("Bake clip {0} - frame {1}/{2}", clip.name, i, frames);

            clip.SampleAnimation(target, duration / frames * i);
            var baked = new Mesh();
            mesh.BakeMesh(baked);
            mirror.sharedMesh = baked;
            
            data.AddFrame(baked.vertices.Select(v => Quaternion.Euler(rotOffset) * v * scale + posOffset).ToArray());

            yield return null;
        }

        var prevOffsetMax = clipDatas.Count == 0 ? prevOffset - 1 : clipDatas[clipDatas.Count - 1].FrameEnd;

        clipDatas.Add(new ClipData(clip.name, duration, prevOffsetMax + 1, prevOffsetMax + frames + 1, clipData.speed, clip.isLooping, clipData.isDefault));
    }

    Texture2D CreateTexture(SampledMeshData data, int w, int h)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        tex.filterMode = FilterMode.Point;

        for (int r = 0; r < data.frames.Count; r++)
        {
            var vertices = data.frames[r].vertices;
            for (int c = 0; c < vertices.Length; c++)
            {
                var v = vertices[c];

                //var a = (float)c / texSize;
                tex.SetPixel(c, r, new Color(v.x, v.y, v.z));
            }
        }

        return tex;
    }

    Vector2[] CreateUV2(Frame f, int w)
    {
        var uv = new Vector2[f.vertices.Length];
        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2((float)i / w, 0);
        }
        return uv;
    }

    Mesh CreateMesh(SkinnedMeshRenderer skinnedMeshRenderer, Vector2[] uv2, string dataName, float meshScale, Vector3 rotOffset)
    {
        var mesh = new Mesh();
        mesh.name = string.Format("{0} baked mesh", dataName);

        var sampleMesh = skinnedMeshRenderer.sharedMesh;

        mesh.vertices = sampleMesh.vertices.Select(v => Quaternion.Euler(rotOffset) * v * meshScale).ToArray();
        mesh.uv = sampleMesh.uv;
        mesh.triangles = sampleMesh.triangles;
        mesh.normals = sampleMesh.normals.Select(v => Quaternion.Euler(rotOffset) * v).ToArray();
        mesh.uv2 = uv2;

        return mesh;
    }
}

public class SampledMeshData
{
    public List<Frame> frames = new List<Frame>();

    public int VertexCount => frames[0].vertices.Length;

    public void AddFrame(Vector3[] vertices)
    {
        if (frames.Count > 0 && frames[0].vertices.Length != vertices.Length)
        {
            Debug.LogErrorFormat("Vertices length inconsistent {0} vs {1}", frames[0].vertices.Length, vertices.Length);
            return;
        }

        frames.Add(new Frame(vertices));
    }

    public void Append(SampledMeshData other)
    {
        frames.AddRange(other.frames);
    }

    public float Normalized()
    {
        float min = frames.Min(f => f.vertices.Min(v => Mathf.Min(v.x, v.y, v.z)));
        float max = frames.Max(f => f.vertices.Max(v => Mathf.Max(v.x, v.y, v.z)));

        Debug.LogFormat("Min {0}, Max {1}", min, max);

        if (frames.Any(f => f.vertices.Any(v => v.x < min || v.y < min || v.z < min)))
        {
            Debug.LogError("MINMIN");
        }

        if (frames.Any(f => f.vertices.Any(v => v.x > max || v.y > max || v.z > max)))
        {
            Debug.LogError("Max max");
        }

        max = Mathf.Max(Mathf.Abs(min), Mathf.Abs(max));
        min = -max;

        var magnitude = max - min;

        var normalizedFrames = new List<Frame>();

        for (int i = 0; i < frames.Count; i++)
        {
            Vector3[] fs = frames[i].vertices;
            Vector3[] vs = new Vector3[fs.Length];

            for (int k = 0; k < fs.Length; k++)
            {
                vs[k] = fs[k] / magnitude + 0.5f * Vector3.one;
            }

            normalizedFrames.Add(new Frame(vs));
        }

        if (normalizedFrames.Any(f => f.vertices.Any(v => v.x > 1 || v.y > 1 || v.z > 1 || v.x < 0 || v.y < 0 || v.z < 0)))
        {
            Debug.LogError("Normalize wrong");
        }

        frames = normalizedFrames;

        return magnitude;
    }
}

[System.Serializable]
public class AnimData
{
    public string dataName;
    public GameObject target;
    public float scale = 1;
    public float meshScale = 1;
    public Vector3 posOffset;
    public Vector3 rotOffset;
    public AnimationClipData[] clips;
    public List<ClipData> clipDatas { get; set; }
    public SampledMeshData data { get; set; }
    public float magnitude { get; set; }
}

[System.Serializable]
public class AnimationClipData
{
    public AnimationClip clip;
    public float speed;
    public bool isDefault;
}