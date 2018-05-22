//-----------------------------------------------------------------------
// <copyright file="TimingLogger.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;

    public class TimingLogger : IDisposable
    {
        private DateTime startTime;
        private DateTime endTime;
        private string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lost.TimingLogger"/> class.
        /// </summary>
        /// <param name='message'>The message to print to the logger (along with timing info).</param>
        public TimingLogger(string message)
        {
            this.Initialize(message);
        }

        public TimingLogger(string message, params string[] args)
        {
            this.Initialize(string.Format(message, args));
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Lost.TimingLogger"/> object.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Dispose"/> when you are finished using the <see cref="LostEngine.LostTimingLogger"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="LostEngine.LostTimingLogger"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="LostEngine.LostTimingLogger"/> so
        /// the garbage collector can reclaim the memory that the <see cref="LostEngine.LostTimingLogger"/> was occupying.
        /// </remarks>
        public void Dispose()
        {
            this.endTime = DateTime.Now;
            TimeSpan span = this.endTime.Subtract(this.startTime);
            UnityEngine.Debug.LogFormat("{0} took {1} milliseconds", this.message, span.TotalMilliseconds);
        }

        private void Initialize(string message)
        {
            this.message = message;
            this.startTime = DateTime.Now;
        }
    }
}
