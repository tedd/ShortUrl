using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tedd.ShortUrl.Web;
using Tedd.ShortUrl.Web.Controllers;
using Tedd.ShortUrl.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace Tedd.ShortUrl.Test
{
    public class ShortUrlTest
    {
        private string testAccessToken = "$$TESTTOKEN$$CHANGEME$$";
        private TestServer _server;
        private HttpClient _client;

        public ShortUrlTest()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }


        private async Task<(AdminCreateRequestModel request, AdminCreateResponseModel response)> CreateUrl()
        {
            // Create URL
            var data = new AdminCreateRequestModel()
            {
                AccessToken = testAccessToken,
                Expires = DateTime.Now.AddHours(1),
                MetaData = "123",
                Url = "https://www.google.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response1 = await _client.PutAsync("/Admin/Create", content);
            response1.EnsureSuccessStatusCode();
            var response1String = await response1.Content.ReadAsStringAsync();
            var response1Object = JsonConvert.DeserializeObject<AdminCreateResponseModel>(response1String);
            Assert.True(response1Object.Success);
            Assert.True(!string.IsNullOrEmpty(response1Object.Key));
            Assert.True(response1Object.Key.Length > 4);
            return (request: data, response: response1Object);
        }

        [Fact]
        public async Task CreateAndVisit()
        {
            var (data, response1Object) = await CreateUrl();

            // Test URL
            var response2 = await _client.GetAsync($"/{response1Object.Key}");
            Assert.True(response2.StatusCode == HttpStatusCode.Redirect);
            Assert.True(response2.Headers.Location.AbsoluteUri == new Uri(data.Url).AbsoluteUri);
        }

        [Fact]
        public async Task CreateAndGet()
        {
            var (data, response1Object) = await CreateUrl();

            // Test URL

            var response2 = await _client.GetAsync($"/Admin/Get/{response1Object.Key}?AccessToken="+data.AccessToken);
            response2.EnsureSuccessStatusCode();
            var response2String = await response2.Content.ReadAsStringAsync();
            var response2Object = JsonConvert.DeserializeObject<AdminGetResponseModel>(response2String);
            Assert.True(response2Object.Success);
//            Assert.True(response2Object.ShortUrlModel.);
        }

        private async Task CreateAccessDenied()
        {
            // Create URL
            var data = new AdminCreateRequestModel()
            {
                AccessToken = "INVALID",
                Expires = DateTime.Now.AddHours(1),
                MetaData = "123",
                Url = "https://www.google.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response1 = await _client.PutAsync("/Admin/Create", content);
            response1.EnsureSuccessStatusCode();
            var response1String = await response1.Content.ReadAsStringAsync();
            var response1Object = JsonConvert.DeserializeObject<AdminCreateResponseModel>(response1String);
            Assert.False(response1Object.Success);
            Assert.True(string.IsNullOrEmpty(response1Object.Key));
            Assert.True(string.IsNullOrEmpty(response1Object.Key));
            
        }


        [Fact]
        public async Task GetNotExist()
        {          
            var response2 = await _client.GetAsync($"/Admin/Get/FAKE?AccessToken=" + testAccessToken);
            response2.EnsureSuccessStatusCode();
            var response2String = await response2.Content.ReadAsStringAsync();
            var response2Object = JsonConvert.DeserializeObject<AdminGetResponseModel>(response2String);
            Assert.True(response2Object.Success);
            //            Assert.True(response2Object.ShortUrlModel.);
        }

        [Fact]
        public async Task GetAccessDenied()
        {
            var (data, response1Object) = await CreateUrl();

            var response2 = await _client.GetAsync($"/Admin/Get/{response1Object.Key}?AccessToken=INVALID" );
            response2.EnsureSuccessStatusCode();
            var response2String = await response2.Content.ReadAsStringAsync();
            var response2Object = JsonConvert.DeserializeObject<AdminGetResponseModel>(response2String);
            Assert.False(response2Object.Success);
            //            Assert.True(response2Object.ShortUrlModel.);
        }

   

        [Fact]
        public async Task KeyNotFound()
        {
            var falseKey = "ABC123";

            // Test URL
            var response2 = await _client.GetAsync($"/{falseKey}");
            Assert.True(response2.StatusCode == HttpStatusCode.OK);
            var str = await response2.Content.ReadAsStringAsync();
            var strU = str.ToUpper();
            Assert.Contains("<HTML",strU);
        }


        //[Fact]
        //public async Task KeyNotFound()
        //{
        //    var falseKey = "ABC123";

        //    // Test URL
        //    var response2 = await _client.GetAsync($"/{falseKey}");
        //    Assert.True(response2.StatusCode == HttpStatusCode.NotFound);
        //}

        [Fact]
        public async Task CreateAndVisitMuch()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var count = 10000000;
            var created = new (AdminCreateRequestModel request, AdminCreateResponseModel response)[count];
            Parallel.For(0, count, async (i) =>
            {
                created[i] = await CreateUrl();
            });

            var createTimeUsed = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            // Test URL
            Parallel.For((long) 0, count, async (i) =>
            {
                var data = created[i].request;
                var response1Object = created[i].response;
                var response2 = await _client.GetAsync($"/{response1Object.Key}");
                Assert.True(response2.StatusCode == HttpStatusCode.Redirect);
                Assert.True(response2.Headers.Location.AbsoluteUri == new Uri(data.Url).AbsoluteUri);
            });
            var visitTimeUsed = stopwatch.ElapsedMilliseconds;

        }
    }
}
