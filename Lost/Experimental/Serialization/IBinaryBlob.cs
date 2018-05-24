//-----------------------------------------------------------------------
// <copyright file="IBinaryBlob.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;

    public interface IBinaryBlob
    {
        void Serialize(BinaryWriter writer);

        void Deserialize(BinaryReader reader);
    }
}
