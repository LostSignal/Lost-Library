//-----------------------------------------------------------------------
// <copyright file="SignUpDialog.cs" company="Lost Signal LLC">
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

    public class SignUpDialog : DialogLogic
    {
        #pragma warning disable 0649
        [SerializeField] private LostButton closeButton;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_InputField confirmPasswordInputField;
        [SerializeField] private Toggle autoLoginToggle;
        [SerializeField] private LostButton alreadRegistedButton;
        [SerializeField] private LostButton signUpButton;
        #pragma warning restore 0649

        private GetPlayerCombinedInfoRequestParams infoRequestParams;
        private bool isLeaveGameCoroutineRunning;
        private bool isSignUpCoroutineRunning;

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
            this.AssertNotNull(this.confirmPasswordInputField, nameof(this.confirmPasswordInputField));
            this.AssertNotNull(this.autoLoginToggle, nameof(this.autoLoginToggle));
            this.AssertNotNull(this.alreadRegistedButton, nameof(this.alreadRegistedButton));
            this.AssertNotNull(this.signUpButton, nameof(this.signUpButton));
        }

        protected override void Awake()
        {
            base.Awake();
            this.OnValidate();

            this.closeButton.onClick.AddListener(this.OnBackButtonPressed);

            this.emailInputField.onValueChanged.AddListener(this.UpdateSignUpButton);
            this.passwordInputField.onValueChanged.AddListener(this.UpdateSignUpButton);
            this.confirmPasswordInputField.onValueChanged.AddListener(this.UpdateSignUpButton);

            this.autoLoginToggle.isOn = PF.Login.HasEverLoggedIn == false || PF.Login.AutoLoginWithDeviceId;

            this.alreadRegistedButton.onClick.AddListener(this.ShowLogInDialog);

            this.signUpButton.interactable = false;
            this.signUpButton.onClick.AddListener(this.SignIn);
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

        private void UpdateSignUpButton(string newValue)
        {
            var email = this.emailInputField.text;
            var password = this.passwordInputField.text;
            var confirmPassword = this.confirmPasswordInputField.text;

            this.signUpButton.interactable =
                email.IsNullOrWhitespace() == false &&
                password.IsNullOrWhitespace() == false &&
                password.Length >= 6 &&
                password.Length <= 100 &&
                password == confirmPassword;
        }

        private void SignIn()
        {
            CoroutineRunner.Instance.StartCoroutine(SignInCoroutine());

            IEnumerator SignInCoroutine()
            {
                if (this.isSignUpCoroutineRunning)
                {
                    yield break;
                }

                this.isSignUpCoroutineRunning = true;

                var login = PF.Login.LoginWithEmail(true, this.emailInputField.text, this.passwordInputField.text);

                yield return login;

                if (login.HasError)
                {
                    yield return PlayFabMessages.HandleError(login.Exception);
                }
                else
                {
                    PF.Login.HasEverLoggedIn = true;
                    PF.Login.LastLoginEmail = this.emailInputField.text;
                    PF.Login.AutoLoginWithDeviceId = this.autoLoginToggle.isOn;

                    if (PF.Login.AutoLoginWithDeviceId)
                    {
                        PF.Login.LinkDeviceId(PF.Login.DeviceId);
                    }

                    this.Dialog.Hide();
                }

                this.isSignUpCoroutineRunning = false;
            }
        }

        private void ShowLogInDialog()
        {
            this.Dialog.Hide();

            this.ExecuteDelayed(0.25f, () =>
            {
                DialogManager.GetDialog<LogInDialog>().Show(this.infoRequestParams, this.emailInputField.text);
            });
        }
    }
}

#endif
