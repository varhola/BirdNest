using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Xml;

namespace BirdNest.Api
{
    public class ApiClient
    {
        public ApiClient() { }

        private readonly HttpClient client = new HttpClient();

        private async Task<string?> Get(string url) {
            try
            {
                return await client.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Exception Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public async Task<XmlDocument?> GetXml(string url)
        {
            string? getResult = await Get(url);
            if (getResult == null) return null;

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(getResult);
                return xmlDoc;
            }
            catch (XmlException e)
            {
                Console.WriteLine("Exception Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

        public async Task<JsonNode?> GetJson(string url)
        {
            string? getResult = await Get(url);
            if (getResult == null) return null;
            try
            {
                return JsonNode.Parse(getResult);
            }
            catch (JsonException e)
            {
                Console.WriteLine("Exception Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }

    }
}
