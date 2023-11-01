using UnityEngine;

namespace Dacodelaac.UI
{
    public class LoadingBlockerCanvas : MonoBehaviour
    {
        static LoadingBlockerCanvas instance;
        public static LoadingBlockerCanvas Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LoadingBlockerCanvas>();
                    instance.Hide();
                }

                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                instance.Hide();
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}