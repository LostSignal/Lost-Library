//-----------------------------------------------------------------------// <copyright file="RealtimeMessage.cs" company="Lost Signal LLC">//     Copyright (c) Lost Signal LLC. All rights reserved.// </copyright>//-----------------------------------------------------------------------namespace Lost{
    public abstract class RealtimeMessage
    {
        public string Type => this.GetType().Name;
    }}