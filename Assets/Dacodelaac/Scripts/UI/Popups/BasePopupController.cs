using System;
using System.Collections.Generic;
using Dacodelaac.Core;
using Dacodelaac.DebugUtils;
using UnityEngine;

namespace Dacodelaac.UI.Popups
{
    [RequireComponent(typeof(Canvas))]
    public class BasePopupController : BaseMono
    {
        [SerializeField] BasePopup[] popupPrefabs;
        [SerializeField] bool manualInitialize;

        public BasePopup Current => popups.Last?.Value;
        
        private Canvas canvas;
        private Dictionary<Type, BasePopup> popupDict;
        private LinkedList<BasePopup> popups;
        private bool initialized;

        public override void Initialize()
        {
            if (initialized) return;
            
            canvas = GetComponent<Canvas>();

            popupDict = new Dictionary<Type, BasePopup>();
            popups = new LinkedList<BasePopup>();
            if (!manualInitialize)
            {
                foreach (var prefab in popupPrefabs)
                {
                    var popup = Instantiate(prefab, transform);
                    popup.name = prefab.name;
                }
            }
            var ps = GetComponentsInChildren<BasePopup>(true);
            foreach (var popup in ps)
            {
                popup.gameObject.SetActive(false);
                
                popup.Initialize(this);
                popup.SetOrder(canvas.sortingOrder + 1);
                
                var type = popup.GetType();
                popupDict.Add(type, popup);
            }
            
            initialized = true;
        }

        public void Show<T>(bool animated, ShowAction showAction = ShowAction.DoNothing, object data = null)
        {
            if (!popupDict.TryGetValue(typeof(T), out var popup))
            {
                Dacoder.Log("Cannot find that popup!");
                return;
            }

            Show(popup, animated, showAction, data);
        }

        public void Show(BasePopup basePopup, bool animated, ShowAction showAction = ShowAction.DoNothing, object data = null)
        {
            var t = GetTopPopup();
            if (t == basePopup) return;

            if (t != null)
            {
                switch (showAction)
                {
                    case ShowAction.DoNothing:
                        break;
                    case ShowAction.DismissCurrent:
                        RemoveLast();
                        t.Dismiss(animated);
                        break;
                    case ShowAction.PauseCurrent:
                        t.Pause(animated);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(showAction), showAction, null);
                }
            }

            AddLast(basePopup);

            basePopup.Show(animated, data);
        }

        public void Dismiss<T>(bool animated)
        {
            if (!popupDict.TryGetValue(typeof(T), out var popup))
            {
                Dacoder.Log("Cannot find that popup!");
                return;
            }
            
            Dismiss(popup, animated);
        }
        
        public void DismissCurrent(bool animated)
        {
            var last = popups.Last;
            if (last != null)
            {
                Dismiss(last.Value, animated);
            }
        }

        public void Dismiss(BasePopup basePopup, bool animated)
        {
            if (!Remove(basePopup)) return;
            
            basePopup.Dismiss(animated);
            var t = GetTopPopup();
            if (t == null) return;
                
            t.Resume(true);
        }

        private void Reorder()
        {
            var p = popups.First;
            var i = canvas.sortingOrder;
            while (p != null)
            {
                p.Value.SetOrder(++i);
                p = p.Next;
            }
        }

        private void AddLast(BasePopup basePopup)
        {
            if (popups.Contains(basePopup))
            {
                popups.Remove(basePopup);
            }

            popups.AddLast(basePopup);
            Reorder();
        }

        private bool Remove(BasePopup basePopup)
        {
            if (!popups.Remove(basePopup)) return false;
            
            Reorder();
            return true;

        }

        private void RemoveLast()
        {
            popups.RemoveLast();
            Reorder();
        }

        private BasePopup GetTopPopup()
        {
            return popups.Last?.Value;
        }

        public T GetPopup<T>() where T : BasePopup
        {
            return popupDict[typeof(T)] as T;
        }

        public enum ShowAction
        {
            DoNothing,
            DismissCurrent,
            PauseCurrent
        }
    }
}