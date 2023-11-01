using System;
using Dacodelaac.Core;
using Dacodelaac.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Dacodelaac.UI.Buttons
{
    public class TwoLayerButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Ease ease = Ease.OutQuint;
        [SerializeField] Image front;
        [SerializeField] Vector2 offset;
        [SerializeField] Type type;
        [SerializeField] bool hold = false;
        [SerializeField] public bool isInteractable = true;
        [SerializeField] Button.ButtonClickedEvent m_OnClick;

        enum Type
        {
            Vertical,
            Horizontal
        }

        RectTransform FrontRect => front ? front.rectTransform : null;

        bool clicking;

        void OnEnable()
        {
            clicking = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isInteractable) return;
         
            // globalSfx.PlayButtonClick();
            clicking = true;
            DoOffset();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!hold && isInteractable)
            {
                m_OnClick.Invoke();
            }

            clicking = false;
            ResetOffset();
        }

        void DoOffset()
        {
            DOTween.Kill(this);
            var targetPos = type == Type.Vertical ? new Vector2(offset.x, 0) : new Vector2(0, offset.y);
            FrontRect.DOAnchorPos(targetPos, 0.15f).SetEase(ease).SetUpdate(true).SetTarget(this);
        }

        void ResetOffset()
        {
            if (!FrontRect) return;

            DOTween.Kill(this);
            FrontRect.anchoredPosition = offset;
        }

        void Update()
        {
            if (hold && clicking && isInteractable)
            {
                m_OnClick.Invoke();
            }
        }


#if UNITY_EDITOR
        void OnValidate()
        {
            ResetOffset();
        }
#endif
    }
}