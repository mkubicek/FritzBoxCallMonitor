using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFritzCallMonitor
{
    using System.Threading;

    using FritzBoxCallMonitor;

    class Program
    {
        private static FritzBoxCallMonitorService fritzBoxCallMonitorService = new FritzBoxCallMonitorService();
        static void Main(string[] args)
        {
            Console.WriteLine("Starting..");

            fritzBoxCallMonitorService.IncomingCall += FritzBoxCallMonitorServiceIncomingCall;
            fritzBoxCallMonitorService.IncomingCallAnswered += FritzBoxCallMonitorServiceIncomingCallAnswered;
            fritzBoxCallMonitorService.IncomingCallCallerHangUp += FritzBoxCallMonitorServiceIncomingCallCallerHangUp;
            fritzBoxCallMonitorService.IncomingCallDisconnect += FritzBoxCallMonitorServiceIncomingCallDisconnect;

            while (true)
            {
                Console.ReadKey(true);
            }
        }

        static void FritzBoxCallMonitorServiceIncomingCallDisconnect(object sender, IncomingCallEventArgs e)
        {
            Write("Call ended: " + e.RemoteNumber + " how long? " + e.ConnectionDurationInSeconds + " seconds");
        }

        static void FritzBoxCallMonitorServiceIncomingCallCallerHangUp(object sender, IncomingCallEventArgs e)
        {
            Write("Ups.. Caller hang up before we were able to pick up the phone.. " + e.RemoteNumber);
        }

        static void FritzBoxCallMonitorServiceIncomingCallAnswered(object sender, IncomingCallEventArgs e)
        {
            Write("Call answered: " + e.RemoteNumber);
        }

        static void FritzBoxCallMonitorServiceIncomingCall(object sender, IncomingCallEventArgs e)
        {
            Write("Incoming call!: " + e.RemoteNumber);
        }

        static void Write(string message)
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + message);
        }
    }
}
