using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventHandler = System.EventHandler;

public class ClientState
    {
        public Socket socket;
        public byte[] readBuff = new byte [1024];
        public int hp = -100;
        public float x = 0;
        public float y = 0;
        public float z = 0;
        public float eulY = 0;
    }
    class Program
    {
        private static Socket listenfd;
        public static Dictionary<Socket,ClientState> clients = new Dictionary<Socket, ClientState>();
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
            List<Socket> checkRead = new List<Socket>();
            while (true)
            {
                checkRead.Clear();
                checkRead.Add(listenfd);
                foreach (ClientState s  in clients.Values)
                {
                    checkRead.Add(s.socket);
                }
                // selete
                Socket.Select(checkRead, null, null, 1000);
                foreach (Socket s in checkRead)
                {
                    if (s == listenfd)
                    {
                        ReadListenfd(s);
                    }
                    else
                    {
                        ReadClientfd(s);
                    }
                }
            }
            // Accept
            //listenfd.BeginAccept(AcceptCallBack,listenfd);
            //Console.ReadLine();
        }

        private static bool ReadClientfd(Socket clientfd)
        {
            ClientState state = clients[clientfd];
            int count = 0;
            try
            {
                count = clientfd.Receive(state.readBuff);
            }
            catch (SocketException e)
            {
                MethodInfo mei =  typeof(global::EventHandler).GetMethod("OnDisconnect");
                object[] ob = {state};
                mei.Invoke(null, ob);
                
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Receive SocketException" + e.ToString());
                return false;
            }
            if (count <= 0)
            {
                MethodInfo mei = typeof(global::EventHandler).GetMethod("OnDisconnect");
                object[] ob = {state};
                mei.Invoke(null, ob);
                
                clientfd.Close();
                clients.Remove(clientfd);
                Console.WriteLine("Socket Close");
                return false;
            }
            // 广播
            string str = Encoding.Default.GetString(state.readBuff, 0, count);
            Console.WriteLine(str);
            string[] split = str.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            String funName = "Msg" + msgName;
            MethodInfo mi = typeof(MsgHandler).GetMethod(funName);
            object[] o = {state, msgArgs};
            mi.Invoke(null, o);
            return true;
        }

        private static void ReadListenfd(Socket listenfd)
        {
            Console.WriteLine("Accept");
            Socket clientfd = listenfd.Accept();
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd, state);
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
                //string clientHashCode = clientfd.GetHashCode().ToString();
                //string recvStr = string.Format("userId{0}: {1}", clientHashCode, str);
                Console.WriteLine(str);
                byte[] sendByte = Encoding.Default.GetBytes(str);
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

        public static void Send(ClientState cs, string sendStr)
        {
            byte[] sendByte = Encoding.Default.GetBytes(sendStr);
            cs.socket.Send(sendByte);
        }
    }
