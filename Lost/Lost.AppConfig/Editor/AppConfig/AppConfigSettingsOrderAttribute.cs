//-----------------------------------------------------------------------
// <copyright file="AppConfigSettingsOrderAttribute.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;

    public class AppConfigSettingsOrderAttribute : Attribute
    {
        public int Order { get; private set; }

        public AppConfigSettingsOrderAttribute(int order)
        {
            this.Order = order;
        }
    }
}
