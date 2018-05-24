//-----------------------------------------------------------------------
// <copyright file="AssetBundleLoadOperation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;

    public abstract class AssetBundleLoadOperation : IEnumerator
    {
        public object Current
        {
            get { return null; }
        }

        public bool MoveNext()
        {
            return this.IsDone() == false;
        }

        public void Reset()
        {
        }

        public abstract bool Update();

        public abstract bool IsDone();
    }
}
