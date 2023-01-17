using BirdNest.Api;
using BirdNest.Structs;
using Microsoft.AspNetCore.SignalR.Client;
using System.Xml;

namespace BirdNest.BackgroundServices
{
    public class PositionFetcherService : IHostedService, IDisposable
    {
        private Timer? _timer = null;

        private HubConnection dronePositionConnection = new HubConnectionBuilder().WithUrl("http://localhost:5064/birdNest").WithAutomaticReconnect().Build();
        private HubConnection droneHubConnection = new HubConnectionBuilder().WithUrl("http://localhost:5064/droneInfo").WithAutomaticReconnect().Build();

        // private HubConnection dronePositionConnection = new HubConnectionBuilder().WithUrl("https://birdnest20230115134750.azurewebsites.net/birdNest").WithAutomaticReconnect().Build();
        // private HubConnection droneHubConnection = new HubConnectionBuilder().WithUrl("https://birdnest20230115134750.azurewebsites.net/droneInfo").WithAutomaticReconnect().Build();

        private static readonly ApiClient client = new ApiClient();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            XmlDocument? xmlDoc = await client.GetXml("http://assignments.reaktor.com/birdnest/drones");

            XmlNodeList? nodes = xmlDoc?.SelectNodes("report/capture/drone");

            List<DronePosition> result = new List<DronePosition>();
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    XmlNode? serialNumber = node.SelectSingleNode("serialNumber");
                    XmlNode? positionX = node.SelectSingleNode("positionX");
                    XmlNode? positionY = node.SelectSingleNode("positionY");

                    if (
                        serialNumber != null &&
                        positionX != null &&
                        positionY != null
                    ) {
                        DronePosition newPos = new DronePosition
                            (
                                id: serialNumber.InnerText,
                                x: Convert.ToDouble(positionX.InnerText),
                                y: Convert.ToDouble(positionY.InnerText)
                            );

                        result.Add( newPos );
                    }
                }
                if (dronePositionConnection.State == HubConnectionState.Disconnected) await dronePositionConnection.StartAsync();
                if (dronePositionConnection.State == HubConnectionState.Connected) await dronePositionConnection.InvokeAsync("UpdateLatestPositions", result);

                if (droneHubConnection.State == HubConnectionState.Disconnected) await droneHubConnection.StartAsync();
                if (dronePositionConnection.State == HubConnectionState.Connected)  await droneHubConnection.InvokeAsync("UpdateLatestViolations", result);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
