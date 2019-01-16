//-----------------------------------------------------------------------
// <copyright file="CloudScriptHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using PlayFab.ClientModels;

    public class CloudScriptHelper
    {
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

        public T GetCloudScrtipResult<T>(UnityTask<ExecuteCloudScriptResult> result)
        {
            string functionResult = result.Value.FunctionResult != null ? result.Value.FunctionResult.ToString() : null;
            return PF.SerializerPlugin.DeserializeObject<T>(functionResult);
        }
    }
}

#endif
