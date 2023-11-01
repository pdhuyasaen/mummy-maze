using System;
using Dacodelaac.DataStorage;
using Dacodelaac.Events;
using UnityEngine;

namespace Dacodelaac.Variables
{
    public class BaseVariable<TType> : BaseEvent<TType>, IVariable<TType>, ISerializationCallbackReceiver
    {
        [SerializeField] TType initializeValue;
        [SerializeField] bool isSavable;
        [SerializeField] bool isRaiseEvent;
        [NonSerialized] TType runtimeValue;

        public TType Value
        {
            get => isSavable ? GameData.Get(Id, initializeValue) : runtimeValue;
            set
            {
                if (isSavable)
                {
                    GameData.Set(Id, value);
                }
                else
                {
                    runtimeValue = value;
                }
                if (isRaiseEvent)
                {
                    Raise(value);
                }
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            runtimeValue = initializeValue;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}