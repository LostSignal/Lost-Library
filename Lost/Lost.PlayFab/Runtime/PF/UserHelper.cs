//-----------------------------------------------------------------------
// <copyright file="UserHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System.Collections.Generic;
    using PlayFab.ClientModels;

    public class UserHelper
    {
        private UserAccountInfo userAccountInfo;

        public string PlayFabId { get; private set; }
        public long PlayFabNumericId { get; private set; }
        public string DisplayName { get; private set; }
        public string FacebookId { get; private set; }
        public bool IsFacebookLinked => string.IsNullOrEmpty(this.FacebookId) == false;
        public string AvatarUrl { get; private set; }

        public UserHelper()
        {
            PF.PlayfabEvents.OnLinkFacebookAccountResultEvent += PlayfabEvents_OnLinkFacebookAccountResultEvent1;
            PF.PlayfabEvents.OnUnlinkFacebookAccountResultEvent += PlayfabEvents_OnUnlinkFacebookAccountResultEvent1;
            PF.PlayfabEvents.OnLoginResultEvent += PlayfabEvents_OnLoginResultEvent;
            PF.PlayfabEvents.OnUpdateAvatarUrlResultEvent += PlayfabEvents_OnUpdateAvatarUrlResultEvent;
            PF.PlayfabEvents.OnUpdateUserTitleDisplayNameResultEvent += PlayfabEvents_OnUpdateUserTitleDisplayNameResultEvent;
            PF.PlayfabEvents.OnLinkFacebookAccountResultEvent += PlayfabEvents_OnLinkFacebookAccountResultEvent;
            PF.PlayfabEvents.OnUnlinkFacebookAccountResultEvent += PlayfabEvents_OnUnlinkFacebookAccountResultEvent;
        }

        public UnityTask<bool> ChangeDisplayName(string newDisplayName)
        {
            return UnityTask<bool>.Run(this.ChangeDisplayNameCoroutine(newDisplayName));
        }

        public UnityTask<bool> ChangeDisplayNameWithPopup()
        {
            return UnityTask<bool>.Run(this.ChangeDisplayNameWithPopupCoroutine());
        }

        private IEnumerator<bool> ChangeDisplayNameCoroutine(string newDisplayName)
        {
            var updateDisplayName = PF.Do(new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = newDisplayName,
            });

            while (updateDisplayName.IsDone == false)
            {
                yield return default(bool);
            }

            // Early out if we got an error
            if (updateDisplayName.HasError)
            {
                PlayFabMessages.HandleError(updateDisplayName.Exception);
                yield return false;
                yield break;
            }

            // TODO [bgish]: Fire off DisplayName changed event?
            // Updating the display name
            this.DisplayName = newDisplayName;

            yield return true;
        }

        private IEnumerator<bool> ChangeDisplayNameWithPopupCoroutine()
        {
            var stringInputBox = PlayFabMessages.ShowChangeDisplayNameInputBox(PF.User.DisplayName);

            while (stringInputBox.IsDone == false)
            {
                yield return default(bool);
            }

            if (stringInputBox.Value == StringInputResult.Cancel)
            {
                yield return false;
                yield break;
            }

            var updateDisplayName = PF.Do(new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = StringInputBox.Instance.Text,
            });

            while (updateDisplayName.IsDone == false)
            {
                yield return default(bool);
            }

            // Early out if we got an error
            if (updateDisplayName.HasError)
            {
                PlayFabMessages.HandleError(updateDisplayName.Exception);
                yield return false;
                yield break;
            }

            // TODO [bgish]: Fire off DisplayName changed event?
            // Updating the display name
            this.DisplayName = StringInputBox.Instance.Text;

            yield return true;
        }

        private void PlayfabEvents_OnLoginResultEvent(LoginResult result)
        {
            this.userAccountInfo = result?.InfoResultPayload?.AccountInfo;

            this.PlayFabId = this.userAccountInfo?.PlayFabId;
            this.DisplayName = this.userAccountInfo?.TitleInfo?.DisplayName;
            this.FacebookId = this.userAccountInfo?.FacebookInfo?.FacebookId;
            this.AvatarUrl = this.userAccountInfo?.TitleInfo?.AvatarUrl;

            this.PlayFabNumericId = PF.ConvertPlayFabIdToLong(this.PlayFabId);

            // TODO [bgish]: Fire a AvatarUrlChanged event
            // TODO [bgish]: Fire a DisplayNameChanged event
            // TODO [bgish]: Fire a FacebookChanged event
        }
        
        private void PlayfabEvents_OnLinkFacebookAccountResultEvent1(LinkFacebookAccountResult result)
        {
            #if USING_FACEBOOK_SDK
            this.FacebookId = PF.Login.FacebookLoginResult?.AccessToken?.UserId;
            #endif
        }

        private void PlayfabEvents_OnUnlinkFacebookAccountResultEvent1(UnlinkFacebookAccountResult result)
        {
            this.FacebookId = null;
        }

        private void PlayfabEvents_OnUpdateAvatarUrlResultEvent(EmptyResponse result)
        {
            // TODO [bgish]: Update AvatarUrl and fire a AvatarUrlChanged event
        }

        private void PlayfabEvents_OnUpdateUserTitleDisplayNameResultEvent(UpdateUserTitleDisplayNameResult result)
        {
            // TODO [bgish]: Update DisplayName and fire a DisplayNameChanged event
        }

        private void PlayfabEvents_OnLinkFacebookAccountResultEvent(LinkFacebookAccountResult result)
        {
            // TODO [bgish]: Update FacebookId and fire a FacebookChanged event
        }

        private void PlayfabEvents_OnUnlinkFacebookAccountResultEvent(UnlinkFacebookAccountResult result)
        {
            // TODO [bgish]: Update FacebookId and fire a FacebookChanged event
        }
    }
}

#endif
