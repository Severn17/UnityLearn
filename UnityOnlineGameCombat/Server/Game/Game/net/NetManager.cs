using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

class NetManager
{
    //监听Socket
    public static Socket listenfd;
    //客户端Socket及状态信息
    public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
    //Select的检查列表
    static List<Socket> checkRead = new List<Socket>();

    public static void StartLoop(int listenPort)
    {
        // Socket
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Bind
        IPAddress ipAdr = IPAddress.Parse("0.0.0.0");
        IPEndPoint iPEp = new IPEndPoint(ipAdr, listenPort);
        listenfd.Bind(iPEp);
        // Listen
        listenfd.Listen(0);
        Console.WriteLine("[服务器]启动成功");
        // 循环
        while (true)
        {
            ResetCheckRead(); //重置
            Socket.Select(checkRead, null, null, 1000);
            // 检查可读对象
            for (int i = checkRead.Count - 1; i >= 0; i--)
            {
                Socket s = checkRead[i];
                if (s==listenfd)
                {
                    ReadListenfd(s);
                }
                else
                {
                    ReadClientfd(s);
                }
            }
            // 超时
            Timer();
        }
    }

    private static void Timer()
    {
        throw new NotImplementedException();
    }

    private static void ReadClientfd(Socket s)
    {
        throw new NotImplementedException();
    }

    private static void ReadListenfd(Socket s)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// 填充ChechRead
    /// </summary>
    private static void ResetCheckRead()
    {
        checkRead.Clear();
        checkRead.Add(listenfd);
        foreach (ClientState s in clients.Values)
        {
            checkRead.Add(s.socket);
        }
    }
}
