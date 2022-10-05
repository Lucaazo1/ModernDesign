using System.Data.SqlClient;
using System.Net.Http.Json;
using Dapper;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

DeviceClient deviceClient;

//var isconfigured

var deviceName = "";
var deviceType = "";
var location = "";

var connectionstring = "HostName=1234goodIoThubname.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=kjaa5RplwIgdxdWsmFKjTzk7GP/9nLmf/9FSt59ruzw=";

using var conn = new SqlConnection(connectionstring);
var deviceId = await conn.QueryFirstOrDefaultAsync<string>("select deviceId from settings");

if (string.IsNullOrEmpty(deviceId))
{
    //await conn.executeAsync()
    deviceId = Guid.NewGuid().ToString();

    Console.WriteLine("Enter Device Name: ");
    deviceName = Guid.NewGuid().ToString();
    Console.WriteLine("Enter Device Type: ");
    deviceType = Guid.NewGuid().ToString();
    Console.WriteLine("Enter location: ");
    location = Guid.NewGuid().ToString();


    using var client = new HttpClient();
    var result = await client.PostAsJsonAsync("http/localhost:7066/api/device/connect", new { deviceId = deviceId });
    var device_connectionString = await result.Content.ReadAsStringAsync();

    //MQTT läs lite snabbt om det men det är sätt som default.
    deviceClient = DeviceClient.CreateFromConnectionString(device_connectionString, TransportType.Mqtt);

    var twin = deviceClient.GetTwinAsync();
    try
    {
        var interval = (int)twin.Properties.Desired["interval"];
    }
    catch
    { }


    var twinCollection = new TwinCollection();
    twinCollection["deviceName"] = deviceName;
    twinCollection["deviceType"] = deviceType;
    twinCollection["location"] = location;

    await deviceClient.UpdateReportedPropertiesAsync(twinCollection);
    await conn.ExecuteAsync("Insert into settings values (@DeviceId, @ConnectionString, @DeviceName, @DeviceType, @Location)", new
    {
        DeviceId = deviceId,
        ConnectionString = device_connectionString,
        DeviceName = deviceName,
        DeviceType = deviceType,
        Location = location
    });

    //foreach (var item in Twin)
    //{ }
}

using var client = new HttpClient();
var result = await client.PostAsJsonAsync("", new {deviceId});
device_connectionString = await result.Content.ReadAsStringAsync();


