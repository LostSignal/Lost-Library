//-----------------------------------------------------------------------
// <copyright file="CharacterHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System.Collections.Generic;
    using PlayFab;
    using PlayFab.ClientModels;

    public class CharacterHelper
    {
        public List<CharacterResult> Characters { get; private set; } = new List<CharacterResult>();

        public CharacterHelper()
        {
            PF.PlayfabEvents.OnLoginResultEvent += this.PlayfabEvents_OnLoginResultEvent;
            PF.PlayfabEvents.OnGetAllUsersCharactersResultEvent += PlayfabEvents_OnGetAllUsersCharactersResultEvent;
            PF.PlayfabEvents.OnGetPlayerCombinedInfoResultEvent += PlayfabEvents_OnGetPlayerCombinedInfoResultEvent;
        }

        public UnityTask<ListUsersCharactersResult> GetUserCharacters()
        {
            return PF.Do<ListUsersCharactersRequest, ListUsersCharactersResult>(new ListUsersCharactersRequest(), PlayFabClientAPI.GetAllUsersCharacters);
        }

        public UnityTask<GetCharacterDataResult> GetCharacterData(string playfabId, string characterId, List<string> keys)
        {
            return PF.Do<GetCharacterDataRequest, GetCharacterDataResult>(new GetCharacterDataRequest
            {
                PlayFabId = playfabId,
                CharacterId = characterId,
                Keys = keys,
            },
            PlayFabClientAPI.GetCharacterData);
        }

        public UnityTask<GetCharacterDataResult> GetCharacterData(string characterId, List<string> keys)
        {
            return PF.Do<GetCharacterDataRequest, GetCharacterDataResult>(new GetCharacterDataRequest
            {
                CharacterId = characterId,
                Keys = keys,
            },
            PlayFabClientAPI.GetCharacterData);
        }

        public UnityTask<UpdateCharacterDataResult> UpdateCharacterData(string characterId, Dictionary<string, string> data)
        {
            return PF.Do<UpdateCharacterDataRequest, UpdateCharacterDataResult>(new UpdateCharacterDataRequest
            {
                CharacterId = characterId,
                Data = data,
            },
            PlayFabClientAPI.UpdateCharacterData);
        }

        public UnityTask<ExecuteCloudScriptResult> Execute(string functionName, object functionParameters = null)
        {
            return PF.Do(new ExecuteCloudScriptRequest
            {
                FunctionName = functionName,
                RevisionSelection = CloudScriptRevisionOption.Specific,
                SpecificRevision = PF.CloudScriptRevision,
                GeneratePlayStreamEvent = true,
                FunctionParameter = functionParameters,
            });
        }

        private void PlayfabEvents_OnLoginResultEvent(LoginResult result)
        {
            this.UpdateCharacters(result.InfoResultPayload?.CharacterList);
        }

        private void PlayfabEvents_OnGetAllUsersCharactersResultEvent(ListUsersCharactersResult result)
        {
            this.UpdateCharacters(result.Characters);
        }

        private void PlayfabEvents_OnGetPlayerCombinedInfoResultEvent(GetPlayerCombinedInfoResult result)
        {
            this.UpdateCharacters(result.InfoResultPayload?.CharacterList);
        }

        private void UpdateCharacters(List<CharacterResult> characters)
        {
            if (characters == null)
            {
                return;
            }

            this.Characters.Clear();
            this.Characters.AddRange(characters);
        }
    }
}

#endif
