using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet;
using System.Diagnostics;
using System.Text;
using MQTTnet.Extensions.ManagedClient;

namespace Zhaoxi.Digital.Mobile;

public partial class App : Application
{
    public static IManagedMqttClient MqttClient;

    public static event EventHandler<string> PushlishMessage;

    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();


        // 初始化MQTT
        Connect();
    }

    void Connect()
    {
        // 连接到MQTT服务器
        MqttClient = new MqttFactory().CreateManagedMqttClient();
        MqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(e =>
        {
            // 订阅主题过滤器
            MqttTopicFilter topicFilter = new MqttTopicFilterBuilder()
                .WithTopic("pub/#")// 主题
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
                .Build();
            MqttClient.SubscribeAsync(topicFilter);
        });
        MqttClient.ApplicationMessageReceivedHandler = 
            new MqttApplicationMessageReceivedHandlerDelegate(Client_ApplicationMessageReceived);

        IMqttClientOptions clientOptions = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer("47.109.25.112", 1883)
            .WithCredentials("admin", "123456")
            .Build();
        IManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(clientOptions)
            .Build();

        MqttClient.StartAsync(options).GetAwaiter().GetResult();

    }

    void Client_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        Debug.WriteLine(">>> 收到消息:" + e.ApplicationMessage.ConvertPayloadToString() + ",来自客户端" + e.ClientId + ",主题:" + e.ApplicationMessage.Topic);

        PushlishMessage?.Invoke(this, Encoding.Default.GetString(e.ApplicationMessage.Payload));
    }
}
