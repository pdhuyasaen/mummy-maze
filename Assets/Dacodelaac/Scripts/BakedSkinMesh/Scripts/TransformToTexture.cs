using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class TransformToTexture : MonoBehaviour
{
    [SerializeField] MeshFilter mirror;

    [Header("Config")]
    [SerializeField] string dataName;
    [SerializeField] GameObject target;
    [SerializeField] GameObject targetTo;
    [SerializeField] AnimationClip clipFrom;
    [SerializeField] AnimationClip clipTo;
    [SerializeField] int framesPerSecond = 60;
    [SerializeField] float duration;
    [SerializeField] GameObject from;
    [SerializeField] GameObject to;

    //const string TEXTURE_PATH = "Assets/BakedSkinMesh/Texture/baked_anim_texture_{0}.asset";
    //const string MESH_PATH = "Assets/BakedSkinMesh/MeshUV2/baked_anim_mesh_{0}.asset";
    //const string DATA_PATH = "Assets/BakedSkinMesh/Data/baked_anim_data_{0}.asset";

    [ContextMenu("Pose")]
    public void Pose()
    {
        var mesh = target.GetComponentInChildren<SkinnedMeshRenderer>();
        target.transform.rotation = Quaternion.Inverse(mesh.transform.rotation);
        target.transform.position -= mesh.transform.position;

        from.transform.rotation = target.transform.rotation;
        from.transform.position = target.transform.position;

        to.transform.rotation = target.transform.rotation;
        to.transform.position = target.transform.position;

        targetTo.transform.rotation = target.transform.rotation;
        targetTo.transform.position = target.transform.position;

        clipFrom.SampleAnimation(target, clipFrom.length);
        clipFrom.SampleAnimation(from, clipFrom.length);
        clipFrom.SampleAnimation(to, clipFrom.length);
        clipTo.SampleAnimation(targetTo, 0);
    }       

    public IEnumerator Sample(SampledMeshData data, List<ClipData> clipDatas, int prevOffset)
    {        
        var mesh = target.GetComponentInChildren<SkinnedMeshRenderer>();        

        yield return SampleTransform(mesh, data, clipDatas, prevOffset);
    }

    //public IEnumerator Sample()
    //{
    //    var data = new SampledMeshData();
    //    var mesh = target.GetComponentInChildren<SkinnedMeshRenderer>();

    //    var clipDatas = new List<ClipData>();

    //    yield return SampleTransform(mesh, data, clipDatas);

    //    float magnitude = data.Normalized();

    //    var texSize = Mathf.NextPowerOfTwo(Mathf.Max(data.frames.Count, data.frames[0].vertices.Length));

    //    var tex = CreateTexture(data, texSize);

    //    var uv2 = CreateUV2(data, texSize);

    //    var bakedMesh = CreateMesh(mesh.sharedMesh, uv2);

    //    var bakedData = ScriptableObject.CreateInstance<BakedAnimationData>();
    //    bakedData.SetData(bakedMesh, tex, clipDatas, magnitude);

    //    AssetDatabase.CreateAsset(tex, string.Format(TEXTURE_PATH, dataName));
    //    AssetDatabase.CreateAsset(bakedMesh, string.Format(MESH_PATH, dataName));
    //    AssetDatabase.CreateAsset(bakedData, string.Format(DATA_PATH, dataName));

    //    EditorUtility.SetDirty(tex);
    //    EditorUtility.SetDirty(bakedMesh);
    //    EditorUtility.SetDirty(bakedData);

    //    AssetDatabase.SaveAssets();
    //    AssetDatabase.Refresh();

    //    Debug.LogFormat("Done");

    //    EditorApplication.isPlaying = false;
    //}

    public IEnumerator SampleTransform(SkinnedMeshRenderer mesh, SampledMeshData data, List<ClipData> clipDatas, int prevOffset)
    {
        int frames = (int)(framesPerSecond * duration);        
        for (int i = 0; i <= frames; i++)
        {
            Debug.LogFormat("Bake frame {0}/{1}", i, frames);

            target.transform.rotation = Quaternion.Lerp(from.transform.rotation, to.transform.rotation, i * 1.0f / frames);
            
            var baked = new Mesh();
            mesh.BakeMesh(baked);
            mirror.sharedMesh = baked;

            var vertices = baked.vertices;
            for(int j = 0; j < vertices.Length; j++)
            {
                vertices[j] = mesh.transform.TransformPoint(vertices[j]);
            }

            data.AddFrame(vertices);

            yield return null;
        }

        var prevOffsetMax = clipDatas.Count == 0 ? prevOffset - 1 : clipDatas[clipDatas.Count - 1].FrameEnd;

        clipDatas.Add(new ClipData(dataName, duration, prevOffsetMax + 1, prevOffsetMax + frames + 1, 1, false, false));
    }

    //Texture2D CreateTexture(SampledMeshData data, int texSize)
    //{
    //    var tex = new Texture2D(texSize, texSize, TextureFormat.RGBA32, false);
    //    tex.filterMode = FilterMode.Point;

    //    for (int r = 0; r < data.frames.Count; r++)
    //    {
    //        var vertices = data.frames[r].vertices;
    //        for (int c = 0; c < vertices.Length; c++)
    //        {
    //            var v = vertices[c];

    //            var a = (float)c / texSize;
    //            tex.SetPixel(c, r, new Color(v.x, v.y, v.z, a));
    //        }
    //    }

    //    return tex;
    //}

    //Vector2[] CreateUV2(SampledMeshData data, int texSize)
    //{
    //    var uv = new Vector2[data.frames[0].vertices.Length];
    //    for (int i = 0; i < uv.Length; i++)
    //    {
    //        uv[i] = new Vector2((float)i / texSize, 0);
    //    }
    //    return uv;
    //}

    //Mesh CreateMesh(Mesh sampleMesh, Vector2[] uv2)
    //{
    //    var mesh = new Mesh();
    //    mesh.name = string.Format("{0} baked mesh", dataName);

    //    mesh.vertices = sampleMesh.vertices;
    //    mesh.uv = sampleMesh.uv;
    //    mesh.triangles = sampleMesh.triangles;
    //    mesh.normals = sampleMesh.normals;
    //    mesh.uv2 = uv2;

    //    return mesh;
    //}
}