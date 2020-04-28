using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte [1024];
    }
    class Program
    {
        private static Socket listenfd;
        static Dictionary<Socket,ClientState> clients = new Dictionary<Socket, ClientState>();
        static void Main(string[] args)
        {
            // socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Bind
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEd = new IPEndPoint(ipAdr, 8888);
            listenfd.Bind(ipEd);
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功");
            // Accept
            listenfd.BeginAccept(AcceptCallBack,listenfd);
            Console.ReadLine();
        }

        private static void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
            //accept
            Socket listenfd = (Socket) ar.AsyncState;
            Socket clientfd = listenfd.EndAccept(ar);
            Console.WriteLine("[服务器]Accept");
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
            //Receive
            clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallBack, state);
            listenfd.BeginAccept(AcceptCallBack, listenfd);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Accept fail" + e.ToString());
            }
        }

        private static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                ClientState state = (ClientState) ar.AsyncState;
                Socket clientfd = state.socket;
                int count = clientfd.EndReceive(ar);
                if (count == 0)
                {
                    clientfd.Close();
                    clients.Remove(clientfd);
                    Console.WriteLine("Socket Close");
                    return;
                }
                string str = Encoding.Default.GetString(state.readBuff, 0, count);
                string clientHashCode = clientfd.GetHashCode().ToString();
                string recvStr = string.Format("userId{0}: {1}", clientHashCode, str);
                Console.WriteLine();
                byte[] sendByte = Encoding.Default.GetBytes(recvStr);
                foreach (ClientState s in clients.Values)
                {
                    s.socket.Send(sendByte);
                }
                clientfd.BeginReceive(state.readBuff, 0, 1024, 0, ReceiveCallBack, state);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Accept fail" + e.ToString());
            }
        }
    }
}
