namespace Zhaoxi.DigitalService
{
    public class Program
    {
        // 这个项目只能是.NET环境下创建
        // Framework平台下使用WCF，寄宿到Windows服务
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}