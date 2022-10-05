using Microsoft.Azure.Devices;
using ModernDesign.MVVM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernDesign.MVVM.Views
{
    /// <summary>
    /// Interaction logic for KitchenView.xaml
    /// </summary>
    public partial class KitchenView : UserControl
    {
        public KitchenView()
        {
            InitializeComponent();
        }
        
        private async void btn_DirectMethod_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var deviceInfo = (DeviceInfo)button!.DataContext;
            //MessageBox.Show(deviceInfo.DeviceId);

            using ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("HostName=1234goodIoThubname.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=kjaa5RplwIgdxdWsmFKjTzk7GP/9nLmf/9FSt59ruzw=");

            var directMethod = new CloudToDeviceMethod("StartStop");
            directMethod.SetPayloadJson(JsonConvert.SerializeObject(new { interval = 50000 }));


            var result = await serviceClient.InvokeDeviceMethodAsync(deviceInfo.DeviceId, directMethod);
            //var data = result.GetPayloadAsJson();
        }
    }
}
