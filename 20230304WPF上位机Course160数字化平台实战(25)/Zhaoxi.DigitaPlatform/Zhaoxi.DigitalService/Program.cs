namespace Zhaoxi.DigitalService
{
    public class Program
    {
        // �����Ŀֻ����.NET�����´���
        // Frameworkƽ̨��ʹ��WCF�����޵�Windows����
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