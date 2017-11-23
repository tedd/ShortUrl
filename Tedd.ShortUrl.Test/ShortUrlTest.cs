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
using Xunit.Abstractions;

namespace Tedd.ShortUrl.Test
{
    public class ShortUrlTest
    {
        private readonly ITestOutputHelper _output;
        private string testAccessToken = "$$TESTTOKEN$!!$CHANGEME$$"; // This one must be created in database!
        private TestServer _server;
        private HttpClient _client;

        public ShortUrlTest(ITestOutputHelper output)
        {
            _output = output;
            // Arrange
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }




        [Fact]
        public async Task CreateAndVisit()
        {
            var (data, response1Object) = await _client.CreateUrl(testAccessToken);

            // Test URL
            var response2 = await _client.GetAsync($"/{response1Object.Key}");
            Assert.True(response2.StatusCode == HttpStatusCode.Redirect);
            Assert.True(response2.Headers.Location.AbsoluteUri == new Uri(data.Url.Replace("$key$", response1Object.Key)).AbsoluteUri);
        }

        [Fact]
        public async Task CreateAndGet()
        {
            var (data, response1Object) = await _client.CreateUrl(testAccessToken);

            // Test URL

            var response2 = await _client.GetAsync($"/Admin/Get/{response1Object.Key}?AccessToken=" + data.AccessToken);
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
                ExpiresUtc = DateTime.Now.AddHours(1),
                MetaData = "123",
                Url = "https://www.google.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var response1 = await _client.PostAsync("/Admin/Create", content);
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
            var (data, response1Object) = await _client.CreateUrl(testAccessToken);

            var response2 = await _client.GetAsync($"/Admin/Get/{response1Object.Key}?AccessToken=INVALID");
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
            Assert.Contains("<HTML", strU);
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
            var createCount = 10000;
            var visitCount = createCount * 10;
            var created = new(AdminCreateRequestModel request, AdminCreateResponseModel response)[createCount];
            var cc1 = createCount >> 1;
            var cc2 = createCount - cc1;
            stopwatch.Start();
            Parallel.For(0, cc1, () =>
                 {
                     var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                     var client = server.CreateClient();
                     return (Server: server, Client: client);
                 },
                (i, pls, sc) =>
                //for (var i = 0; i< cc1;i++)
                {
                    created[i] = sc.Client.CreateUrl(testAccessToken).Result;
                    return sc;
                }, (sc) =>
                    {
                        sc.Client.Dispose();
                        sc.Server.Dispose();
                    });
            var cc1TimeUsed = stopwatch.ElapsedMilliseconds;
            _output.WriteLine($"Round 1: Create time used for {cc1} urls: {(double)cc1TimeUsed} ms total, {(double)cc1TimeUsed / (double)cc1} ms per create");
            stopwatch.Restart();

            Parallel.For(cc1, createCount, () =>
                {
                    var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
                    var client = server.CreateClient();
                    return (Server: server, Client: client);
                },
                (i, pls, sc) =>
                //for (var i = 0; i< cc1;i++)
                {
                    created[i] = sc.Client.CreateUrl(testAccessToken).Result;
                    return sc;
                }, (sc) =>
                {
                    sc.Client.Dispose();
                    sc.Server.Dispose();
                });
            var cc2TimeUsed = stopwatch.ElapsedMilliseconds;
            _output.WriteLine($"Round 2: Create time used for {cc2} urls: {(double)cc2TimeUsed} ms total, {(double)cc2TimeUsed / (double)cc2} ms per create");

            var diffMs = Math.Abs(((double)cc1TimeUsed / (double)cc1) - ((double)cc2TimeUsed / (double)cc2));
            _output.WriteLine($"Differential time between first and second batch: {diffMs} ms");
            Assert.True(diffMs < 10D);
            //Assert.True(createTimeUsed < 1000);

            stopwatch.Restart();

            // Test URL
            var rnd = new Random();

            //Parallel.For((long) 0, visitCount, async (i) =>
            for (var i = 0; i < visitCount; i++)
            {
                var c = rnd.Next(0, createCount);
                var data = created[c].request;
                var response1Object = created[c].response;
                var response2 = await _client.GetAsync($"/{response1Object.Key}");
                Assert.True(response2.StatusCode == HttpStatusCode.Redirect);
                Assert.True(response2.Headers.Location.AbsoluteUri == new Uri(data.Url).AbsoluteUri);
            }//);
            var visitTimeUsed = stopwatch.ElapsedMilliseconds;
            _output.WriteLine($"Visit time used for {visitCount} visits: {visitTimeUsed} ms");

            //Assert.True(visitTimeUsed < 1000);
        }
    }
}
