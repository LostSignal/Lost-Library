//-----------------------------------------------------------------------
// <copyright file="PlayFabMessages.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using BigBlindInteractive.TienLen;
    using PlayFab;
    using UnityEngine;
    using UnityEngine.Purchasing;

    public static class PlayFabMessages
    {
        public static UnityTask<YesNoResult> ShowInsufficientCurrency()
        {
            var localizeStrings = BigBlindInteractive.TienLen.MessageBoxLocalization.Instance.insufficientCurrency;
            return MessageBox.Instance.ShowYesNo(localizeStrings.Title, localizeStrings.Body);
        }

        public static void ShowConnectingToStoreSpinner()
        {
            var localizeStrings = BigBlindInteractive.TienLen.MessageBoxLocalization.Instance.connectingToStoreSpinner;
            SpinnerBox.Instance.UpdateBodyText(localizeStrings.Body);
        }

        public static UnityTask<StringInputResult> ShowChangeDisplayNameInputBox(string currentDisplayName)
        {
            var localizeStrings = BigBlindInteractive.TienLen.MessageBoxLocalization.Instance.changeName;
            var title = localizeStrings.Title;
            var body = localizeStrings.Body;
            return StringInputBox.Instance.Show(title, body, currentDisplayName);
        }

        public static UnityTask<OkResult> HandleError(System.Exception exception)
        {
            UnityTask<OkResult> result = null;

            #if USING_UNITY_PURCHASING
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

            var changeNameStrings = BigBlindInteractive.TienLen.MessageBoxLocalization.Instance.changeName;
            var purchaseFailedStrings = BigBlindInteractive.TienLen.MessageBoxLocalization.Instance.failedPurchase;

            switch (playfabException.Error.Error)
            {
                // Display Name Changing Errors
                case PlayFabErrorCode.NameNotAvailable:
                    MessageBox.Instance.ShowOk(changeNameStrings.RenameFailedTitle, changeNameStrings.RenameFailed_NotAvaialbe);
                    break;

                case PlayFabErrorCode.ProfaneDisplayName:
                    MessageBox.Instance.ShowOk(changeNameStrings.RenameFailedTitle, changeNameStrings.RenameFailed_ProfaneDisplayName);
                    break;

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
                    MessageBox.Instance.ShowOk(purchaseFailedStrings.Title, purchaseFailedStrings.UnableToValidateReceipt);
                    break;

                // TODO [bgish] - handle way more...
            }

            return null;
        }

        #if USING_UNITY_PURCHASING

        private static UnityTask<OkResult> HandlePurchasingInitializationError(PurchasingInitializationException purchasingInitializationException)
        {
            if (purchasingInitializationException == null)
            {
                return null;
            }

            var localizeStrings = BigBlindInteractive.TienLen.MessageBoxLocalization.Instance.storeError;

            switch (purchasingInitializationException.FailureReason)
            {
                case InitializationFailureReason.AppNotKnown:
                    Debug.LogErrorFormat("Error initializing purchasing \"{0}\"", purchasingInitializationException.FailureReason.ToString());
                    return MessageBox.Instance.ShowOk(localizeStrings.Title, localizeStrings.AppNotKnown);

                case InitializationFailureReason.NoProductsAvailable:
                    Debug.LogErrorFormat("Error initializing purchasing \"{0}\"", purchasingInitializationException.FailureReason.ToString());
                    return MessageBox.Instance.ShowOk(localizeStrings.Title, localizeStrings.NoProductsAvailable);

                case InitializationFailureReason.PurchasingUnavailable:
                    return MessageBox.Instance.ShowOk(localizeStrings.Title, localizeStrings.PurchasingUnavailable);

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
            var localizeStrings = BigBlindInteractive.TienLen.MessageBoxLocalization.Instance.storeError;
            return MessageBox.Instance.ShowOk(localizeStrings.Title, localizeStrings.ConnectTimeOut);
        }

        private static UnityTask<OkResult> HandlePurchasingError(PurchasingException purchasingException)
        {
            if (purchasingException == null)
            {
                return null;
            }

            var localizeStrings = MessageBoxLocalization.Instance.failedPurchase;
            string messageBoxTitle = localizeStrings.Title;

            switch (purchasingException.FailureReason)
            {
                case PurchaseFailureReason.DuplicateTransaction:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, localizeStrings.DuplicateTransaction);

                case PurchaseFailureReason.ExistingPurchasePending:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, localizeStrings.ExistingPurchasePending);

                case PurchaseFailureReason.PaymentDeclined:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, localizeStrings.PaymentDeclined);

                case PurchaseFailureReason.ProductUnavailable:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, localizeStrings.ProductUnavailable);

                case PurchaseFailureReason.PurchasingUnavailable:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, localizeStrings.PurchasingUnavailable);

                case PurchaseFailureReason.SignatureInvalid:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, localizeStrings.SignatureInvalid);

                case PurchaseFailureReason.Unknown:
                    return MessageBox.Instance.ShowOk(messageBoxTitle, localizeStrings.Unknown);

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
