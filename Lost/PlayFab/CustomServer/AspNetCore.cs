//-----------------------------------------------------------------------
// <copyright file="AspNetCore.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY

namespace Microsoft.AspNetCore.Mvc
{
    using System;

    public class Controller
    {
    }

    public class RouteAttribute : System.Attribute
    {
        public RouteAttribute(string template)
        {
        }
    }

    public class HttpGetAttribute : System.Attribute
    {
        public HttpGetAttribute()
        {
        }

        public HttpGetAttribute(string template)
        {
        }
    }

    public class HttpPostAttribute : System.Attribute
    {
        public HttpPostAttribute()
        {
        }

        public HttpPostAttribute(string template)
        {
        }
    }

    public class HttpPutAttribute : System.Attribute
    {
        public HttpPutAttribute()
        {
        }

        public HttpPutAttribute(string template)
        {
        }
    }

    public class HttpDeleteAttribute : System.Attribute
    {
        public HttpDeleteAttribute()
        {
        }

        public HttpDeleteAttribute(string template)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromBodyAttribute : Attribute
    {
    }
}

#endif
