using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RepoManager.Server
{
    internal class GitHubTokenService
    {
        public async Task<string> Get(string code, string state, ILogger log)
        {
            try
            {
                string clientId = GetEnvironmentVariable("ClientId");
                string clientSecret = GetEnvironmentVariable("ClientSecret");
                var response = await new HttpClient().PostAsync("https://github.com/login/oauth/access_token",
                new FormUrlEncodedContent(new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string> ("client_id", clientId ),
                    new KeyValuePair<string, string> ("client_secret", clientSecret ),
                    new KeyValuePair<string, string> ("code", code ),
                    new KeyValuePair<string, string> ("state", state )
                }));

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to get token from GitHub");
            }
            
            return "failed";
        }

        private static string GetEnvironmentVariable(string variable)
        {
            return System.Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process);
        }
    }
}