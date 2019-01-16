//-----------------------------------------------------------------------
// <copyright file="GoToScene.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class GoToScene : MonoBehaviour
    {
        public void GoToSceneName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void GoToSceneIndex(int sceneBuildIndex)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }
}
