using BirdNest.Structs;
using Microsoft.AspNetCore.SignalR;

namespace BirdNest.Hubs
{
    public class DronePositionsHub : Hub
    {
        private static IEnumerable<DronePosition> LatestPositions = new List<DronePosition>();

        public async Task GetLatestPositions()
        {
            await Clients.All.SendAsync("updatePositions", LatestPositions);
        }

        public async Task UpdateLatestPositions(IEnumerable<DronePosition> newPositions)
        {
            LatestPositions = newPositions;
            await GetLatestPositions();
        }
    }
}

