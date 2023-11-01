using Dacodelaac.Collections;
using UnityEngine;

namespace Dacodelaac.Core
{
    public class BaseTickMono : BaseMono, ITick
    {
        [SerializeField] TickCollection earlyTickCollection;
        [SerializeField] TickCollection tickCollection;
        [SerializeField] TickCollection lateTickCollection;
        [SerializeField] TickCollection fixedTickCollection;

        protected virtual void OnEnable()
        {
            if (earlyTickCollection) earlyTickCollection.Add(this);
            if (tickCollection) tickCollection.Add(this);
            if (lateTickCollection) lateTickCollection.Add(this);
            if (fixedTickCollection) fixedTickCollection.Add(this);
        }

        protected virtual void OnDisable()
        {
            if (earlyTickCollection) earlyTickCollection.Remove(this);
            if (tickCollection) tickCollection.Remove(this);
            if (lateTickCollection) lateTickCollection.Remove(this);
            if (fixedTickCollection) fixedTickCollection.Remove(this);
        }
        
        public virtual void EarlyTick()
        {
        }

        public virtual void Tick()
        {
        }

        public virtual void LateTick()
        {
        }

        public virtual void FixedTick()
        {
        }
    }
}