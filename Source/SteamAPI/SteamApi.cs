using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Creventive.SteamAPI
{
    /// <summary>
    /// Wrapper around a small set of steam API methods
    /// </summary>
    public class SteamApi : IDisposable
    {
        HttpClient _webClient;

        public SteamApi()
        {
            _webClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.steampowered.com/ISteamRemoteStorage/")
            };
        }

        public async Task<SteamPublishedFileDetails[]> GetPublishedFileDetailsAsync(CancellationToken token, params long[] ids)
        {
            var fields = new Dictionary<string, string>
            {
                ["itemcount"] = ids.Length.ToString()
            };
            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                fields[$"publishedfileids[{i}]"] = id.ToString();
            }
            var content = new FormUrlEncodedContent(fields);
            var responseData = await _webClient.PostAsync("GetPublishedFileDetails/v1/", content, token);
            if (responseData.IsSuccessStatusCode)
            {
                var json = await responseData.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<RootSteamResponse<GetPublishedFileDetailsResponse>>(json);
                return response.Response.PublishedFileDetails.ToArray();
            }

            return new SteamPublishedFileDetails[0];
        }

        public void Dispose()
        {
            _webClient?.Dispose();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class GetPublishedFileDetailsResponse : SteamResponse
        {
            public List<SteamPublishedFileDetails> PublishedFileDetails { get; } = new List<SteamPublishedFileDetails>();
        }
    }
}
