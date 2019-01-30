//-----------------------------------------------------------------------
// <copyright file="GuidManager.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;

    public class GuidManager : SingletonGameObject<GuidManager>
    {
        private Dictionary<string, GuidComponent> guids = new Dictionary<string, GuidComponent>();

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
