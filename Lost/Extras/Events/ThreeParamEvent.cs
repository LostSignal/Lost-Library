//-----------------------------------------------------------------------
// <copyright file="ThreeParamEvent.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    public class ThreeParamEvent<T, U, V>
    {
        private List<Action<T, U, V>> actions = new List<Action<T, U, V>>();

        public void Subscribe(Action<T, U, V> action)
        {
            this.actions.AddIfNotNullAndUnique(action);
        }

        public void Unsubscribe(Action<T, U, V> action)
        {
            this.actions.Remove(action);
        }

        public void Raise(T eventObject1, U eventObject2, V eventObject3)
        {
            foreach (var action in this.actions)
            {
                action.Invoke(eventObject1, eventObject2, eventObject3);
            }
        }
    }
}
