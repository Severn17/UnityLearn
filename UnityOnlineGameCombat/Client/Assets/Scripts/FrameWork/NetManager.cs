using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace FrameWork
{
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 3,
    }
    // 事件委托类型
    public delegate void EventListener(String err);
    public class NetManager
    {
        // 套接字
        static Socket socket;
        // 接收缓冲区
        static ByteArray readBuff;
        // 写入队列
        private static Queue<ByteArray> writeQueue;
        // 事件监听列表
        static Dictionary<NetEvent,EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

        static bool isConnecting = false;

        static bool isClosing = false;
        // 消息列表
        static List<MsgBase> msgList = new List<MsgBase>();
        // 消息长度
        static int msgCount = 0;
        // 每一次Update处理的消息量
        readonly static int MAX_MESSAGE_FIRE = 10;
        

        public static void AddEventListener(NetEvent netEvent, EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] += listener;
            }
            else
            {
                eventListeners[netEvent] = listener;
            }
        }

        public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent] -= listener;
            }
            // 删除
            if (eventListeners[netEvent]==null)
            {
                eventListeners.Remove(netEvent);
            }
        }

        // 分发事件
        public static void FireEvnet(NetEvent netEvent,String err)
        {
            if (eventListeners.ContainsKey(netEvent))
            {
                eventListeners[netEvent](err);
            }
        }
        
        public static void Connct(string ip,int port)
        {
            // 状态判断
            if (socket !=null && socket.Connected)
            {
                Debug.Log("Connect fail already connected!");
                return;
            }
            if (isConnecting)
            {
                Debug.Log("Connect fail isConnecting");
                return;
            }
            // 初始化成员
            InitState();
            // 参数
            socket.NoDelay = true;
            // Connect
            isConnecting = true;
            socket.BeginConnect(ip, port, ConnectCallBack, socket);
        }

        private static void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndAccept(ar);
                Debug.Log("Connect fail Succ ");
                FireEvnet(NetEvent.ConnectSucc, "");
                isConnecting = false;
                //开始接收
                socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
            }
            catch (SocketException e)
            {

                Debug.Log("Connect fail " + e.ToString());
                FireEvnet(NetEvent.ConnectFail, "");
                isConnecting = false;
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                    return;
                }
                readBuff.writeIdx += count;
                OnReceiveData();
                //继续接收数据
                if (readBuff.remain < 8)
                {
                    readBuff.MoveBytes();
                    readBuff.ReSize(readBuff.length * 2);
                }
                socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
            }
            catch (Exception e)
            {
                Debug.Log("Socket Receive fail" + e.ToString());
            }
        }

        private static void OnReceiveData()
        {
            //消息长度
            if (readBuff.length <= 2)
            {
                return;
            }
            //获取消息体长度
            int readIdx = readBuff.readIdx;
            byte[] bytes = readBuff.bytes;
            Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
            if (readBuff.length < bodyLength)
            {
                return;
            }
            readBuff.readIdx += 2;
            //解析协议名
            int nameCount = 0;
            string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
            if (protoName == "")
            {
                Debug.Log("OnReceiveData MsgBase.DecodeName fail");
                return;
            }
            readBuff.readIdx += nameCount;
            //解析协议体
            int bodyCount = 0;
            MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();
            //添加到消息队列
            lock (msgList)
            {
                msgList.Add(msgBase);
            }
            msgCount++;
            //继续读取消息
            if (readBuff.length > 2)
            {
                OnReceiveData();
            }
        }

        public static void Send(MsgBase msg)
        {
            if (socket == null || !socket.Connected)
            {
                return;
            }
            if (isConnecting)
            {
                return;
            }
            if (isClosing)
            {
                return;
            }
            byte[] nameBytes = MsgBase.EncodeName(msg);
            byte[] bodyBytes = MsgBase.Encode(msg);
            int len = nameBytes.Length + bodyBytes.Length;
            byte[] sendBytes = new byte[2 + len];
            //组装长度
            sendBytes[0] = (byte)(len % 256);
            sendBytes[1] = (byte)(len / 256);
            //组装消息体
            Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
            Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);
            //写入队列
            ByteArray ba = new ByteArray(sendBytes);
            int count = 0;
            lock (writeQueue)
            {
                writeQueue.Enqueue(ba);
                count = writeQueue.Count;
            }
            //send
            if (count == 1)
            {
                socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (socket == null || !socket.Connected)
            {
                return;
            }
            //EndSend
            int count = socket.EndSend(ar);
            //获取写入队列第一条数据
            ByteArray ba;
            lock (writeQueue)
            {
                ba = writeQueue.First();
            }
            // 完整发送
            ba.readIdx += count;
            if (ba.length == 0)
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();
                    ba = writeQueue.First();
                }
            }
            //继续发送
            if (ba!=null)
            {
                socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallback, socket);
            }
            else if (isClosing)
            {
                socket.Close();
            }
        }

        /// <summary>
        /// 初始化状态
        /// </summary>
        private static void InitState()
        {
            // Socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // 缓冲区
            readBuff = new ByteArray();
            // 写入队列
            writeQueue = new Queue<ByteArray>();
            // 是否正在连接
            isConnecting = true;
        }

        public static void Close()
        {
            //状态判断
            if (socket == null || !socket.Connected)
            {
                return;
            }
            if (isConnecting)
            {
                return;
            }
            // 还有数据
            if (writeQueue.Count > 0)
            {
                isClosing = true;
            }
            else
            {
                socket.Close();
                FireEvnet(NetEvent.Close, "");
            }
        }
        public void OnCloseClick()
        {
            NetManager.Close();
        }

    }
}