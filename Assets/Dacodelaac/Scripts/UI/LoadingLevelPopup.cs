using System.Collections;
using Dacodelaac.UI.Popups;
using DG.Tweening;
using UnityEngine;

namespace Dacodelaac.UI
{
    public class LoadingLevelPopup : BasePopup
    {
        [SerializeField] private Transform logo;
        [SerializeField] private float minLoadTime = 2f;

        private Coroutine _coroutine;
        private bool IsLoading { get; set; }
        private bool IsHide { get; set; }

        public void OnLoad(bool show)
        {
            if (show && !IsLoading)
            {
                IsLoading = true;
                
                gameObject.SetActive(true);
                canvasGroup.alpha = 1;
                _coroutine = StartCoroutine(LoadingRoutine());
            }
            else
            {
                if (!IsLoading) Hide();
                else IsHide = true;
            }
        }

        private IEnumerator LoadingRoutine()
        {
            canvas.gameObject.SetActive(true);
            logo.localScale = Vector3.one;// * 10f;
            
            DOTween.Kill(this);
            DOTween.To(() => 0f, x =>
            {
                logo.localScale = Mathf.Lerp(2.5f, 1f, x) * Vector3.one;
            }, 1f, 1f).SetTarget(this).SetUpdate(true).OnComplete(() =>
            {
                logo.DOScale(1.1f, 0.5f).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo).SetTarget(this)
                    .SetUpdate(true);
            });

            yield return new WaitForSeconds(minLoadTime);
            
            if (IsHide) Hide();
        }

        private void Hide()
        {
            DOTween.To(() => 1f, x => { canvasGroup.alpha = x; }, 0f, 1f).SetTarget(this).SetUpdate(true)
                .OnComplete(() => { canvas.gameObject.SetActive(false); });
            if (_coroutine != null) StopCoroutine(_coroutine);
            
            IsLoading = false;
            IsHide = false;
        }

        public override void DoDisable()
        {
            base.DoDisable();
            DOTween.Kill(this);
            if (_coroutine != null) StopCoroutine(_coroutine);
        }
    }
}