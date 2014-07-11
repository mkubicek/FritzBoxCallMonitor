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
        private static MonitorService monitorService = new MonitorService();
        static void Main(string[] args)
        {
            Console.WriteLine("Starting..");

            monitorService.IncomingCall += monitorService_IncomingCall;
            monitorService.IncomingCallAnswered += monitorService_IncomingCallAnswered;
            monitorService.IncomingCallCallerHangUp += monitorService_IncomingCallCallerHangUp;
            monitorService.IncomingCallDisconnect += monitorService_IncomingCallDisconnect;

            while (true)
            {
                Console.ReadKey(true);
            }
        }

        static void monitorService_IncomingCallDisconnect(object sender, IncomingCallEventArgs e)
        {
            Write("Call ended: " + e.RemoteNumber + " how long? " + e.ConnectionDurationInSeconds + " seconds");
        }

        static void monitorService_IncomingCallCallerHangUp(object sender, IncomingCallEventArgs e)
        {
            Write("Ups.. Caller hang up before we were able to pick up the phone.. " + e.RemoteNumber);
        }

        static void monitorService_IncomingCallAnswered(object sender, IncomingCallEventArgs e)
        {
            Write("Call answered: " + e.RemoteNumber);
        }

        static void monitorService_IncomingCall(object sender, IncomingCallEventArgs e)
        {
            Write("Incoming call!: " + e.RemoteNumber);
        }

        static void Write(string message)
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + message);
        }
    }
}
