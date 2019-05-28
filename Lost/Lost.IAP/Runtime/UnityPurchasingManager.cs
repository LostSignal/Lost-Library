//-----------------------------------------------------------------------
// <copyright file="UnityPurchasingManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_PURCHASING && !UNITY_XBOXONE

namespace Lost.IAP
{
    using System.Collections.Generic;
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

        public string GetLocalizedPrice(string itemId)
        {
            if (this.controller == null)
            {
                Debug.LogError("Tried to get Localized Price of {0} before inittializing UnityIAP!");
                return null;
            }

            Product product = this.controller.products.WithID(itemId);

            if (product != null)
            {
                return product.metadata.localizedPriceString;
            }

            return null;
        }

        public UnityTask<bool> InitializeUnityPurchasing(System.Action<AppStore, ConfigurationBuilder> configurationBuilder)
        {
            return UnityTask<bool>.Run(InitializeUnityPurchasingCoroutine());

            IEnumerator<bool> InitializeUnityPurchasingCoroutine()
            {
                if (this.initializationState == InitializationState.InitializedSucceeded)
                {
                    yield return true;
                    yield break;
                }

                float startTime = Time.realtimeSinceStartup;
                this.initializationState = InitializationState.Initializing;
                UnityPurchasing.Initialize(this, this.GetConfigurationBuilder(configurationBuilder));

                while (this.initializationState == InitializationState.Initializing)
                {
                    yield return default(bool);

                    if (Time.realtimeSinceStartup - startTime > 5.0f)
                    {
                        throw new PurchasingInitializationTimeOutException();
                    }
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
        }

        public UnityTask<PurchaseEventArgs> PurchaseProduct(string itemId)
        {
            return UnityTask<PurchaseEventArgs>.Run(this.PurchaseProductCoroutine(itemId));
        }

        private ConfigurationBuilder GetConfigurationBuilder(System.Action<AppStore, ConfigurationBuilder> configurationBuilder)
        {
            if (this.builder == null)
            {
                var module = StandardPurchasingModule.Instance();

                if (Debug.isDebugBuild || Application.isEditor)
                {
                    module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
                }

                this.builder = ConfigurationBuilder.Instance(module);

                configurationBuilder?.Invoke(module.appStore, this.builder);
            }

            return this.builder;
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

        private IEnumerator<PurchaseEventArgs> PurchaseProductCoroutine(string itemId)
        {
            if (this.purchasingState != PurchasingState.PurchasingWaiting)
            {
                throw new PurchasingException(PurchaseFailureReason.ExistingPurchasePending);
            }

            this.purchasingState = PurchasingState.Purchasing;
            this.purchaseFailureReason = PurchaseFailureReason.Unknown;
            this.purchaseEventArgs = null;

            this.controller.InitiatePurchase(itemId);

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
