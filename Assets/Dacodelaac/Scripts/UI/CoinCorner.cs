using System.Collections;
using Dacodelaac.DataType;
using Dacodelaac.Events;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dacodelaac.UI
{
    public class CoinCorner : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        [SerializeField] RectTransform rect;
        [SerializeField] GameObject flyCoinPrefab;

        void OnEnable()
        {
            var pos = rect.anchoredPosition;
            pos.x = 400;
            rect.anchoredPosition = pos;
        }

        public void OnChanged(ShortDouble value)
        {
            text.text = value.ToString("0.#");
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        public void Hide()
        {
            DOTween.Kill(rect);
            rect.DOAnchorPosX(400, 0.3f).SetTarget(rect);
        }

        public void Show()
        {
            DOTween.Kill(rect);
            rect.DOAnchorPosX(0, 0.3f).SetTarget(rect);
        }

        public void FlyCoin(FlyCoinData data)
        {
            StartCoroutine(IEFlyCoin(data));
        }
        
        IEnumerator IEFlyCoin(FlyCoinData data)
        {
            yield return new WaitForEndOfFrame();
            
            var container = transform.parent.parent;

            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            var coinCornerPosition =
                container.InverseTransformPoint((corners[0] + corners[1]) / 2);
            coinCornerPosition.z = 0;
            
            var position = container.InverseTransformPoint(data.Position);
            position.z = 0;
            
            var count = Random.Range(5, 10);
            var completedCount = 0;
            for (var i = 0; i < count; i++)
            {
                var coin = Instantiate(flyCoinPrefab, container);
                var rect = coin.GetComponent<RectTransform>();
                rect.anchoredPosition3D = position;

                var offset = (Vector3) Random.insideUnitCircle * 200f;
                var delay = Random.Range(0.3f, 0.7f);
                var duration = Random.Range(0.5f, 1f);
                rect.DOAnchorPos(position + offset, 0.3f).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    rect.DOAnchorPos(coinCornerPosition, duration).SetEase(Ease.InQuad).SetDelay(delay)
                        .OnComplete(
                            () =>
                            {
                                Destroy(coin.gameObject);
                                completedCount++;
                            });
                });
            }

            yield return new WaitUntil(() => completedCount >= count);

            data.OnCompleted?.Invoke();
        }
    }
}