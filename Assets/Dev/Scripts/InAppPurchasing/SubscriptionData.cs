using System.Linq;
using Dacodelaac.Core;
using UnityEngine;

namespace Dev.Scripts.InAppPurchasing
{
    [CreateAssetMenu(menuName = "InAppPurchasing/SubscriptionData")]
    public class SubscriptionData : BaseSO, IProduct
    {
        [SerializeField] ProductData[] products;

        public event System.Action<IProduct> OnPurchasedEvent;

        public bool CanPurchase => !HasPurchased;
        public bool HasPurchased => products.Any(p => p.HasPurchased);

        public override void Initialize()
        {
            base.Initialize();
            foreach (var product in products)
            {
                product.SetSubscription(this);
            }
        }

        public override void ListenEvents()
        {
            base.ListenEvents();
            foreach (var product in products)
            {
                product.OnPurchasedEvent += OnPurchased;
            }
        }

        public override void StopListenEvents()
        {
            base.StopListenEvents();
            foreach (var product in products)
            {
                product.OnPurchasedEvent -= OnPurchased;
            }
        }

        public void Purchase()
        {
        }

        void OnPurchased(IProduct p)
        {
            StopListenEvents();

            foreach (var product in products)
            {
                if (product != p)
                {
                    product.TriggerPurchased();
                }
            }

            OnPurchasedEvent?.Invoke(this);
        }
    }
}