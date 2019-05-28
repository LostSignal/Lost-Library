//-----------------------------------------------------------------------
// <copyright file="LogInDialog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System.Collections;
    using PlayFab.ClientModels;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class LogInDialog : DialogLogic
    {
        #pragma warning disable 0649
        [SerializeField] private LostButton closeButton;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private Toggle autoLoginToggle;
        [SerializeField] private LostButton logInButton;
        [SerializeField] private LostButton forgotPasswordButton;
        [SerializeField] private LostButton createNewAccountButton;
        [SerializeField] private string forgotEmailTemplateId;
        #pragma warning restore 0649

        private GetPlayerCombinedInfoRequestParams infoRequestParams;
        private bool isLeaveGameCoroutineRunning;
        private bool isLoginCoroutineRunning;
        private bool isForgotPasswordCoroutineRunning;

        public void Show(GetPlayerCombinedInfoRequestParams infoRequestParams = null, string email = "")
        {
            this.infoRequestParams = infoRequestParams;
            this.emailInputField.text = email;
            this.Dialog.Show();
        }

        private void OnValidate()
        {
            this.AssertNotNull(this.closeButton, nameof(this.closeButton));
            this.AssertNotNull(this.emailInputField, nameof(this.emailInputField));
            this.AssertNotNull(this.passwordInputField, nameof(this.passwordInputField));
            this.AssertNotNull(this.autoLoginToggle, nameof(this.autoLoginToggle));
            this.AssertNotNull(this.logInButton, nameof(this.logInButton));
            this.AssertNotNull(this.forgotPasswordButton, nameof(this.forgotPasswordButton));
            this.AssertNotNull(this.createNewAccountButton, nameof(this.createNewAccountButton));
        }

        protected override void Awake()
        {
            base.Awake();
            this.OnValidate();

            this.closeButton.onClick.AddListener(this.OnBackButtonPressed);

            this.emailInputField.text = PF.Login.LastLoginEmail;
            this.emailInputField.onValueChanged.AddListener(this.UpdateLogInButton);
            this.passwordInputField.onValueChanged.AddListener(this.UpdateLogInButton);

            this.autoLoginToggle.isOn = PF.Login.HasEverLoggedIn == false || PF.Login.AutoLoginWithDeviceId;

            this.forgotPasswordButton.onClick.AddListener(this.ForgotPassword);
            this.createNewAccountButton.onClick.AddListener(this.ShowSignUpDialog);

            this.logInButton.interactable = false;
            this.logInButton.onClick.AddListener(this.LogIn);
        }

        protected override void OnBackButtonPressed()
        {
            CoroutineRunner.Instance.StartCoroutine(LeaveGameCoroutine());

            IEnumerator LeaveGameCoroutine()
            {
                if (this.isLeaveGameCoroutineRunning)
                {
                    yield break;
                }

                this.isLeaveGameCoroutineRunning = true;

                this.Dialog.Hide();

                yield return WaitForUtil.Seconds(0.25f);

                var leaveGamePrompt = PlayFabMessages.ShowExitAppPrompt();

                yield return leaveGamePrompt;

                if (leaveGamePrompt.Value == YesNoResult.Yes)
                {
                    Platform.QuitApplication();
                    yield break;
                }

                this.Dialog.Show();

                this.isLeaveGameCoroutineRunning = false;
            }
        }

        private void UpdateLogInButton(string newValue)
        {
            var email = this.emailInputField.text;
            var password = this.passwordInputField.text;

            this.logInButton.interactable =
                email.IsNullOrWhitespace() == false &&
                password.IsNullOrWhitespace() == false &&
                password.Length >= 6 &&
                password.Length <= 100;
        }

        private void ShowSignUpDialog()
        {
            this.Dialog.Hide();

            this.ExecuteDelayed(0.25f, () =>
            {
                DialogManager.GetDialog<SignUpDialog>().Show(this.infoRequestParams, this.emailInputField.text);
            });
        }

        private void LogIn()
        {
            CoroutineRunner.Instance.StartCoroutine(LogInCoroutine());

            IEnumerator LogInCoroutine()
            {
                if (this.isLoginCoroutineRunning)
                {
                    yield break;
                }

                this.isLoginCoroutineRunning = true;

                var login = PF.Login.LoginWithEmail(false, this.emailInputField.text, this.passwordInputField.text, this.infoRequestParams);

                yield return login;

                if (login.HasError)
                {
                    yield return PlayFabMessages.HandleError(login.Exception);
                }
                else
                {
                    PF.Login.LastLoginEmail = this.emailInputField.text;
                    PF.Login.AutoLoginWithDeviceId = this.autoLoginToggle.isOn;

                    if (PF.Login.AutoLoginWithDeviceId)
                    {
                        PF.Login.LinkDeviceId(PF.Login.DeviceId);
                    }

                    this.Dialog.Hide();
                }

                this.isLoginCoroutineRunning = false;
            }
        }

        private void ForgotPassword()
        {
            CoroutineRunner.Instance.StartCoroutine(ForgotPasswordCoroutine());

            IEnumerator ForgotPasswordCoroutine()
            {
                if (this.isForgotPasswordCoroutineRunning)
                {
                    yield break;
                }

                this.isForgotPasswordCoroutineRunning = true;

                this.Dialog.Hide();
                yield return WaitForUtil.Seconds(.25f);

                var forgot = PlayFabMessages.ShowForgotPasswordPrompt(this.emailInputField.text);

                if (forgot.Value == YesNoResult.Yes)
                {
                    var accountRecovery = PF.Login.SendAccountRecoveryEmail(this.emailInputField.text, this.forgotEmailTemplateId);

                    yield return accountRecovery;

                    if (accountRecovery.HasError)
                    {
                        yield return PlayFabMessages.HandleError(accountRecovery.Exception);
                    }
                }

                this.Dialog.Show();

                this.isForgotPasswordCoroutineRunning = false;
            }
        }
    }
}

#endif
