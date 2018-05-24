//-----------------------------------------------------------------------
// <copyright file="IPoolable.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public interface IPoolable
    {
        void Recycle();
    }
}
