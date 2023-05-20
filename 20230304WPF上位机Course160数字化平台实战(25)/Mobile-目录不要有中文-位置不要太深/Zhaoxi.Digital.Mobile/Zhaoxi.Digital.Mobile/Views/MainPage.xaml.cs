using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using MQTTnet.Client.Connecting;
using System.Text;
using System.Diagnostics;
using Zhaoxi.Digital.Mobile.ViewModels;

namespace Zhaoxi.Digital.Mobile.Views;

public partial class MainPage : ContentPage
{
    //int count = 0;

    public MainPage()
    {
        InitializeComponent();

        this.BindingContext = new MainViewModel();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        //count++;

        //if (count == 1)
        //    CounterBtn.Text = $"Clicked {count} time";
        //else
        //    CounterBtn.Text = $"Clicked {count} times";

        //SemanticScreenReader.Announce(CounterBtn.Text);

        Connect();
    }

    IManagedMqttClient client;
    void Connect()
    {
        // 连接到MQTT服务器
        client = new MqttFactory().CreateManagedMqttClient();
        client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(e =>
        {
            // 订阅主题过滤器
            MqttTopicFilter topicFilter = new MqttTopicFilterBuilder()
                .WithTopic("pub/#")// 主题
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                .Build();
            client.SubscribeAsync(topicFilter);
        });
        client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(Client_ApplicationMessageReceived);

        IMqttClientOptions clientOptions = new MqttClientOptionsBuilder()
            .WithClientId("ZX001")
            .WithTcpServer("47.109.25.112", 1883)
            .WithCredentials("admin", "123456")
            .Build();
        IManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(clientOptions)
            .Build();

        client.StartAsync(options).GetAwaiter().GetResult();

    }
    void Client_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        Debug.WriteLine(">>> 收到消息:" + e.ApplicationMessage.ConvertPayloadToString() + ",来自客户端" + e.ClientId + ",主题:" + e.ApplicationMessage.Topic);
    }

    private async void OnCommandClicked(object sender, EventArgs e)
    {
        MqttApplicationMessage message = new MqttApplicationMessageBuilder()
            .WithTopic("cmd/20230113224241947/40002")
            .WithPayload(Encoding.Default.GetBytes("100"))
            .WithRetainFlag(false)
            .Build();
        await client.PublishAsync(message);
    }
}

