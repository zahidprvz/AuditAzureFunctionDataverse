using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using AuditDataverseFunctionApp.Models;

namespace AuditDataverseFunctionApp
{
    public class SendAuditToDataverse
    {
        private readonly ILogger _logger;

        public SendAuditToDataverse(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendAuditToDataverse>();
        }

        [Function("SendAuditToDataverse")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var response = req.CreateResponse();

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var auditPayload = JsonSerializer.Deserialize<AuditPayload>(requestBody);

                var connectionString = Environment.GetEnvironmentVariable("DataverseConnectionString");
                using var serviceClient = new ServiceClient(connectionString);

                foreach (var audit in auditPayload.value)
                {
                    var entity = new Entity("cr356_customauditlogs")
                    {
                        ["cr356_auditid"] = audit.auditid,
                        ["cr356_createdon"] = audit.createdon, 
                        ["cr356__userid_value"] = audit._userid_value,
                        ["cr356_objecttypecode"] = audit.objecttypecode,
                        ["cr356__objectid_value"] = audit._objectid_value,
                        ["cr356_changedata"] = audit.changedata
                    };

                    await serviceClient.CreateAsync(entity);
                }


                response.StatusCode = HttpStatusCode.OK;
                await response.WriteStringAsync($"Inserted {auditPayload.value.Count} audit records.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                response.StatusCode = HttpStatusCode.InternalServerError;
                await response.WriteStringAsync($"Error: {ex.Message}");
            }

            return response;
        }
    }
}



// Code to connect to Dataverse and check the connection status (commented out for clarity)

// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Azure.Functions.Worker.Http;
// using Microsoft.Extensions.Logging;
// using System.Threading.Tasks;
// using Microsoft.PowerPlatform.Dataverse.Client;
// using Microsoft.Xrm.Sdk;
// using System;

// namespace AuditDataverseFunctionApp
// {
//     public class SendAuditToDataverse
//     {
//         private readonly ILogger _logger;

//         public SendAuditToDataverse(ILoggerFactory loggerFactory)
//         {
//             _logger = loggerFactory.CreateLogger<SendAuditToDataverse>();
//         }

//         [Function("SendAuditToDataverse")]
//         public async Task<HttpResponseData> Run(
//             [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
//         {
//             _logger.LogInformation("Connecting to Dataverse...");

//             var connectionString = Environment.GetEnvironmentVariable("DataverseConnectionString");

//             try
//             {
//                 var serviceClient = new ServiceClient(connectionString);

//                 // Try to retrieve user ID to confirm connection
//                 var whoAmIRequest = new OrganizationRequest("WhoAmI");
//                 var response = serviceClient.Execute(whoAmIRequest);

//                 var userId = response.Results["UserId"];
//                 _logger.LogInformation($"Connected! UserId: {userId}");

//                 var httpResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
//                 await httpResponse.WriteStringAsync($"Connected to Dataverse as UserId: {userId}");
//                 return httpResponse;
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError($"Failed to connect: {ex.Message}");
//                 var httpResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
//                 await httpResponse.WriteStringAsync($"Error: {ex.Message}");
//                 return httpResponse;
//             }
//         }
//     }
// }
