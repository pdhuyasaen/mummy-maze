using System.Collections.Generic;
using Dacodelaac.Core;
using Dacodelaac.DebugUtils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dacodelaac.Events
{
    public class BaseEvent : BaseSO, IEvent
    {
        readonly List<IEventListener> listeners = new List<IEventListener>();

        public void Raise()
        {
#if UNITY_EDITOR
            Dacoder.Log($"===> {name}");
#endif
            for (var i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void AddListener(IEventListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public void RemoveListener(IEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }

        public void RemoveAll()
        {
            listeners.Clear();
        }
    }

    public class BaseEvent<TType> : BaseSO, IEvent<TType>
    {
        readonly List<IEventListener<TType>> listeners = new List<IEventListener<TType>>();

        public void Raise(TType value)
        {
#if UNITY_EDITOR
            Dacoder.Log($"===> {name}");
#endif
            for (var i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(value);
            }
        }

        public void AddListener(IEventListener<TType> listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public void RemoveListener(IEventListener<TType> listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }

        public void RemoveAll()
        {
            listeners.Clear();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BaseEvent), true)]
    public class BaseEventEditor : Editor
    {
        BaseEvent baseEvent;

        void OnEnable()
        {
            baseEvent = target as BaseEvent;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Raise"))
            {
                baseEvent.Raise();
            }
        }
    }
    //
    // [CustomEditor(typeof(BaseEvent<>), true)]
    // public class TypedBaseEventEditor : Editor
    // {
    //     public override void OnInspectorGUI()
    //     {
    //         base.OnInspectorGUI();
    //
    //         var debugProperty = serializedObject.FindProperty("debugValue");
    //
    //         if (debugProperty != null)
    //         {
    //             using (new EditorGUIUtils.VerticalHelpBox())
    //             {
    //                 using (var scope = new EditorGUI.ChangeCheckScope())
    //                 {
    //                     EditorGUILayout.PropertyField(debugProperty, GUIContent.none);
    //
    //                     if (scope.changed)
    //                     {
    //                         serializedObject.ApplyModifiedProperties();
    //                     }
    //                 }
    //
    //                 if (GUILayout.Button("Raise"))
    //                 {
    //                     var targetType = target.GetType();
    //                     var targetField = targetType.GetFieldRecursive("debugValue",
    //                         BindingFlags.Instance | BindingFlags.NonPublic);
    //                     var debugValue = targetField.GetValue(target);
    //
    //                     var raiseMethod = targetType.BaseType.GetMethod("Raise",
    //                         BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
    //                     raiseMethod?.Invoke(target, new[] {debugValue});
    //                 }
    //             }
    //         }
    //     }
    // }
#endif
}