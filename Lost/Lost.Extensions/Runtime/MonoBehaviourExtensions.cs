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
        public static void AssertGetComponent<T>(this MonoBehaviour monoBehaviour, ref T memberVariable)
            where T : Component
        {
            if (memberVariable == null)
            {
                memberVariable = monoBehaviour.GetComponent<T>();

                if (memberVariable == null)
                {
                    Debug.LogErrorFormat(monoBehaviour.gameObject, "{0} {1} couldn't find {2} component", monoBehaviour.GetType().Name, GetFullName(monoBehaviour), typeof(T).Name);
                }
                else if (Application.isPlaying)
                {
                    Debug.LogWarningFormat(monoBehaviour.gameObject, "Unneseccasy GetComponent<{0}> call on GameObject {1}.  Should prepopulate this in editor and not at runtime.", typeof(T).Name, GetFullName(monoBehaviour));
                }
                else
                {
                    #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(monoBehaviour);
                    #endif
                }
            }
        }

        public static void AssertGetComponentInParent<T>(this MonoBehaviour monoBehaviour, ref T memberVariable)
            where T : Component
        {
            if (memberVariable == null)
            {
                memberVariable = monoBehaviour.GetComponentInParent<T>();

                if (memberVariable == null)
                {
                    Debug.LogErrorFormat(monoBehaviour.gameObject, "{0} {1} couldn't find {2} component in parent.", monoBehaviour.GetType().Name, GetFullName(monoBehaviour), typeof(T).Name);
                }
                else if (Application.isPlaying)
                {
                    Debug.LogWarningFormat(monoBehaviour.gameObject, "Unneseccasy GetComponentInParent<{0}> call on GameObject {1}.  Should prepopulate this in editor and not at runtime.", typeof(T).Name, GetFullName(monoBehaviour));
                }
                else
                {
                    #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(monoBehaviour);
                    #endif
                }
            }
        }

        public static void AssertNotNull(this MonoBehaviour monoBehaviour, object obj, string nameOfObject = null)
        {
            if (obj == null)
            {
                Debug.LogErrorFormat(monoBehaviour.gameObject, "{0} {1} has null object {2}", monoBehaviour.GetType().Name, GetFullName(monoBehaviour), nameOfObject);
            }
        }

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
            yield return WaitForUtil.EndOfFrame;
            action?.Invoke();
        }

        private static IEnumerator DelayInSecondsCoroutine(float delayInSeconds, Action action)
        {
            yield return WaitForUtil.Seconds(delayInSeconds);
            action?.Invoke();
        }

        private static IEnumerator DelayExecuteRealtimeCoroutine(float delayInRealtimeSeconds, Action action)
        {
            yield return WaitForUtil.RealtimeSeconds(delayInRealtimeSeconds);
            action?.Invoke();
        }

        private static string GetFullName(MonoBehaviour monoBehaviour)
        {
            return GetFullName(monoBehaviour.gameObject);
        }

        private static string GetFullName(GameObject gameObject)
        {
            if (gameObject.transform.parent == null)
            {
                return string.Empty;
            }
            else
            {
                string parentName = GetFullName(gameObject.transform.parent.gameObject);

                return parentName == string.Empty ? gameObject.name : parentName + "/" + gameObject.name;
            }
        }
    }
}
