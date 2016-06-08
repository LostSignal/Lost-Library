//-----------------------------------------------------------------------
// <copyright file="PrintGlobalPosition.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;
    
    public class PrintGlobalPosition : MonoBehaviour
    {
        [MenuItem("CONTEXT/Transform/Print Global Position")]
        public static void SelectAllChildMeshesWithNoMeshCollider(MenuCommand command)
        {
            Vector3 pos = Selection.activeGameObject.transform.position;
            Logger.LogInfo(Selection.activeGameObject, "{0}'s world position is {1}, {2}, {3}", Selection.activeGameObject.name, pos.x, pos.y, pos.z);
        }
    }
}
