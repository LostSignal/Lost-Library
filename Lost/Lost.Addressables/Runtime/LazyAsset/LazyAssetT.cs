//-----------------------------------------------------------------------
// <copyright file="LazyAssetT.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER || USING_UNITY_ADDRESSABLES

namespace Lost
{
#if UNITY_2018_3_OR_NEWER

    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    [Serializable]
    public class LazyAsset<T> : LazyAsset, ILazyAsset, IValidate where T : UnityEngine.Object
    {
        private AsyncOperationHandle operation;
        private UnityTask<T> cachedTask;

        public override Type Type
        {
            get { return typeof(T); }
        }

        public LazyAsset()
        {
        }

        public LazyAsset(string guid) : base(guid)
        {
        }

        public UnityTask<T> Load()
        {
            #if UNITY_EDITOR
            if (typeof(T).IsSubclassOf(typeof(Component)) || typeof(T) == typeof(GameObject))
            {
                Debug.LogWarningFormat("You are loading LastAsset<{0}> as if it were a resource, you should instead use Instantiate instead of Load.", typeof(T).Name);
            }
            #endif

            if (this.cachedTask != null)
            {
                return this.cachedTask;
            }
            else
            {
                return UnityTask<T>.Run(Coroutine());
            }

            IEnumerator<T> Coroutine()
            {
                if (typeof(T) == typeof(Sprite))
                {
                    this.operation = Addressables.LoadAssetAsync<Sprite>(this.RuntimeKey);
                }
                else
                {
                    this.operation = Addressables.LoadAssetAsync<UnityEngine.Object>(this.RuntimeKey);
                }

                while (this.operation.IsDone == false && this.operation.Status != AsyncOperationStatus.Failed)
                {
                    yield return default(T);
                }

                if (this.operation.Status == AsyncOperationStatus.Failed)
                {
                    Debug.LogErrorFormat("Unable to successfully load asset {0} of type {1}", this.AssetGuid, typeof(T).Name);
                    yield return default(T);
                    yield break;
                }

                T value;

                if (typeof(T).IsSubclassOf(typeof(Component)))
                {
                    var gameObject = operation.Result as GameObject;

                    if (gameObject == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} is not of type GameObject, so can't get Component {1} from it.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }

                    value = gameObject?.GetComponent<T>();

                    if (value == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} does not have Component {1} on it.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }
                }
                else
                {
                    value = operation.Result as T;

                    if (value == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} is not of type {1}.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }
                }

                this.cachedTask = UnityTask<T>.Empty(value);
                yield return value;
            }
        }

        public UnityTask<T> Instantiate(Transform parent = null, bool reset = true)
        {
            #if UNITY_EDITOR
            if (typeof(T).IsSubclassOf(typeof(Component)) == false && typeof(T) != typeof(GameObject))
            {
                Debug.LogWarningFormat("You are Instantiating LastAsset<{0}> as if it were a GameObject, you should instead use Load instead of Instantiate.", typeof(T).Name);
            }
            #endif

            return UnityTask<T>.Run(Coroutine());

            IEnumerator<T> Coroutine()
            {
                var instantiateOperation = Addressables.InstantiateAsync(this.RuntimeKey, parent);

                while (instantiateOperation.IsDone == false && instantiateOperation.Status != AsyncOperationStatus.Failed)
                {
                    yield return default(T);
                }

                if (instantiateOperation.Status == AsyncOperationStatus.Failed)
                {
                    Debug.LogErrorFormat("Unable to successfully instantiate asset {0} of type {1}", this.AssetGuid, typeof(T).Name);
                    yield return default(T);
                    yield break;
                }

                var gameObject = instantiateOperation.Result;

                if (gameObject != null && reset)
                {
                    gameObject.transform.Reset();
                }

                if (typeof(T) == typeof(GameObject))
                {
                    yield return gameObject as T;
                }
                else if (typeof(T).IsSubclassOf(typeof(Component)))
                {
                    if (gameObject == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} is not of type GameObject, so can't get Component {1} from it.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }

                    var component = gameObject?.GetComponent<T>();

                    if (component == null)
                    {
                        Debug.LogErrorFormat("LazyAsset {0} does not have Component {1} on it.", this.AssetGuid, typeof(T).Name);
                        yield break;
                    }

                    yield return component;
                }
                else
                {
                    Debug.LogError("LazyAssetT hit unknown if/else situtation.");
                }
            }
        }

        public void Release()
        {
            if (this.operation.IsValid() == false)
            {
                Debug.LogWarning("Cannot release a null or unloaded asset.");
                return;
            }

            Addressables.Release(this.operation);
            this.operation = default(AsyncOperationHandle);
            this.cachedTask = null;
        }

        void IValidate.Validate()
        {
#if UNITY_EDITOR
            // TODO [bgish]: Verify Guid is valid and the object can be cast as T
            throw new NotImplementedException();
#endif
        }
    }

#else

        [System.Serializable]
    public class LazyAsset<T> : LazyAsset, ILazyAsset
    {
    }

#endif
}

#endif
