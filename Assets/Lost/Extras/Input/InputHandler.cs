//-----------------------------------------------------------------------
// <copyright file="InputHandler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    
    public interface InputHandler
    {
        void HandleInputs(List<Input> touch, Input mouse, Input pen);
    }
}
