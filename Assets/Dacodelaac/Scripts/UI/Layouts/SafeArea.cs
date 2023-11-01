using Dacodelaac.Core;
using UnityEngine;

namespace Dacodelaac.UI.Layouts
{
    public class SafeArea : BaseMono
    {
        RectTransform rect;
        Rect lastSafeArea = new Rect(0, 0, 0, 0);
        
        public override void Initialize()
        {
            rect = GetComponent<RectTransform>();
            Refresh();
        }

        public override void Tick()
        {
            Refresh();
        }

        void Refresh()
        {
            var safeArea = Screen.safeArea;

            if (safeArea != lastSafeArea)
            {
                ApplySafeArea(safeArea);
            }
        }

        void ApplySafeArea(Rect r)
        {
            lastSafeArea = r;

            var anchorMin = r.position;
            var anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }
    }
}