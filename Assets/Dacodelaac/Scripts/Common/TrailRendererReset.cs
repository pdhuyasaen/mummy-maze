using Dacodelaac.Core;
using UnityEngine;

namespace Dacodelaac.Common
{
    public class TrailRendererReset : BaseMono
    {
        public override void DoDisable()
        {
            base.DoDisable();
            GetComponent<TrailRenderer>().Clear();
        }
    }
}