using System.Collections.Generic;
using Dacodelaac.Core;
using Dacodelaac.Variables;
using DG.DemiLib.Attributes;
using Lean.Touch;
using UnityEngine;

namespace Dacodelaac.Inputs
{
    [DeScriptExecutionOrder(ExecutionOrder.Third)]
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] InputHandlerDataVariable inputHandlerDataVariable;

        InputHandlerData data = new InputHandlerData();
        LeanFingerFilter use = new LeanFingerFilter(true);
        List<LeanFinger> fingers;

        void Start()
        {
            inputHandlerDataVariable.Value = data;
        }

        void Update()
        {
            data.Finger = null;
            if (data.Stopped)
            {
                return;
            }
            fingers = use.GetFingers();
            if (fingers.Count > 0)
            {
                data.Finger = fingers[0];
            }
        }

        public void OnStopInput()
        {
            data.Stopped = true;
        }
    }
}