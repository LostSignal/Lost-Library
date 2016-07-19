//-----------------------------------------------------------------------
// <copyright file="MonoBehaviourExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    
    public static class MonoBehaviourExtensions
    {
        public static T GetOrAddComponent<T>(this MonoBehaviour behaviour) where T : Component
        {
            return behaviour.GetComponent<T>() ?? behaviour.gameObject.AddComponent<T>();
        }
        
        public static void DrawGizmoCube(this MonoBehaviour lhs, Color color, float width, float height, Vector2 offset)
        {
            #if UNITY_EDITOR
            Gizmos.color = color;
            Vector2 parentScale = new Vector2(lhs.gameObject.transform.localToWorldMatrix[0, 0], lhs.gameObject.transform.localToWorldMatrix[1, 1]);
            Vector2 localUnits = new Vector2(width, height);
            Vector3 worldUnits = new Vector3(localUnits.x * parentScale.x, localUnits.y * parentScale.y, 0);

            Gizmos.DrawWireCube(lhs.gameObject.transform.position + new Vector3(offset.x, offset.y, 0), worldUnits);
            #endif
        }
    }
}
