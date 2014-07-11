// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonitorService.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the MonitorService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FritzBoxCallMonitor
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;

    public class MonitorService
    {
        private ConcurrentDictionary<int, string> incomingCallDictionary;

        /// <summary>
        /// Signalizes an incoming call.
        /// </summary>
        public event EventHandler<IncomingCallEventArgs> IncomingCall;

        /// <summary>
        /// Signalizes an unanswered incoming call canceled by the caller.
        /// </summary>
        public event EventHandler<IncomingCallEventArgs> IncomingCallCallerHangUp;

        /// <summary>
        /// Signalizes an answered incoming call (connection established on a handset).
        /// </summary>
        public event EventHandler<IncomingCallEventArgs> IncomingCallAnswered;

        /// <summary>
        /// Signalizes a disconnected answered incoming call.
        /// </summary>
        public event EventHandler<IncomingCallEventArgs> IncomingCallDisconnect;

        public MonitorService(string fritzBoxHost = "fritz.box")
        {
            this.incomingCallDictionary = new ConcurrentDictionary<int, string>();

            IPAddress fritzBoxIp = null;
            IPAddress.TryParse(fritzBoxHost, out fritzBoxIp);
            if (fritzBoxIp == null)
            {
                fritzBoxIp = Dns.GetHostEntry(fritzBoxHost).AddressList.First();
            }

            var tcpClient = new EventDrivenTcpClient(fritzBoxIp, 1012, true);
            tcpClient.DataReceived += this.TcpClientDataReceived;
            tcpClient.ConnectionStatusChanged += this.TcpClientConnectionStatusChanged;
            tcpClient.ReconnectInterval = 30000;
            tcpClient.Connect();
        }

        private void TcpClientConnectionStatusChanged(
            EventDrivenTcpClient sender,
            EventDrivenTcpClient.ConnectionStatus status)
        {
            Debug.Print("Connection status changed: {0}", status);
        }

        private void TcpClientDataReceived(EventDrivenTcpClient sender, object data)
        {
            Debug.Print("Message received: {0}", data);

            //Outgoing call:             Date;CALL;ConnectionID;LocalExtension;LocalNumber;RemoteNumber;
            //Incoming call:             Date;RING;ConnectionID;RemoteNumber;LocalNumber;
            //On connection established: Date;CONNECT;ConnectionID;LocalExtension;RemoteNumber;
            //On connection end:         Date;DISCONNECT;ConnectionID;DurtionInSeconds;

            string[] messageParts = data.ToString().Split(';');
            DateTime timestamp = DateTime.Parse(messageParts[0]);
            string eventType = messageParts[1];
            int connectionId = int.Parse(messageParts[2]);

            switch (eventType)
            {
                case "CALL":
                    {
                        string localExtension = messageParts[3];
                        string localNumber = messageParts[4];
                        string remoteNumber = messageParts[5];

                        // todo something with outgoing calls..
                    }

                    break;
                case "RING":
                    {
                        string remoteNumber = messageParts[3];

                        string localNumber = messageParts[4];

                        // add to call dictionary
                        this.incomingCallDictionary.TryAdd(connectionId, remoteNumber);

                        var handler = this.IncomingCall;

                        if (handler != null)
                        {
                            handler(this, new IncomingCallEventArgs(remoteNumber, connectionId));
                        }
                    }

                    break;
                case "CONNECT":
                    {
                        string localExtension = messageParts[3];

                        string remoteNumber = messageParts[4];

                        var handler = this.IncomingCallAnswered;

                        // determine if we wether disconnected an incoming or an outgoing call
                        if (this.incomingCallDictionary.TryGetValue(connectionId, out remoteNumber))
                        {
                            if (handler != null)
                            {
                                handler(this, new IncomingCallEventArgs(remoteNumber, connectionId));
                            }
                        }
                        else
                        {
                            // todo something with outgoing call connects..
                        }
                    }

                    break;
                case "DISCONNECT":
                    {
                        int durationInSeconds = int.Parse(messageParts[3]);
                        string remoteNumber;

                        var handler = this.IncomingCallCallerHangUp;

                        // determine if we wether disconnected an incoming or an outgoing call
                        if (this.incomingCallDictionary.TryGetValue(connectionId, out remoteNumber))
                        {
                            // caller hang up before connection was established
                            if (durationInSeconds == 0)
                            {
                                if (handler != null)
                                {
                                    handler(this, new IncomingCallEventArgs(remoteNumber, connectionId));
                                }
                            }
                            else
                            {
                                handler = this.IncomingCallDisconnect;
                                if (handler != null)
                                {
                                    handler(
                                        this,
                                        new IncomingCallEventArgs(remoteNumber, connectionId, durationInSeconds));
                                }
                            }

                            // remove call from call dictionary
                            this.incomingCallDictionary.TryRemove(connectionId, out remoteNumber);
                        }
                        else
                        {
                            // todo something with outgoing call disconnects..
                        }
                }

                    break;
            }
        }
    }
}

