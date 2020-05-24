using OBSWebsocketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace ObsOverwatch
{
    public class ObsOverwatchService : ServiceControl
    {
        private bool ServiceIsRestarting;
        private OverwatchConfiguration Configuration;
        private OBSWebsocket ObsWebSocket;
        private FixedSizeQueue<float> ConsecutiveStrains;

        #region Start/Stop

        public bool Start(HostControl hostControl)
        {
            ServiceIsRestarting = false;
            Configuration = OverwatchConfiguration.Read();

            ConsecutiveStrains = new FixedSizeQueue<float>(Configuration.ConsecutiveStrains);

            ObsWebSocket = new OBSWebsocket();
            ObsWebSocket.WSTimeout = TimeSpan.FromSeconds(5);
            ObsWebSocket.StreamStatus += OnStreamStatusReceived;

            ObsWebSocket.Connect(
                Configuration.WebsocketAddress, Configuration.WebsocketPassword);

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Configuration = null;
            return true;
        }

        #endregion Start/Stop

        private void OnStreamStatusReceived(OBSWebsocket sender, StreamStatus status)
        {
            ConsecutiveStrains.Enqueue(status.Strain);
            Console.WriteLine($"Received strain: {status.Strain}");
            if (ConsecutiveStrains.IsFull)
            {
                float averageStrain = GetFloatAverage(ConsecutiveStrains);
                if (!ServiceIsRestarting && averageStrain >= Configuration.StrainRestartLimit)
                    Task.Run(() => RestartStream());
            }
        }

        private void RestartStream()
        {
            ServiceIsRestarting = true;
            ObsWebSocket.StopStreaming();

            ConsecutiveStrains.Clear();
            Thread.Sleep(Configuration.TimeAfterStopBeforeStart);

            ObsWebSocket.StartStreaming();
            ServiceIsRestarting = false;
        }

        private static float GetFloatAverage(IEnumerable<float> collection)
        {
            int count = collection.Count();
            float total = 0;

            foreach (float strain in collection)
                total += strain;

            return total / count;
        }
    }
}
