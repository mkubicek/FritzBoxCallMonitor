// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncomingCall.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IncomingCall type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FritzBoxCallMonitor
{
    using System;

    public class IncomingCallEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingCallEventArgs"/> class.
        /// </summary>
        /// <param name="remoteNumber">
        /// The remote telephone number.
        /// </param>
        public IncomingCallEventArgs(string remoteNumber, int connectionId, int connectionDurationInSeconds = 0)
        {
            this.RemoteNumber = remoteNumber;
            this.ConnectionId = connectionId;
            this.ConnectionDurationInSeconds = connectionDurationInSeconds;
        }

        public string RemoteNumber { get; private set; }

        public int ConnectionId { get; private set; }

        public int ConnectionDurationInSeconds { get; private set; }
    }
}
