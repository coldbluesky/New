using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using Zhaoxi.DigitaPlatform.DeviceAccess;
using System.Reflection;
using Zhaoxi.DigitaPlatform.Entities;
using Zhaoxi.DigitaPlatform.DeviceAccess.Base;
using System.Threading.Tasks;
using System.Threading;

namespace Zhaoxi.DigitalService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    await Task.Delay(1000, stoppingToken);
            //}
            _logger.LogInformation("ExecuteAsync");
            Console.WriteLine("ExecuteAsync");
            this.StartListen();
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");
            //return Task.Run(() =>
            //{
            //    this.StartListen();
            //});
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync");
            return Task.Run(() =>
            {
                this.Stop();
            }, cancellationToken);
        }

        /***********************************************************************************************/
        Random random = new Random();
        CancellationTokenSource cts = new CancellationTokenSource();
        Socket server;
        List<ClientModel> Clients = new List<ClientModel>();
        List<Task> Tasks = new List<Task>();

        #region ����TCP�������
        private void StartListen()
        {
            try
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, 8899));
                server.Listen(10);
                _logger.LogInformation("TCPԶ�̷����������������ȴ��ͻ��˽���....");

                AcceptClient(server);

                CheckAlive();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("TCPԶ�̷�������ʧ�ܡ�" + ex.Message);
            }
        }
        #endregion
        #region ֹͣ����
        public void Stop()
        {
            _logger.LogInformation("Dispose");
            cts.Cancel();
            Task.WaitAll(Tasks.ToArray());

            server.Shutdown(SocketShutdown.Both);
            server.Close();
            server.Dispose();
        }
        #endregion

        #region ���ͻ��˻�Ծ�ԣ���������
        private void CheckAlive()
        {
            var t = Task.Factory.StartNew(async () =>
            {
                int index = 0;
                while (!cts.IsCancellationRequested)
                {
                    await Task.Delay(1000);

                    if (Clients.Count == 0) continue;

                    if (Clients[index].Lifetime < DateTime.Now)
                    {
                        Clients[index].Client.Shutdown(SocketShutdown.Both);
                        Clients[index].Client.Close();
                        Clients[index].Client.Dispose();

                        Clients.RemoveAt(index);
                    }
                    else
                        index++;
                    index %= Clients.Count;
                }
            }, cts.Token);

            Tasks.Add(t);
        }
        #endregion

        #region ���ܿͻ��˽���
        /// <summary>
        /// ���ܿͻ��˽��룬���ҷַ�һ���ͻ���ID
        /// </summary>
        /// <param name="socket"></param>
        private void AcceptClient(Socket socket)
        {
            var t = Task.Factory.StartNew(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    var client = socket.Accept();

                    try
                    {
                        // �����ͻ���ID
                        ushort id = (ushort)random.Next(0, ushort.MaxValue);
                        while (Clients.Exists(c => c.ID == id))
                        {
                            id = (ushort)random.Next(0, ushort.MaxValue);
                        }
                        byte[] regBytes = new byte[]
                        {
                            0x00,0x00,0x00,0x00,0x01,0x00,0x02,(byte)(id/256),(byte)(id%256)
                        };
                        client.Send(regBytes, 0, regBytes.Length, SocketFlags.None);

                        ClientModel clientModel = new ClientModel { ID = id, Client = client, Lifetime = DateTime.Now.AddSeconds(20) };
                        Clients.Add(clientModel);
                        _logger.LogInformation("�ͻ��˽��� - " + id);

                        // ��ʼ���տͻ�����Ϣ
                        Receive(client);

                        // start subscribe monitor
                        this.MonitorData(clientModel);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("AcceptClient - " + ex.Message);
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        client.Dispose();
                    }

                }
            }, cts.Token);

            Tasks.Add(t);
        }
        #endregion

        #region ���ݽ���
        private void Receive(Socket client)
        {
            var t = Task.Factory.StartNew(() =>
            {
                client.ReceiveTimeout = 0;
                while (!cts.IsCancellationRequested)
                {
                    try
                    {
                        List<byte> totalBytes = new List<byte>();
                        byte[] respBytes = new byte[7];
                        client.Receive(respBytes, 0, 7, SocketFlags.None);
                        totalBytes.AddRange(respBytes);

                        ushort len = BitConverter.ToUInt16(new byte[] { respBytes[6], respBytes[5] });
                        if (len > 0)
                        {
                            byte[] dataBytes = new byte[len];
                            client.Receive(dataBytes, 0, len, SocketFlags.None);
                            totalBytes.AddRange((byte[])dataBytes);
                        }

                        // �ͻ��˱��
                        ushort id = BitConverter.ToUInt16(new byte[] { totalBytes[3], totalBytes[2] });
                        var citem = Clients.FirstOrDefault(c => c.ID == id);
                        if (citem == null) continue;
                        citem.Lifetime = DateTime.Now.AddSeconds(20);

                        List<byte> byteList = new List<byte>();
                        byteList.AddRange(totalBytes.GetRange(0, 5));
                        if (totalBytes[4] == 0xCF)
                        {
                            // ����
                        }
                        else if (totalBytes[4] == 0x03)
                        {
                            _logger.LogInformation("���յ�������Ϣ");
                            // ������Ϣ   �����豸ͨ�Ų��������
                            var infoBytes = totalBytes.GetRange(7, totalBytes.Count - 7);
                            ushort plen = BitConverter.ToUInt16(new byte[] { infoBytes[1], infoBytes[0] });
                            var pBytes = infoBytes.GetRange(2, plen);
                            string pstr = Encoding.Default.GetString(pBytes.ToArray(), 0, plen);
                            _logger.LogInformation(pstr);
                            citem.Properties.Add(pstr.Split(','));

                            int i = 2 + plen;
                            ushort vlen = BitConverter.ToUInt16(new byte[] { infoBytes[i + 1], infoBytes[i] });
                            var vBytes = infoBytes.GetRange(i + 2, vlen);
                            string vstr = Encoding.Default.GetString(vBytes.ToArray(), 0, vlen);
                            _logger.LogInformation(vstr);
                            citem.Variables.Add(vstr.Split(','));
                        }
                        else if (totalBytes[4] == 0x07)
                        {
                            citem.Lifetime = DateTime.Now;
                            continue;
                        }

                        byteList.Add(0x00);
                        byteList.Add(0x00);
                        client.Send(byteList.ToArray(), 0, byteList.Count, SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }
            }, cts.Token);

            Tasks.Add(t);
        }
        #endregion

        #region �������ݻ�ȡ������
        private void MonitorData(ClientModel cm)
        {
            var clientModel = cm;

            var t = Task.Factory.StartNew(() =>
            {
                _logger.LogInformation("��ʼ���ļ��� " + clientModel.ID);
                Communication communication = Communication.Create();

                int dindex = 0;
                while (!cts.IsCancellationRequested)
                {
                    if (clientModel.Properties.Count == 0) continue;

                    var prop = clientModel.Properties[dindex];

                    var result_eo = communication.GetExecuteObject(
                        prop.Select(p =>
                        new DevicePropItemEntity { PropName = p.Split(":")[0], PropValue = p.Split(":")[1] }
                        ).ToList());
                    if (!result_eo.Status)
                    {
                        // �����쳣����
                        _logger.LogInformation("��ȡͨ�Ŷ����쳣");
                        return;
                    }

                    var variable = clientModel.Variables[dindex];
                    dindex++; dindex %= clientModel.Variables.Count;

                    var vs = variable.Select(v => new VariableProperty
                    {
                        VarId = v.Split(":")[0],
                        VarAddr = v.Split(":")[1],
                        ValueType = Type.GetType("System." + v.Split(":")[2])
                    }).ToList();

                    foreach (var v in vs)
                    {
                        try
                        {
                            var ra = result_eo.Data.AnalysisAddress(v);
                            if (!ra.Status)
                            {
                                // �����쳣����
                                _logger.LogInformation("������ַ�쳣");
                                continue;
                            }
                            ra.Data.Variables.Add(ra.Data);
                            var result_value = result_eo.Data.Read(new List<CommAddress> { ra.Data });
                            if (!result_value.Status)
                            {
                                // �����쳣����
                                _logger.LogInformation("��ȡ�����쳣");
                                continue;
                            }

                            if (!clientModel.Values.ContainsKey(v.VarId))
                                clientModel.Values.Add(v.VarId, new byte[] { });

                            if (!clientModel.Values[v.VarId].SequenceEqual(ra.Data.ValueBytes))
                            {
                                _logger.LogInformation("��ȡ�������ݣ�������");
                                // ֪ͨ��ֵ�仯
                                clientModel.Values[v.VarId] = ra.Data.ValueBytes;

                                byte[] idBytes = Encoding.Default.GetBytes(v.VarId);

                                List<byte> sendBytes = new List<byte> {
                                    0x00,0x00,
                                    (byte)(clientModel.ID/256),(byte)(clientModel.ID%256),
                                    0x04,
                                    (byte)((idBytes.Length+ra.Data.ValueBytes.Length+4)/256),
                                    (byte)((idBytes.Length+ra.Data.ValueBytes.Length+4)%256),
                                    (byte)(idBytes.Length/256),
                                    (byte)(idBytes.Length%256),
                                };
                                sendBytes.AddRange(idBytes);
                                sendBytes.Add((byte)(ra.Data.ValueBytes.Length / 256));
                                sendBytes.Add((byte)(ra.Data.ValueBytes.Length % 256));
                                sendBytes.AddRange(ra.Data.ValueBytes);

                                clientModel.Client.Send(sendBytes.ToArray(), 0, sendBytes.Count, SocketFlags.None);
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
            }, cts.Token);

            Tasks.Add(t);

        }
        #endregion
    }
}