using System;
using System.Collections.Generic;
using _Root.Scripts.Pattern;
using Dacodelaac.Core;
using Dacodelaac.DataStorage;
using Dacodelaac.DebugUtils;
using Dacodelaac.Variables;
using UnityEngine;
using UnityEngine.Purchasing;
using Event = Dacodelaac.Events.Event;

namespace Dev.Scripts.InAppPurchasing
{
    [CreateAssetMenu(menuName = "InAppPurchasing/ProductData")]
    public class ProductData : BaseSO, IProduct
    {
        [SerializeField] private string googlePlayId;
        [SerializeField] private string appStoreId;
        [SerializeField] private ProductType productType;
        [SerializeField] private float purchaseInterval = -1;

        [SerializeField] private double price;
        [SerializeField] private BooleanVariable noAds;
        [SerializeField] private Event hideBannerEvent;
        [SerializeField] private bool isNoAdsProduct;
        public string GooglePlayId => googlePlayId;
        public string AppStoreId => appStoreId;
        public string StoreSpecificId => Application.platform == RuntimePlatform.Android ? GooglePlayId : AppStoreId;
        public ProductType ProductType => productType;

        public event Action<IProduct> OnChangedEvent;
        public event Action<IProduct> OnPurchasedEvent;
        public bool HasPurchased { get; }

        private Store store;
        private ProductMetadata metadata;
        private SubscriptionData subscription;
        private SubscriptionInfo subscriptionInfo;
        
#if UNITY_EDITOR
        public string PriceString => $"{price}$";
        public double PriceDouble => price;
#else
        public string PriceString => metadata == null ? $"{price}$" : metadata.localizedPriceString;
        public double PriceDouble => metadata == null ? price : double.Parse(metadata.localizedPrice.ToString());
#endif

        public int PurchasesCount
        {
            get => GameData.Get($"{Id}_count", 0);
            set => GameData.Set($"{Id}_count", value);
        }

        public double LastTimePurchase
        {
            get => GameData.Get($"{Id}_last_time", 0);
            set => GameData.Set($"{Id}_last_time", value);
        }

        public bool Purchased
        {
            get => GameData.Get($"{Id}_purchased", false);
            set => GameData.Set($"{Id}_purchased", value);
        }

        public bool CanPurchase
        {
            get
            {
                switch (productType)
                {
                    case ProductType.Consumable:
                        return purchaseInterval < 0 || GameUtils.TimeNow - LastTimePurchase > purchaseInterval;
                    case ProductType.NonConsumable:
                        return !HasPurchased;
                    case ProductType.Subscription:
                        return false;
                    default:
                        return false;
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            if (productType == ProductType.Consumable && GameUtils.TimeNow - LastTimePurchase > purchaseInterval)
            {
                Purchased = false;
            }

            subscriptionInfo = null;
            subscription = null;
            metadata = null;
        }

        public void OnInitialized(Product product, Dictionary<string, string> introductoryInfoDict)
        {
            metadata = product.metadata;
            
            if (productType == ProductType.Subscription && product.receipt != null)
            {
                if (CheckIfProductIsAvailableForSubscriptionManager(product.receipt))
                {
                    var introJson = (introductoryInfoDict == null || !introductoryInfoDict.ContainsKey(StoreSpecificId))
                        ? null
                        : introductoryInfoDict[StoreSpecificId];
                    subscriptionInfo = new SubscriptionManager(product, introJson).getSubscriptionInfo();
                    if (subscriptionInfo != null)
                    {
                        if (subscriptionInfo.isSubscribed() == Result.False)
                        {
                            Purchased = false;
                        }

                        Dacoder.Log("-----SubscriptionInfo-----");
                        Dacoder.Log("productId", subscriptionInfo.getProductId());
                        Dacoder.Log("purchaseDate", subscriptionInfo.getPurchaseDate());
                        Dacoder.Log("expireDate", subscriptionInfo.getExpireDate());
                        Dacoder.Log("isSubscribed" + subscriptionInfo.isSubscribed());
                        Dacoder.Log("isExpired", subscriptionInfo.isExpired());
                        Dacoder.Log("isCancelled", subscriptionInfo.isCancelled());
                        Dacoder.Log("isFreeTrial", subscriptionInfo.isFreeTrial());
                        Dacoder.Log("isAutoRenewing", subscriptionInfo.isAutoRenewing());
                        Dacoder.Log("remainingTime", subscriptionInfo.getRemainingTime());
                        Dacoder.Log("isIntroductoryPricePeriod", subscriptionInfo.isIntroductoryPricePeriod());
                        Dacoder.Log("introductoryPrice", subscriptionInfo.getIntroductoryPrice());
                        Dacoder.Log("introductoryPricePeriod", subscriptionInfo.getIntroductoryPricePeriod());
                        Dacoder.Log("introductoryPricePeriodCycles",
                            subscriptionInfo.getIntroductoryPricePeriodCycles());
                        Dacoder.Log("---------------------------");
                    }
                }
                else
                {
                    Dacoder.LogError(
                        "This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                }
            }

            OnChangedEvent?.Invoke(this);
        }

        bool CheckIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receiptWrapper = (Dictionary<string, object>) MiniJson.JsonDecode(receipt);
            if (!receiptWrapper.ContainsKey("Store") || !receiptWrapper.ContainsKey("Payload"))
            {
                Dacoder.LogError("The product receipt does not contain enough information");
                return false;
            }

            var store = (string) receiptWrapper["Store"];
            var payload = (string) receiptWrapper["Payload"];

            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                    {
                        var payloadWrapper = (Dictionary<string, object>) MiniJson.JsonDecode(payload);
                        if (!payloadWrapper.ContainsKey("json"))
                        {
                            Dacoder.LogError(
                                "The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }

                        var originalJsonPayloadWrapper =
                            (Dictionary<string, object>) MiniJson.JsonDecode((string) payloadWrapper["json"]);
                        if (originalJsonPayloadWrapper == null ||
                            !originalJsonPayloadWrapper.ContainsKey("developerPayload"))
                        {
                            Dacoder.LogError(
                                "The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }

                        var developerPayloadJson = (string) originalJsonPayloadWrapper["developerPayload"];
                        var developerPayloadWrapper =
                            (Dictionary<string, object>) MiniJson.JsonDecode(developerPayloadJson);
                        if (developerPayloadWrapper == null || !developerPayloadWrapper.ContainsKey("is_free_trial") ||
                            !developerPayloadWrapper.ContainsKey("has_introductory_price_trial"))
                        {
                            Dacoder.Log(
                                "The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }

                        return true;
                    }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                    {
                        return true;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        public void Purchase()
        {
#if !UNITY_EDITOR
            store.PurchaseProduct(this);
#else
            OnPurchased();
#endif
        }

        public void SetStore(Store s)
        {
            store = s;
        }

        public void SetSubscription(SubscriptionData s)
        {
            if (productType != ProductType.Subscription)
            {
                return;
            }

            subscription = s;
        }

        public void OnPurchased()
        {
            Dacoder.Log($"Purchased product {Id} success");
            if (isNoAdsProduct)
            {
                noAds.Value = true;
                hideBannerEvent.Raise();
            }
            PurchasesCount++;
            Purchased = true;
            LastTimePurchase = GameUtils.TimeNow;
            TriggerPurchased();
            GameData.Save();
        }

        public void TriggerPurchased()
        {
            OnPurchasedEvent?.Invoke(this);
            OnChangedEvent?.Invoke(this);
        }
    }
}