//-----------------------------------------------------------------------
// <copyright file="MonoBehaviourExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections;
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

        public static void ExecuteAtEndOfFrame(this MonoBehaviour lhs, Action action)
        {
            lhs.StartCoroutine(DelayTillEndOfFrameCoroutine(action));
        }

        public static void ExecuteDelayed(this MonoBehaviour lhs, float delayInSeconds, Action action)
        {
            lhs.StartCoroutine(DelayInSecondsCoroutine(delayInSeconds, action));
        }

        public static void ExecuteDelayedRealtime(this MonoBehaviour lhs, float delayInRealtimeSeconds, Action action)
        {
            lhs.StartCoroutine(DelayExecuteRealtimeCoroutine(delayInRealtimeSeconds, action));
        }

        private static IEnumerator DelayTillEndOfFrameCoroutine(Action action)
        {
            yield return new WaitForEndOfFrame();
            action.InvokeIfNotNull();
        }
        
        private static IEnumerator DelayInSecondsCoroutine(float delayInSeconds, Action action)
        {
            yield return new WaitForSeconds(delayInSeconds);
            action.InvokeIfNotNull();
        }

        private static IEnumerator DelayExecuteRealtimeCoroutine(float delayInRealtimeSeconds, Action action)
        {
            yield return new WaitForSecondsRealtime(delayInRealtimeSeconds);
            action.InvokeIfNotNull();
        }
    }
}
