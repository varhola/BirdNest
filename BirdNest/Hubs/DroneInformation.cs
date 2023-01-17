using BirdNest.Api;
using BirdNest.Structs;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json.Nodes;

namespace BirdNest.Hubs
{
    public class DroneInformationHub : Hub
    {
        private static Queue<List<JsonNode>> ViolationHistory { get; set; } = new Queue<List<JsonNode>>();

        static readonly ApiClient client = new ApiClient();

        private async Task<List<JsonNode>> getViolatingDrones(List<DronePosition> violations)
        {
            List<JsonNode> result = new List<JsonNode>();
            foreach (DronePosition violation in violations)
            {
                JsonNode? fetchResult = await client.GetJson($"https://assignments.reaktor.com/birdnest/pilots/{violation.id}");
                if (fetchResult != null) {
                    fetchResult.AsObject().Add("distance", violation.distance);
                    result.Add(fetchResult);
                }
            }
            return result;
        }

        private IEnumerable<JsonNode> GetUniqueViolations()
        {
            List<JsonNode> result = new List<JsonNode>();
            IEnumerable<IGrouping<string?, JsonNode>> groups = ViolationHistory.SelectMany(x => x).ToList().GroupBy(i => i["pilotId"]?.ToString());
            foreach (IGrouping<string?, JsonNode> group in groups) {
                JsonNode? minDistance = group.ToList().MinBy(i => (Double?)i["distance"]);
                if (minDistance != null) result.Add(minDistance);
            }
            return result;
        }

        public async Task GetLatestViolations()
        {
            await Clients.All.SendAsync("updateViolations", GetUniqueViolations().Reverse());
        }

        public async Task UpdateLatestViolations(IEnumerable<DronePosition> newPositions)
        {
            bool update = false;
            if (ViolationHistory.Count == 30)
            {
                List<JsonNode> lastItem = ViolationHistory.Dequeue();
                update = lastItem.Count > 0;
            }
            List<DronePosition> newViolations = newPositions.ToList().FindAll( i => i.ndz );
            update = update || newViolations.Count > 0;
            List<JsonNode> violatingDrones = await getViolatingDrones(newViolations);
            ViolationHistory.Enqueue( violatingDrones );
            if ( update ) await Clients.All.SendAsync("updateViolations", GetUniqueViolations().Reverse());
        }
    }
}

