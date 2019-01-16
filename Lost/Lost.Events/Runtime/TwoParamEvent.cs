//-----------------------------------------------------------------------
// <copyright file="TwoParamEvent.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;

    public class TwoParamEvent<T, U>
    {
        private List<Action<T, U>> actions = new List<Action<T, U>>();

        public void Subscribe(Action<T, U> action)
        {
            this.actions.AddIfNotNullAndUnique(action);
        }

        public void Unsubscribe(Action<T, U> action)
        {
            this.actions.Remove(action);
        }

        public void Raise(T eventObject1, U eventObject2)
        {
            foreach (var action in this.actions)
            {
                action.Invoke(eventObject1, eventObject2);
            }
        }
    }
}
