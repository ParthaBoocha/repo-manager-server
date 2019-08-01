using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RepoManager.Server
{
    public static class Token
    {
        [FunctionName("Token")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing Token request");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string code = data?.code;
            string state = data?.state;

            if(!string.IsNullOrEmpty(code)
                && !string.IsNullOrEmpty(state))
            {
                var token = await new GitHubTokenService().Get(code, state, log);
                return (ActionResult)new OkObjectResult((new { token = token }));
            }
            
            return new BadRequestObjectResult("code and state are required to get token");
        }
    }
}
