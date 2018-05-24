//-----------------------------------------------------------------------
// <copyright file="PlayFabMessages.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USE_PLAYFAB_SDK

namespace Lost
{
    using PlayFab;
    using UnityEngine;
    using UnityEngine.Purchasing;

    public static class PlayFabMessages
    {
        public static UnityTask<YesNoResult> ShowInsufficientCurrency()
        {
            return MessageBox.Instance.ShowYesNo("Not Enough Currency", "You'll need to buy more currency from the store.<br>Would you like to go there now?");
        }

        public static void ShowConnectingToStoreSpinner()
        {
            SpinnerBox.Instance.UpdateBodyText("Connecting To Store...");
        }

        public static UnityTask<OkResult> HandleError(System.Exception exception)
        {
            UnityTask<OkResult> result = null;

            #if UNITY_PURCHASING
            result = result ?? HandlePurchasingError(exception as PurchasingException);
            result = result ?? HandlePurchasingInitializationError(exception as PurchasingInitializationException);
            result = result ?? HandlePurchasingInitializationTimeOutError(exception as PurchasingInitializationTimeOutException);
            #endif

            return result ?? HandlePlayFabError(exception as PlayFabException);
        }

        private static UnityTask<OkResult> HandlePlayFabError(PlayFabException playfabException)
        {
            if (playfabException == null)
            {
                return null;
            }

            switch (playfabException.Error.Error)
            {
                // android receipt errors
                case PlayFabErrorCode.ReceiptCancelled:

                // ios receipt errors
                case PlayFabErrorCode.ReceiptDoesNotContainInAppItems:
                case PlayFabErrorCode.ReceiptContainsMultipleInAppItems:
                case PlayFabErrorCode.InvalidBundleID:
                case PlayFabErrorCode.InvalidVirtualCurrency:
                case PlayFabErrorCode.DownstreamServiceUnavailable:
                case PlayFabErrorCode.InvalidCurrencyCode:

                // common recipt errors between iOS/Android
                case PlayFabErrorCode.InvalidReceipt:
                case PlayFabErrorCode.ReceiptAlreadyUsed:
                case PlayFabErrorCode.NoMatchingCatalogItemForReceipt:
                    Debug.LogErrorFormat("Hit Error {0} while validating receipt", playfabException.Error.Error);
                    MessageBox.Instance.ShowOk("Purchase Failed", "Unable to validate receipt.");
                    break;

                // TODO [bgish] - handle way more...
            }

            return null;
        }

        #if UNITY_PURCHASING

        private static UnityTask<OkResult> HandlePurchasingInitializationError(PurchasingInitializationException purchasingInitializationException)
        {
            if (purchasingInitializationException == null)
            {
                return null;
            }

            switch (purchasingInitializationException.FailureReason)
            {
                case InitializationFailureReason.AppNotKnown:
                    Debug.LogErrorFormat("Error initializing purchasing \"{0}\"", purchasingInitializationException.FailureReason.ToString());
                    return MessageBox.Instance.ShowOk("Store Error", "The store doesn't recognize this application.");

                case InitializationFailureReason.NoProductsAvailable:
                    Debug.LogErrorFormat("Error initializing purchasing \"{0}\"", purchasingInitializationException.FailureReason.ToString());
                    return MessageBox.Instance.ShowOk("Store Error", "There are no valid products available for this application.");

                case InitializationFailureReason.PurchasingUnavailable:
                    return MessageBox.Instance.ShowOk("Store Error", "Unable to purchase.  Purchases have been turned off for this application.");

                default:
                    return null;
            }
        }

        private static UnityTask<OkResult> HandlePurchasingInitializationTimeOutError(PurchasingInitializationTimeOutException purchasingInitializationTimeOutException)
        {
            if (purchasingInitializationTimeOutException == null)
            {
                return null;
            }

            return MessageBox.Instance.ShowOk("Store Error", "We timed out trying to connect to the store.");
        }

        private static UnityTask<OkResult> HandlePurchasingError(PurchasingException purchasingException)
        {
            if (purchasingException == null)
            {
                return null;
            }

            string messageBoxTitle = "Purchase Failed";

            switch (purchasingException.FailureReason)
            {
                case PurchaseFailureReason.DuplicateTransaction:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, "We've encountered a duplicate transaction.");

                case PurchaseFailureReason.ExistingPurchasePending:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, "An existing purchase is already pending.");

                case PurchaseFailureReason.PaymentDeclined:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, "The payment has been declined.");

                case PurchaseFailureReason.ProductUnavailable:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, "The product is unavailable.");

                case PurchaseFailureReason.PurchasingUnavailable:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, "Purchasing is currenctly unavailable.");

                case PurchaseFailureReason.SignatureInvalid:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, "Signature was invalid.");

                case PurchaseFailureReason.Unknown:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, "Sorry we've encountered an unknown error.");

                case PurchaseFailureReason.UserCancelled:
                    // Do nothing, they know they canceled it
                    return null;

                default:
                    return null;
            }
        }

        #endif
    }
}

#endif
