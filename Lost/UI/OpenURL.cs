//-----------------------------------------------------------------------
// <copyright file="OpenURL.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    /// <summary>
    /// Add this class to any UI Button if you want to be able to open an url with a button click.
    /// </summary>
    public class OpenURL : MonoBehaviour
    {
        public void Open(string url)
        {
            Application.OpenURL(url);
        }
    }
}
