//-----------------------------------------------------------------------
// <copyright file="PlayFabMessages.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using PlayFab;
    using UnityEngine;

    #if USING_UNITY_PURCHASING && !UNITY_XBOXONE
    using UnityEngine.Purchasing;
    #endif

    public static class PlayFabMessages
    {
        // Exit App Keys
        private const string ExitAppTitleKey = "EXIT-APP-TITLE";
        private const string ExitAppBodyKey = "EXIT-APP-BODY";

        // Forgot Password Keys
        private const string ForgotPasswordTitleKey = "FORGOT-PASSWORD-TITLE";
        private const string ForgotPasswordBodyKey = "FORGOT-PASSWORD-BODY";

        // Changing Display Name Localization Keys
        private const string ChangeNameTitleKey = "CHANGE-NAME-TITLE";
        private const string ChangeNameBodyKey = "CHANGE-NAME-BODY";
        private const string ChangeNameFailedTitleKey = "CHANGE-NAME-FAILED-TITLE";
        private const string ChangeNameFailedNotAvailableKey = "CHANGE-NAME-FAILED-NOT-AVAILABLE";
        private const string ChangeNameFailedProfaneKey = "CHANGE-NAME-FAILED-PROFANE";

        // Create User Account with Email/Password
        private const string CreateAccountTitleKey = "CREATE-ACCOUNT-TITLE";
        private const string CreateAccountEmailNotAvailableKey = "CREATE-ACCOUNT-EMAIL-NOT-AVAILABLE";
        private const string CreateAccountUsernameNotAvailableKey = "CREATE-ACCOUNT-USERNAME-NOT-AVAILABLE";
        private const string CreateAccountInvalidEmailKey = "CREATE-ACCOUNT-EMAIL-INVALID";
        private const string CreateAccountInvalidPasswordKey = "CREATE-ACCOUNT-PASSWORD-INVALID";
        private const string CreateAccountInvalidUsernameKey = "CREATE-ACCOUNT-USERNAME-INVALID";

        // PlayFab Internal Error
        private const string PlayFabErrorTitleKey = "INTERNAL-PLAYFAB-ERROR-TITLE";
        private const string PlayFabErrorBodyKey = "INTERNAL-PLAYFAB-ERROR-BODY";

        // Login Failed Keys
        private const string LoginFailedTitleKey = "LOGIN-FAILED-TITLE";
        private const string AccountNotFoundKey = "LOGIN-ACCOUNT-NOT-FOUND";
        private const string InvalidEmailOrPasswordKey = "INVALID-EMAIL-OR-PASSWORD";

        // Contact Email Address Not Found
        private const string ContactEmailNotFoundTitleKey = "CONTACT-EMAIL-NOT-FOUND-TITLE";
        private const string ContactEmailNotFoundBodyKey = "CONTACT-EMAIL-NOT-FOUND-BODY";

        // Connecting To Store Localization Keys
        private const string ConnectingToStoreBodyKey = "CONNECTING-TO-STORE-BODY";

        // Insufficient Currency Localization Keys
        private const string InsufficientCurrencyTitleKey = "INSUFFICIENT-CURRENCY-TITLE";
        private const string InsufficientCurrencyBodyKey = "INSUFFICIENT-CURRENCY-BODY";

        // Purchasing Localization Keys
        private const string PurchaseFailedTitleKey = "PURCHASE-FAILED-TITLE";
        private const string PurchaseFailedDuplicateTransactionKey = "PURCHASE-FAILED-DUPLICATE-TRANSACTION";
        private const string PurchaseFailedExistingPurchasePendingKey = "PURCHASE-FAILED-PURCHASE-PENDING";
        private const string PurchaseFailedPaymentDeclinedKey = "PURCHASE-FAILED-PAYMENT-DECLINED";
        private const string PurchaseFailedProductUnavailableKey = "PURCHASE-FAILED-PRODUCT-UNAVAILABLE";
        private const string PurchaseFailedPurchasingUnavailableKey = "PURCHASE-FAILED-PURCHASING-UNVAILABLE";
        private const string PurchaseFailedSignatureInvalidKey = "PURCHASE-FAILED-SIGNATURE-INVALID";
        private const string PurchaseFailedUnknownKey = "PURCHASE-FAILED-UNKNOWN";
        private const string PurchaseFailedUnableToValidateReceiptKey = "PURCHASE-FAILED-UNABLE-TO-VALIDATE-RECEIPT";

        // Store Localization Keys
        private const string StoreFailedTitleKey = "STORE-FAILED-TITLE";
        private const string StoreFailedAppNotKnownKey = "STORE-FAILED-APP-NOT-KNOWN";
        private const string StoreFailedNoProductsAvailableKey = "STORE-FAILED-NO-PRODUCTS-AVAILABLE";
        private const string StoreFailedPurchasingUnavailableKey = "STORE-FAILED-PURCHASING-UNAVAILABLE";
        private const string StoreFailedConnectionTimeOutKey = "STORE-FAILED-CONNECTION-TIME-OUT";

        // TODO [bgish]: Need to move all of this to a localization table
        private static string Get(string localizationKey)
        {
            bool english = Localization.Localization.CurrentLanguage == Localization.Languages.English;

            switch (localizationKey)
            {
                // Exit App Keys
                case ExitAppTitleKey:
                    return english ? "Exit?" : "";
                case ExitAppBodyKey:
                    return english ? "Are you sure you want to exit?" : "";

                // Forgot Password Keys
                case ForgotPasswordTitleKey:
                    return english ? "Forgot Password" : "";
                case ForgotPasswordBodyKey:
                    return english ? "Are you sure you wish to send an account recovery email to \"{email}\"?" : "";

                // Internal PlayFab Error Keys
                case PlayFabErrorTitleKey:
                    return english ? "Internal Error" : "";
                case PlayFabErrorBodyKey:
                    return english ? "We have encountered an internal error.  Please try again later." : "";

                // Login Failure Keys
                case LoginFailedTitleKey:
                    return english? "Login Failed" : "";
                case AccountNotFoundKey:
                    return english? "The specified account could not be found." : "";
                case InvalidEmailOrPasswordKey:
                    return english? "The given email address or password is incorrect." : "";

                // Creating User Account
                case CreateAccountTitleKey:
                    return english ? "Create Account Failed" : "";
                case CreateAccountEmailNotAvailableKey:
                    return english ? "The specified email address is not available." : "";
                case CreateAccountUsernameNotAvailableKey:
                    return english ? "The specified username is not available." : "";
                case CreateAccountInvalidEmailKey:
                    return english ? "The specified email address is invalid." : "";
                case CreateAccountInvalidPasswordKey:
                    return english ? "The specified password is invalid.  Must be between 5 and 100 characters long." : "";
                case CreateAccountInvalidUsernameKey:
                    return english ? "The specified username is invalid.  Must be between 3 and 20 characters long." : "";

                // No Contact Email
                case ContactEmailNotFoundTitleKey:
                    return english ? "Email Not Found" : "";
                case ContactEmailNotFoundBodyKey:
                    return english ? "The specified email address could not be found." : "";

                // Changing Display Name
                case ChangeNameTitleKey:
                    return english ? "Display Name" : "Tên hiển thị";
                case ChangeNameBodyKey:
                    return english ? "Enter in your new display name." : "Nhập tên mới ...";
                case ChangeNameFailedTitleKey:
                    return english ? "Rename Failed" : "Lỗi khi đổi tên";
                case ChangeNameFailedNotAvailableKey:
                    return english ? "That name is currently not available." : "Tên mới không khả dụng";
                case ChangeNameFailedProfaneKey:
                    return english ? "That name contains profanity." : "Tên mới tồn tại tự thô tục.";

                // Connecting To Store
                case ConnectingToStoreBodyKey:
                    return english ? "Connecting to store..." : "Đang kết tối tới Cửa Hàng...";

                // Insufficient Currency
                case InsufficientCurrencyTitleKey:
                    return english ? "Not Enough Currency" : "Không đủ tiền tệ";
                case InsufficientCurrencyBodyKey:
                    return english ? "You'll need to buy more currency from the store.<br>Would you like to go there now?" : "Bạn cần thêm tiền từ Cửa Hàng? Đến Cửa Hàng ngay?";

                // Purchasing
                case PurchaseFailedTitleKey:
                    return english ? "Purchase failed" : "Giao dịch thất bại";
                case PurchaseFailedDuplicateTransactionKey:
                    return english ? "We've encountered a duplicate transaction." : "Phát hiện giao dịch bị trùng lặp";
                case PurchaseFailedExistingPurchasePendingKey:
                    return english ? "An existing purchase is already pending." : "Giao dịch đã tồn tại và đang được xử lý.";
                case PurchaseFailedPaymentDeclinedKey:
                    return english ? "The payment has been declined." : "Giao dịch bị từ chối";
                case PurchaseFailedProductUnavailableKey:
                    return english ? "The product is unavailable." : "Sản phẩm không khả dụng";
                case PurchaseFailedPurchasingUnavailableKey:
                    return english ? "Purchasing is currenctly unavailable." : "Tạm thời không thực hiện được giao dịch";
                case PurchaseFailedSignatureInvalidKey:
                    return english ? "Signature was invalid." : "Chữ kí không có hợp lệ";
                case PurchaseFailedUnknownKey:
                    return english ? "Sorry we've encountered an unknown error." : "Xảy ra lỗi không xác định.";
                case PurchaseFailedUnableToValidateReceiptKey:
                    return english ? "Unable to validate receipt." : "Không thể xác nhận đơn hàng.";

                // Store
                case StoreFailedTitleKey:
                    return english ? "Store Error" : "Lỗi Cửa hàng";
                case StoreFailedAppNotKnownKey:
                    return english ? "The store doesn't recognize this application." : "Ứng dụng không tồn tại trong Cửa hàng";
                case StoreFailedNoProductsAvailableKey:
                    return english ? "There are no valid products available for this application." : "Không tồn tại sản phẩm nào khả dụng cho ứng dụng này.";
                case StoreFailedPurchasingUnavailableKey:
                    return english ? "Unable to purchase.  Purchases have been turned off for this application." : "Không thể thực hiện thanh toán. Ứng dụng này đã bị tắt tính năng thanh toán.";
                case StoreFailedConnectionTimeOutKey:
                    return english ? "We timed out trying to connect to the store." : "Quá thời gian chờ phản hồi từ Cửa hàng";

                default:
                    return null;
            }
        }

        public static UnityTask<YesNoResult> ShowExitAppPrompt()
        {
            return MessageBox.Instance.ShowYesNo(Get(ExitAppTitleKey), Get(ExitAppBodyKey));
        }

        public static UnityTask<YesNoResult> ShowForgotPasswordPrompt(string email)
        {
            return MessageBox.Instance.ShowYesNo(Get(ForgotPasswordTitleKey), Get(ForgotPasswordBodyKey).Replace("{email}", email));
        }

        public static UnityTask<YesNoResult> ShowInsufficientCurrency()
        {
            return MessageBox.Instance.ShowYesNo(Get(InsufficientCurrencyTitleKey), Get(InsufficientCurrencyBodyKey));
        }

        public static void ShowConnectingToStoreSpinner()
        {
            SpinnerBox.Instance.UpdateBodyText(Get(ConnectingToStoreBodyKey));
        }

        public static UnityTask<StringInputResult> ShowChangeDisplayNameInputBox(string currentDisplayName)
        {
            var title = Get(ChangeNameTitleKey);
            var body = Get(ChangeNameBodyKey);
            return StringInputBox.Instance.Show(title, body, currentDisplayName, 15);
        }

        public static UnityTask<OkResult> HandleError(System.Exception exception)
        {
            UnityTask<OkResult> result = null;

            #if USING_UNITY_PURCHASING && !UNITY_XBOXONE
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
                // Major Errors
                case PlayFabErrorCode.InvalidPartnerResponse:
                case PlayFabErrorCode.InvalidTitleId:
                case PlayFabErrorCode.SmtpAddonNotEnabled:
                case PlayFabErrorCode.InvalidParams:
                    {
                        Debug.LogErrorFormat("Internal PlayFab Error: {0}", playfabException.Error.Error);
                        Debug.LogException(playfabException);

                        MessageBox.Instance.ShowOk(Get(PlayFabErrorTitleKey), Get(PlayFabErrorBodyKey));
                        break;
                    }

                // Display Name Changing Errors
                case PlayFabErrorCode.NameNotAvailable:
                    {
                        MessageBox.Instance.ShowOk(Get(ChangeNameFailedTitleKey), Get(ChangeNameFailedNotAvailableKey));
                        break;
                    }

                case PlayFabErrorCode.ProfaneDisplayName:
                    {
                        MessageBox.Instance.ShowOk(Get(ChangeNameFailedTitleKey), Get(ChangeNameFailedProfaneKey));
                        break;
                    }

                // Registering PlayFab User Errors
                case PlayFabErrorCode.EmailAddressNotAvailable:
                    {
                        MessageBox.Instance.ShowOk(Get(CreateAccountTitleKey), Get(CreateAccountEmailNotAvailableKey));
                        break;
                    }

                case PlayFabErrorCode.UsernameNotAvailable:
                    {
                        MessageBox.Instance.ShowOk(Get(CreateAccountTitleKey), Get(CreateAccountUsernameNotAvailableKey));
                        break;
                    }

                case PlayFabErrorCode.InvalidEmailAddress:
                    {
                        MessageBox.Instance.ShowOk(Get(CreateAccountTitleKey), Get(CreateAccountInvalidEmailKey));
                        break;
                    }

                case PlayFabErrorCode.InvalidPassword:
                    {
                        MessageBox.Instance.ShowOk(Get(CreateAccountTitleKey), Get(CreateAccountInvalidPasswordKey));
                        break;
                    }

                case PlayFabErrorCode.InvalidUsername:
                    {
                        MessageBox.Instance.ShowOk(Get(CreateAccountTitleKey), Get(CreateAccountInvalidUsernameKey));
                        break;
                    }

                // Login Errors
                case PlayFabErrorCode.AccountNotFound:
                    {
                        MessageBox.Instance.ShowOk(Get(LoginFailedTitleKey), Get(AccountNotFoundKey));
                        break;
                    }

                case PlayFabErrorCode.InvalidEmailOrPassword:
                    {
                        MessageBox.Instance.ShowOk(Get(LoginFailedTitleKey), Get(InvalidEmailOrPasswordKey));
                        break;
                    }

                // Email Address Not Found
                case PlayFabErrorCode.NoContactEmailAddressFound:
                    {
                        MessageBox.Instance.ShowOk(Get(ContactEmailNotFoundTitleKey), Get(ContactEmailNotFoundBodyKey));
                        break;
                    }

                // common receipt errors between iOS/Android
                case PlayFabErrorCode.ReceiptCancelled:
                case PlayFabErrorCode.InvalidBundleID:
                case PlayFabErrorCode.InvalidReceipt:
                case PlayFabErrorCode.NoMatchingCatalogItemForReceipt:
                case PlayFabErrorCode.ReceiptAlreadyUsed:
                case PlayFabErrorCode.SubscriptionAlreadyTaken:
                case PlayFabErrorCode.InvalidProductForSubscription:

                // ios receipt errors
                case PlayFabErrorCode.DownstreamServiceUnavailable:
                case PlayFabErrorCode.InvalidCurrencyCode:
                case PlayFabErrorCode.InvalidEnvironmentForReceipt:
                case PlayFabErrorCode.InvalidVirtualCurrency:
                case PlayFabErrorCode.ReceiptContainsMultipleInAppItems:
                case PlayFabErrorCode.ReceiptDoesNotContainInAppItems:

                // android receipt errors
                case PlayFabErrorCode.MissingTitleGoogleProperties:
                case PlayFabErrorCode.NoRealMoneyPriceForCatalogItem:
                    {
                        Debug.LogErrorFormat("Hit Error {0} while validating receipt", playfabException.Error.Error);
                        MessageBox.Instance.ShowOk(Get(PurchaseFailedTitleKey), Get(PurchaseFailedUnableToValidateReceiptKey));
                        break;
                    }

                    // TODO [bgish] - handle way more...
            }

            return null;
        }

        #if USING_UNITY_PURCHASING && !UNITY_XBOXONE

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
                    return MessageBox.Instance.ShowOk(Get(StoreFailedTitleKey), Get(StoreFailedAppNotKnownKey));

                case InitializationFailureReason.NoProductsAvailable:
                    Debug.LogErrorFormat("Error initializing purchasing \"{0}\"", purchasingInitializationException.FailureReason.ToString());
                    return MessageBox.Instance.ShowOk(Get(StoreFailedTitleKey), Get(StoreFailedNoProductsAvailableKey));

                case InitializationFailureReason.PurchasingUnavailable:
                    return MessageBox.Instance.ShowOk(Get(StoreFailedTitleKey), Get(StoreFailedPurchasingUnavailableKey));

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

            return MessageBox.Instance.ShowOk(Get(StoreFailedTitleKey), Get(StoreFailedConnectionTimeOutKey));
        }

        private static UnityTask<OkResult> HandlePurchasingError(PurchasingException purchasingException)
        {
            if (purchasingException == null)
            {
                return null;
            }

            string title = Get(PurchaseFailedTitleKey);

            switch (purchasingException.FailureReason)
            {
                case PurchaseFailureReason.DuplicateTransaction:
                    return MessageBox.Instance.ShowOk(title, Get(PurchaseFailedDuplicateTransactionKey));

                case PurchaseFailureReason.ExistingPurchasePending:
                    return MessageBox.Instance.ShowOk(title, Get(PurchaseFailedExistingPurchasePendingKey));

                case PurchaseFailureReason.PaymentDeclined:
                    return MessageBox.Instance.ShowOk(title, Get(PurchaseFailedPaymentDeclinedKey));

                case PurchaseFailureReason.ProductUnavailable:
                    return MessageBox.Instance.ShowOk(title, Get(PurchaseFailedProductUnavailableKey));

                case PurchaseFailureReason.PurchasingUnavailable:
                    return MessageBox.Instance.ShowOk(title, Get(PurchaseFailedPurchasingUnavailableKey));

                case PurchaseFailureReason.SignatureInvalid:
                    return MessageBox.Instance.ShowOk(title, Get(PurchaseFailedSignatureInvalidKey));

                case PurchaseFailureReason.Unknown:
                    return MessageBox.Instance.ShowOk(title, Get(PurchaseFailedUnknownKey));

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
