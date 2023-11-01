using Dacodelaac.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Event = Dacodelaac.Events.Event;

namespace Dacodelaac.UI.Buttons
{
    public class RippleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Ease ease = Ease.OutQuint;
        [SerializeField] float scale = 0.9f;
        [SerializeField] bool hold = false;
        [SerializeField] public bool isInteractable = true;
        [SerializeField] Button.ButtonClickedEvent m_OnClick;
        [SerializeField] Event buttonClickEvent;

        Vector3 originScale = Vector3.one;
        bool clicking = false;

        void OnEnable()
        {
            clicking = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isInteractable) return;
            
            buttonClickEvent.Raise();
            clicking = true;
            DoScale();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!hold && isInteractable)
            {
                m_OnClick.Invoke();
            }
            clicking = false;
            ResetScale();
        }

        void DoScale()
        {
            DOTween.Kill(this);
            transform.DOScale(originScale * scale, 0.15f).SetEase(ease).SetUpdate(true).SetTarget(this);
        }

        void ResetScale()
        {
            DOTween.Kill(this);
            transform.localScale = originScale;
        }

        void Update()
        {
            if (hold && clicking && isInteractable)
            {
                m_OnClick.Invoke();   
            }
        }
    }
}