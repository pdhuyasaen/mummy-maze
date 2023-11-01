using System.Collections;
using System.Collections.Generic;
using Dacodelaac.Core;
using UnityEngine;
using Random = UnityEngine.Random;

public class BakedAnimator : MonoBehaviour
{
    [SerializeField] BakedAnimationData bakedData;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Material[] materials;
    [SerializeField] Material[] materialsAngry;

    ClipData prevClipData;
    float prevFrame;

    ClipData currentClipData;
    float currentFrame;

    float transitionTime;
    float remainTransitionTime;

    MaterialPropertyBlock mpb;

    public float speed = 1;
    Coroutine sequenceRoutine;

    public event System.Action<string> OnAnimationStartEvent;
    public event System.Action<string> OnAnimationEndEvent;

    Dictionary<string, string> transitionDict = new Dictionary<string, string>();

    Coroutine animRoutine;
    bool isToxic;
    bool isPowerUp;

    public bool IsInitialized { get; set; }
    public bool IsCleaned { get; set; }

    public void Initialize()
    {
        if (IsInitialized) return;
        IsInitialized = true;
        
        isToxic = false;
        isPowerUp = false;
        bakedData.CacheDict();
        CreateMesh();
        UpdateMaterial();
        Play(bakedData.DefaultClip.Name, 0);
        currentFrame = Random.Range(currentClipData.FrameStart, currentClipData.FrameEnd);
        animRoutine = StartCoroutine(Animate());
    }

    public void CleanUp()
    {
        if (IsCleaned) return;
        IsCleaned = true;
        
        if (animRoutine != null)
        {
            StopCoroutine(animRoutine);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            ShowFirstFrame();
        }
    }
#endif

    [ContextMenu("Show First Frame")]
    public void ShowFirstFrame()
    {
        CreateMesh();
        UpdateMaterial();
        currentClipData = bakedData.DefaultClip;
        currentFrame = bakedData.DefaultClip.FrameStart;
        UpdateAnimation();
    }

    void CreateMesh()
    {
        meshFilter.sharedMesh = bakedData.Mesh;
    }

    void UpdateMaterial()
    {
        foreach (var item in materials)
        {
            item.SetTexture("_AnimTex", bakedData.Texture);
            item.SetFloat("_Magnitude", bakedData.Magnitude);
        }

        if (materialsAngry != null)
        {
            foreach (var item in materialsAngry)
            {
                item.SetTexture("_AnimTex", bakedData.Texture);
                item.SetFloat("_Magnitude", bakedData.Magnitude);
            }
        }
    }

    public void SetTransition(Dictionary<string, string> transitions)
    {
        transitionDict = transitions;
    }

    IEnumerator Animate()
    {
        while (true)
        {
            UpdateAnimation();

            yield return null;

            if (remainTransitionTime > 0)
            {
                remainTransitionTime -= Time.deltaTime;
            }
            
            if (prevClipData != null)
            {
                if (prevFrame < prevClipData.FrameEnd)
                {
                    prevFrame += speed * prevClipData.Speed * Time.deltaTime * 60f;
                }
            }

            if (currentFrame < currentClipData.FrameEnd)
            {
                currentFrame += speed * currentClipData.Speed * Time.deltaTime * 60f;
                if (currentFrame >= currentClipData.FrameEnd)
                {
                    UpdateAnimation();
                    yield return null;

                    OnAnimationEnded();
                }
            }
        }
    }

    void UpdateAnimation()
    {
        if (!meshRenderer.isVisible) return;

        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(mpb);
        }

        var offsetY1 = Mathf.Clamp(currentFrame, currentClipData.FrameStart, currentClipData.FrameEnd) /
                       bakedData.Texture.height;
        
        var offsetY2 = prevClipData == null
            ? offsetY1
            : Mathf.Clamp(prevFrame, prevClipData.FrameStart, prevClipData.FrameEnd) / bakedData.Texture.height;
        
        var offsetYLerp = transitionTime > 0 ? Mathf.Clamp01(remainTransitionTime / transitionTime) : 0;
        
        mpb.SetFloat("_OffsetY1", offsetY1);
        mpb.SetFloat("_OffsetY2", offsetY2);
        mpb.SetFloat("_OffsetYLerp", offsetYLerp);
        mpb.SetColor("_Color", isPowerUp ? Color.yellow : isToxic ? Color.green : Color.white);
        mpb.SetFloat("_ColorStrength", isPowerUp || isToxic ? 0.8f : 0f);
        meshRenderer.SetPropertyBlock(mpb);
    }
    
    public void SetToxic(bool toxic)
    {
        isToxic = toxic;
    }

    public void SetPowerUp(bool powerUp)
    {
        isPowerUp = powerUp;
    }

    public void SetAngry(bool angry)
    {
        if (materialsAngry != null && materialsAngry.Length > 0)
        {
            meshRenderer.sharedMaterials = angry ? materialsAngry : materials;
        }
    }

    void OnAnimationStarted()
    {
        OnAnimationStartEvent?.Invoke(currentClipData.Name);
    }

    void OnAnimationEnded()
    {
        OnAnimationEndEvent?.Invoke(currentClipData.Name);
        if (currentClipData.Loop)
        {
            currentFrame = currentClipData.FrameStart;
            OnAnimationStarted();
        }
        else if (transitionDict.TryGetValue(currentClipData.Name, out var nextAnimationName))
        {
            Play(nextAnimationName, 0.1f);
        }
    }

    public void Play(string clipName, float normalizedTransitionTime)
    {
        prevClipData = currentClipData;
        prevFrame = currentFrame;

        currentClipData = bakedData.GetClipData(clipName);
        currentFrame = currentClipData.FrameStart;
        
        remainTransitionTime = transitionTime = currentClipData.Duration * normalizedTransitionTime;
        
        OnAnimationStarted();
    }
}