//-----------------------------------------------------------------------
// <copyright file="GuidManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class GuidManager : SingletonGameObject<GuidManager>
    {
        private Dictionary<string, GuidComponent> guids = new Dictionary<string, GuidComponent>();

        public GameObject GetGameObject(string guid)
        {
            if (this.guids.TryGetValue(guid, out GuidComponent component))
            {
                return component.gameObject;
            }
            else
            {
                Debug.LogErrorFormat("Tried getting GameObject with {0} guid, but it is not registered yet.", guid);
            }

            return null;
        }

        public bool DoesGuidExist(GuidComponent guidComponent)
        {
            if (guids.TryGetValue(guidComponent.Guid, out GuidComponent cachedComponent))
            {
                if (cachedComponent.GetInstanceID() != guidComponent.GetInstanceID())
                {
                    return true;
                }
            }

            return false;
        }

        public void RegisterGuid(GuidComponent guidComponent)
        {

        }

        public void UnregisterGuid(GuidComponent guidComponent)
        {

        }
    }
}
