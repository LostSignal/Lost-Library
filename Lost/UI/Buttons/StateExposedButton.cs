//-----------------------------------------------------------------------
// <copyright file="StateExposedButton.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class StateExposedButton : UnityEngine.UI.Button
    {
        public bool IsPressedDown { get; private set; }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            this.IsPressedDown = true;
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            this.IsPressedDown = false;
        }

        public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            this.IsPressedDown = false;
        }
    }
}
