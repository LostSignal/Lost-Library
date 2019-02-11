//-----------------------------------------------------------------------
// <copyright file="AppVersionsTitleData.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    [UnityEngine.CreateAssetMenu(menuName = "TitleData/AppVersions")]
    public class AppVersionsTitleData : TitleData<AppVersionsData>
    {
    }
}

#endif
