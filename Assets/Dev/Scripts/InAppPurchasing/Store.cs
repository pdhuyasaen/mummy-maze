using System;
using System.Collections.Generic;
using System.Linq;
using Dacodelaac.Core;
using Dacodelaac.DebugUtils;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;
using Event = Dacodelaac.Events.Event;

namespace Dev.Scripts.InAppPurchasing
{
    [CreateAssetMenu(menuName = "InAppPurchasing/Store")]
    public class Store : BaseSO, IDetailedStoreListener
    {
        [SerializeField] private ProductData[] products;
        [SerializeField] private SubscriptionData subscription;
        [SerializeField] private ProductDataEvent purchaseBeginEvent;
        [SerializeField] private ProductDataEvent purchaseEndEvent;
        [SerializeField] private ProductDataEvent purchaseSuccessEvent;
        [SerializeField] private Event restorePurchaseBeginEvent;
        [SerializeField] private Event restorePurchaseEndEvent;

        public bool Initialized => controller != null && extensions != null;

        private IStoreController controller;
        private IExtensionProvider extensions;
        private IGooglePlayStoreExtensions googlePlayStoreExtensions;
        private IAppleExtensions appleExtensions;
        private CrossPlatformValidator validator;

        private List<ProductData> pendingPurchases;
        private bool purchaseInProgress;
        private bool gameFullyLoaded;

        public override async void Initialize()
        {
            base.Initialize();
            
            purchaseInProgress = false;
            gameFullyLoaded = false;
            
            var options = new InitializationOptions().SetEnvironmentName("production");
            await UnityServices.InitializeAsync(options);
            
            pendingPurchases = new List<ProductData>();

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in products)
            {
                product.SetStore(this);
                product.Initialize();
                builder.AddProduct(product.Id, product.ProductType, new IDs()
                {
                    {product.AppStoreId, AppleAppStore.Name},
                    {product.GooglePlayId, GooglePlay.Name}
                });
            }

            subscription.Initialize();

            UnityPurchasing.Initialize(this, builder);
        }

        public override void ListenEvents()
        {
            base.ListenEvents();
            subscription.ListenEvents();
        }

        public override void StopListenEvents()
        {
            base.StopListenEvents();
            subscription.StopListenEvents();
        }

        public void OnGameFullyLoaded()
        {
            if (gameFullyLoaded) return;
            gameFullyLoaded = true;

            if (!Initialized)
            {
                Dacoder.LogError("Store is not initialized");
                return;
            }

            foreach (var product in pendingPurchases)
            {
                HandlePurchase(product);
            }

            pendingPurchases.Clear();
        }

        private void HandlePurchase(ProductData product)
        {
            Dacoder.Log($"HandlePurchase, gameFullyLoaded: {gameFullyLoaded}");
            
            if (!product.CanPurchase)
            {
                Dacoder.LogError("Cannot purchased this product ");
                return;
            }

            product.OnPurchased();
            purchaseSuccessEvent.Raise(product);
        }

        public void PurchaseProduct(ProductData productData)
        {
            if (!Initialized)
            {
                Dacoder.LogError("Store is not initialized");
                return;
            }

            if (purchaseInProgress)
            {
                Dacoder.LogError("Other purchasing is in progress");
                return;
            }

            if (!productData.CanPurchase)
            {
                Dacoder.LogError("Cannot purchased this product");
                return;
            }

            var id = productData.Id;

            if (products.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.Ordinal)) == null)
            {
                Dacoder.LogError($"Purchasing failed. Product is not exist in store");
                return;
            }

            var product = controller.products.WithID(id);

            if (product != null && product.availableToPurchase)
            {
                Dacoder.Log($"Purchasing {product.definition.id}");

                purchaseInProgress = true;
                purchaseBeginEvent.Raise(productData);
#if !UNITY_EDITOR
                controller.InitiatePurchase(product);
#endif
            }
            else
            {
                Dacoder.LogError(
                    $"Purchasing failed. Not purchasing product, either is not found or is not available for purchase");
            }
        }

        public void RestorePurchases()
        {
            if (!Initialized)
            {
                Dacoder.LogError("Store is not initialized");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                Dacoder.Log("RestorePurchases started ...");

                purchaseInProgress = true;
                restorePurchaseBeginEvent.Raise();
                appleExtensions.RestoreTransactions(result =>
                {
                    Dacoder.Log("RestorePurchases continuing: " + result +
                                ". If no further messages, no purchases available to restore.");

                    purchaseInProgress = false;
                    restorePurchaseEndEvent.Raise();
                });
            }
            else
            {
                Dacoder.LogError($"RestorePurchases failed. Not supported on {Application.platform}");
            }
        }

        public bool HasBought(string productId)
        {
            var product = products.FirstOrDefault(p => p.Id == productId);
            return product && product.HasPurchased || subscription.Id == productId && subscription.HasPurchased;
        }

        public IProduct GetProduct(string id)
        {
            return (IProduct) products.FirstOrDefault(p => p.Id == id) ??
                   (subscription.Id == id ? subscription : null);
        }

        public string[] GetIds() => products.Select(p => p.Id).Concat(new[] {subscription.Id}).ToArray();

        #region IStoreListener

        public void OnInitialized(IStoreController c, IExtensionProvider e)
        {
            Dacoder.Log("Store is initialized");

            controller = c;
            extensions = e;

            googlePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

            appleExtensions = extensions.GetExtension<IAppleExtensions>();
            appleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

            var introductoryInfoDict = appleExtensions.GetIntroductoryPriceDictionary();

            foreach (var product in products)
            {
                product.OnInitialized(controller.products.WithID(product.Id), introductoryInfoDict);
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Dacoder.LogError($"On purchase failed. Product: {failureDescription.productId}, {failureDescription.reason}, {failureDescription.message}");

            purchaseInProgress = false;
            purchaseEndEvent.Raise(null);
        }

        void OnDeferred(Product item)
        {
            Dacoder.Log("Purchase deferred: " + item.definition.id);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Dacoder.Log("Store initialize failed", error);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Dacoder.Log("Store initialize failed", error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
#if !UNITY_EDITOR
            try
            {
                validator =
 new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
                var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
                Dacoder.Log("ProcessPurchase: Validate - Receipt is valid. Contents:\n{0}",
                    string.Join("\n", result.Select(i => $"{i.productID}/{i.transactionID}: {i.purchaseDate}")));
            }
            catch (Exception e)
            {
                Dacoder.LogError("ProcessPurchase: Invalid - Receipt is invalid, {0}", e.Message);
                purchaseInProgress = false;
                purchaseEndEvent.Raise(null);
                return PurchaseProcessingResult.Complete;
            }
#endif

            var id = purchaseEvent.purchasedProduct.definition.id;
            var product = products.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.Ordinal));
            
            Dacoder.Log($"Process purchase with id: {id} -o- Product: {product}");
            
            if (product == null)
            {
                Dacoder.LogError($"Process purchase failed. Product {id} is not exist in game");
            }
            else
            {
                HandlePurchase(product);
            }

            purchaseInProgress = false;
            purchaseEndEvent.Raise(product);

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Dacoder.LogError($"On purchase failed. Product: {product.definition.storeSpecificId}, {failureReason}");

            purchaseInProgress = false;
            purchaseEndEvent.Raise(null);
        }

        #endregion

        #region Mockup

#if UNITY_EDITOR
        public void MockProcessPurchase(ProductData product)
        {
            if (!products.Contains(product))
            {
                Dacoder.LogError($"Process purchase failed. Product {product.Id} is not exist in store");
            }
            else
            {
                HandlePurchase(product);
            }

            purchaseInProgress = false;
            purchaseEndEvent.Raise(product);
        }

        public void MockOnPurchaseFailed(ProductData product)
        {
            Dacoder.LogError($"On purchase failed. Product: {product.Id}, {PurchaseFailureReason.UserCancelled}");

            purchaseInProgress = false;
            purchaseEndEvent.Raise(product);
        }
#endif

        #endregion
    }
}