using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tedd.ShortUrl.Web.Models;
using Xunit;

namespace Tedd.ShortUrl.Test
{
    public static class ShortUrlTextExtensions
    {
        public static async Task<(AdminCreateRequestModel request, AdminCreateResponseModel response)> CreateUrl(this HttpClient client, string testAccessToken)
        {
            // Arrange


            // Create URL
            var data = new AdminCreateRequestModel()
            {
                AccessToken = testAccessToken,
                ExpiresUtc = DateTime.Now.AddHours(1),
                MetaData = "123",
                Url = "https://www.google.com/?q=$key$"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response1 = await client.PostAsync("/Admin/Create", content);
            response1.EnsureSuccessStatusCode();
            var response1String = await response1.Content.ReadAsStringAsync();
            var response1Object = JsonConvert.DeserializeObject<AdminCreateResponseModel>(response1String);
            Assert.True(response1Object.Success);
            Assert.True(!string.IsNullOrEmpty(response1Object.Key));
            Assert.True(response1Object.Key.Length > 4);
            Assert.True(response1Object.Url.ToUpperInvariant().Contains(response1Object.Key.ToUpperInvariant()));
            return (request: data, response: response1Object);
        }
    }
}