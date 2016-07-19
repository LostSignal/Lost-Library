//-----------------------------------------------------------------------
// <copyright file="NoSubmitButton.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine.EventSystems;

    public class NoSubmitButton : UnityEngine.UI.Button
    {
        public override void OnSubmit(BaseEventData eventData)
        {
        }
    }
}
