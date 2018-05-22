//-----------------------------------------------------------------------
// <copyright file="ApplyMeshCollidersHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    //// [AddComponentMenu("Lost/Editor Helpers/Apply Mesh Colliders Helper")]
    public class ApplyMeshCollidersHelper : MonoBehaviour
    {
        [MenuItem("CONTEXT/Transform/Select all child meshes with no MeshCollider")]
        public static void SelectAllChildMeshesWithNoMeshCollider(MenuCommand command)
        {
            List<GameObject> selectedObjects = new List<GameObject>();

            foreach (MeshRenderer meshRenderer in Selection.activeGameObject.GetComponentsInChildren<MeshRenderer>())
            {
                if (meshRenderer.gameObject.GetComponent<MeshCollider>() == null)
                {
                    selectedObjects.Add(meshRenderer.gameObject);
                }
            }

            Selection.objects = selectedObjects.ToArray();
        }

        //// [MenuItem("Testing123/Testing456")]
        //// public static void Something(MenuCommand command)
        //// {
        ////     Debug.Log("Something");
        //// }
        ////
        //// [ContextMenu("Another Context Menu")]
        //// public void AnotherContextMenu()
        //// {
        ////
        //// }
    }
}
