using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class BakedAnimationData : ScriptableObject
{
    [SerializeField] Mesh mesh;
    [SerializeField] Texture2D texture;
    [SerializeField] float magnitude;
    [SerializeField] List<ClipData> clipDatas = new List<ClipData>();

    public Mesh Mesh => mesh;
    public Texture2D Texture => texture;
    public float Magnitude => magnitude;
    private Dictionary<string, ClipData> dictData = new Dictionary<string, ClipData>();

    public void SetData(Mesh mesh, Texture2D texture, List<ClipData> clipDatas, float magnitude)
    {
        this.mesh = mesh;
        this.texture = texture;
        this.clipDatas = clipDatas;
        this.magnitude = magnitude;
    }

    public void CacheDict()
    {
        if (dictData.Count != 0) return;
        foreach (var item in clipDatas)
        {
            dictData[item.Name] = item;
        }
    }

    public ClipData GetClipData(string name)
    {
        return dictData[name];
    }

    public ClipData GetClipData(int i) => clipDatas[i];

    public int AnimCount => clipDatas.Count;

    public ClipData DefaultClip => clipDatas.FirstOrDefault(c => c.IsDefault);
}

[System.Serializable]
public class ClipData
{
    [SerializeField] string name;
    [SerializeField] float duration;
    [SerializeField] int frameStart;
    [SerializeField] int frameEnd;
    [SerializeField] float speed;
    [SerializeField] bool loop;
    [SerializeField] bool isDefault;

    public string Name => name;
    public int FrameStart => frameStart;
    public int FrameEnd => frameEnd;
    public float Duration => duration;
    public float Speed => speed;
    public bool Loop => loop;
    public bool IsDefault => isDefault;

    public ClipData(string name, float duration, int frameStart, int frameEnd, float speed, bool loop, bool isDefault)
    {
        this.name = name;
        this.duration = duration;
        this.frameStart = frameStart;
        this.frameEnd = frameEnd;
        this.speed = speed;
        this.loop = loop;
        this.isDefault = isDefault;
    }

    public void SetDefault(bool isDefault)
    {
        this.isDefault = isDefault;
    }
}


[System.Serializable]
public class Frame
{
    public Vector3[] vertices;

    public Frame(Vector3[] vertices)
    {
        this.vertices = vertices;
    }
}