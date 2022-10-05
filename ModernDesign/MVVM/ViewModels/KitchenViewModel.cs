using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using ModernDesign.Helpers;
using ModernDesign.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ModernDesign.MVVM.ViewModels
{
    internal class KitchenViewModel
    {
        private DispatcherTimer timer;
        private ObservableCollection<DeviceInfo> _deviceInfo;
        private List<DeviceInfo> _tempList;
        private readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString("HostName=kyh-shared-iothub.azure-devices.net;SharedAccessKeyName=smartapp;SharedAccessKey=R/N6ZzliBw7DCnNxc1RihORopUA2A0gq5ssFlGT+jO4=");

        public KitchenViewModel(NavigationStore navigationStore)
        {
            _tempList = new List<DeviceInfo>();
            _deviceInfo = new ObservableCollection<DeviceInfo>();
            PopulateDeviceInfosAsync().ConfigureAwait(false);
            SetInterval(TimeSpan.FromSeconds(3));
        }


        public string Title { get; set; } = "Kitchen";
        public string Temperature { get; set; } = "23 °C";
        public string Humidity { get; set; } = "34 %";
        public IEnumerable<DeviceInfo> DeviceInfos => _deviceInfo;



        private void SetInterval(TimeSpan interval)
        {
            timer = new DispatcherTimer()
            {
                Interval = interval
            };

            timer.Tick += new EventHandler(timer_tick);
            timer.Start();
        }

        private async void timer_tick(object sender, EventArgs e)
        {
            await PopulateDeviceInfosAsync();
            await UpdateDeviceInfosAsync();
        }


        private async Task UpdateDeviceInfosAsync()
        {
            _tempList.Clear();

            foreach (var item in _deviceInfo)
            {
                var device = await registryManager.GetDeviceAsync(item.DeviceId);
                if (device == null)
                    _tempList.Add(item);
            }

            foreach (var item in _tempList)
            {
                _deviceInfo.Remove(item);
            }
        }

        private async Task PopulateDeviceInfosAsync()
        {
            //var result = registryManager.CreateQuery("select * from devices where location = 'kitchen'");
            var result = registryManager.CreateQuery("select * from devices where properties.reported.location = 'kitchen'");

            if (result.HasMoreResults)
            {
                foreach (Twin twin in await result.GetNextAsTwinAsync())
                {
                    var device = _deviceInfo.FirstOrDefault(x => x.DeviceId == twin.DeviceId);

                    if (device == null)
                    {
                        device = new DeviceInfo
                        {
                            DeviceId = twin.DeviceId,
                        };

                        try { device.DeviceName = twin.Properties.Reported["deviceName"]; }
                        catch { device.DeviceName = device.DeviceId; }
                        try { device.DeviceType = twin.Properties.Reported["deviceType"]; }
                        catch { }

                        switch (device.DeviceType.ToLower())
                        {
                            case "fan":
                                device.IconActive = "\uf863";
                                device.IconInActive = "\uf863";
                                device.StateActive = "ON";
                                device.StateInActive = "OFF";
                                break;

                            case "light":
                                device.IconActive = "\uf672";
                                device.IconInActive = "\uf0eb";
                                device.StateActive = "ON";
                                device.StateInActive = "OFF";
                                break;

                            default:
                                device.IconActive = "\uf2db";
                                device.IconInActive = "\uf2db";
                                device.StateActive = "ENABLE";
                                device.StateInActive = "DISABLE";
                                break;
                        }

                        _deviceInfo.Add(device);
                    }
                    else { }
                }
            }
            else
            {
                _deviceInfo.Clear();
            }
        }
    }
}
