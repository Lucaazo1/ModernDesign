using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;

namespace AzureFunctions
{
    public static class ConnectedDevice
    {
        [FunctionName("ConnectedDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "device/connect")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            
            try
            {
                string name = req.Query["deviceId"];

                dynamic data = JsonConvert.DeserializeObject(await new StreamReader(req.Body).ReadToEndAsync());
                name = name ?? data?.name;

                if (!string.IsNullOrEmpty(deviceId))
                {
                    //Det här är api:et emellan
                    using var registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IoTHub"));
                    var device = await registryManager.GetDeviceAsync(deviceId);
                    device ??= await registryManager.AddDeviceAsync(new Device(deviceId));

                    return new OkObjectResult($"{Environment.GetEnvironmentVariable("IoTHub").Split(";")[0]}; DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}");
                }

                return new BadRequestObjectResult("deviceId is Required");
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }


        }
    }
}
