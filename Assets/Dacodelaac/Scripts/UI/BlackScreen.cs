using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dacodelaac.UI
{
    public class BlackScreen : MonoBehaviour
    {
        [SerializeField] Image image;

        public void Initialize()
        {
            image.gameObject.SetActive(false);
            DontDestroyOnLoad(gameObject);
        } 
        
        public void Show(float duration, System.Action onCompleted)
        {
            image.gameObject.SetActive(true);
            var color = image.color;
            color.a = 0;
            image.color = color;

            DOTween.Kill(this);
            image.DOFade(1, duration).SetTarget(this).OnComplete(() => onCompleted?.Invoke());
        }
        
        public void Hide(float duration, System.Action onCompleted)
        {
            DOTween.Kill(this);
            image.DOFade(0, duration).SetTarget(this).OnComplete(() =>
            {
                image.gameObject.SetActive(false);
                onCompleted?.Invoke();
            });
        }
    }
}