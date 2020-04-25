using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("hello world");

            // socket
            Socket listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEd = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEd);
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            while (true)
            {
                //accept
                Socket connfd = listenfd.Accept();
                Console.WriteLine("[服务器]Accept");
                //Receive
                byte[] readBuff = new byte[1024];
                int count = connfd.Receive(readBuff);
                string readStr = Encoding.Default.GetString(readBuff, 0, count);
                Console.WriteLine("[服务器接收]" + readStr);
                //Send
                byte[] sendBytes = Encoding.Default.GetBytes(readStr);
                connfd.Send(sendBytes);
            }
        }
        public static void GetTime(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Contains("时间"))
                {
                    Console.WriteLine("[服务器]当前时间:" + DateTime.Now.ToString());
                }
            }
        }
    }
}
