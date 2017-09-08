//-----------------------------------------------------------------------
// <copyright file="UnityPurchasingManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_PURCHASING && USE_PLAYFAB_SDK

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Purchasing;

    public class UnityPurchasingManager : SingletonGameObject<UnityPurchasingManager>, IStoreListener
    {
        private enum InitializationState
        {
            Initializing,
            InitializeFailed,
            InitializedSucceeded,
        }

        private enum PurchasingState
        {
            PurchasingWaiting,
            Purchasing,
            PurchasingFailed,
            PurchasingSucceeded,
        }

        // initialization
        private IStoreController controller;
        private ConfigurationBuilder builder;
        private InitializationState initializationState;
        private InitializationFailureReason initializationFailureReason;

        // purchasing
        private PurchasingState purchasingState;
        private PurchaseFailureReason purchaseFailureReason;
        private PurchaseEventArgs purchaseEventArgs;

        public bool IsIAPInitialized
        {
            get { return initializationState == InitializationState.InitializedSucceeded; }
        }

        public UnityTask<bool> InitializeUnityPurchasing()
        {
            return UnityTask<bool>.Run(this.InitializeUnityPurchasingCoroutine());
        }

        public UnityTask<PurchaseEventArgs> PurchaseProduct(StoreItem storeItem)
        {
            return UnityTask<PurchaseEventArgs>.Run(this.PurchaseProductCoroutine(storeItem));
        }

        private ConfigurationBuilder GetConfigurationBuilder()
        {
            if (this.builder == null)
            {
                var module = StandardPurchasingModule.Instance();

                if (Debug.isDebugBuild || Application.isEditor)
                {
                    module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
                }

                this.builder = ConfigurationBuilder.Instance(module);

                var activeCatalog = Catalogs.Instance.GetCatalog(AppSettings.ActiveConfig.CatalogVersion);
                Debug.AssertFormat(activeCatalog != null, "UnityPurchasingManager.GetConfigurationBuilder Cloudn't find Catalog Version {0}", AppSettings.ActiveConfig.CatalogVersion);

                foreach (var catalogItem in activeCatalog.CatalogItems.Where(x => x.IsIapItem))
                {
                    this.builder.AddProduct(catalogItem.Id, catalogItem.UsageType == UsageType.Consumable ? ProductType.Consumable : ProductType.NonConsumable);
                }

                foreach (var bundleItem in activeCatalog.BundleItems.Where(x => x.IsIapItem))
                {
                    this.builder.AddProduct(bundleItem.Id, ProductType.Consumable);
                }
            }

            return this.builder;
        }
        
        private IEnumerator<bool> InitializeUnityPurchasingCoroutine()
        {
            if (this.initializationState == InitializationState.InitializedSucceeded)
            {
                yield return true;
                yield break;
            }

            this.initializationState = InitializationState.Initializing;
            UnityPurchasing.Initialize(this, this.GetConfigurationBuilder());

            while (this.initializationState == InitializationState.Initializing)
            {
                yield return default(bool);
            }

            if (this.initializationState == InitializationState.InitializedSucceeded)
            {
                yield return true;
            }
            else
            {
                throw new PurchasingInitializationException(initializationFailureReason);
            }
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.initializationState = InitializationState.InitializedSucceeded;
            this.controller = controller;
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            this.initializationState = InitializationState.InitializeFailed;
            this.initializationFailureReason = error;
        }
        
        private IEnumerator<PurchaseEventArgs> PurchaseProductCoroutine(StoreItem storeItem)
        {
            Debug.Assert(storeItem != null, "Trying to buy a null store item!");
            Debug.AssertFormat(storeItem.IsIapItem, "Trying to buy store item {0}, which is not an IAP item!", storeItem.ItemId);

            if (this.purchasingState != PurchasingState.PurchasingWaiting)
            {
                throw new PurchasingException(PurchaseFailureReason.ExistingPurchasePending);
            }

            this.purchasingState = PurchasingState.Purchasing;
            this.purchaseFailureReason = PurchaseFailureReason.Unknown;
            this.purchaseEventArgs = null;

            this.controller.InitiatePurchase(storeItem.ItemId);

            while (this.purchasingState == PurchasingState.Purchasing)
            {
                yield return default(PurchaseEventArgs);
            }

            bool wasSuccessful = this.purchasingState == PurchasingState.PurchasingSucceeded;

            this.purchasingState = PurchasingState.PurchasingWaiting;

            if (wasSuccessful)
            {
                yield return this.purchaseEventArgs;
            }
            else
            {
                throw new PurchasingException(this.purchaseFailureReason);
            }
        }
        
        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
        {
            this.purchasingState = PurchasingState.PurchasingSucceeded;
            this.purchaseEventArgs = e;

            return PurchaseProcessingResult.Complete;
        }

        void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            this.purchasingState = PurchasingState.PurchasingFailed;
            this.purchaseFailureReason = p;
        }
    }
}

#endif
