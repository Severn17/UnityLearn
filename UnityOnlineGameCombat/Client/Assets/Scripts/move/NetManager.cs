using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace move
{
    public class NetManager : MonoBehaviour
    {
        // 套接字
        private static Socket socket;
        // 缓冲区
        private static byte[] readBuff = new byte[1024];
        // 委托类型
        public delegate void MsgListener(String str);
        // 监听列表
        private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
        // 消息列表
        private static List<String> msgList = new List<String>();

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="listener"></param>
        public static void AddListener(string msgName, MsgListener listener)
        {
            listeners[msgName] = listener;
        }
        /// <summary>
        /// 获取描述
        /// </summary>
        /// <returns></returns>
        public static string GetDesc()
        {
            if (socket == null) return "";
            if (!socket.Connected) return "";
            return socket.LocalEndPoint.ToString();
        }

        public static void Connect(string ip, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ip, port);
            // BeginReceive
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
        }
        /// <summary>
        /// Receive回调
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int count = socket.EndReceive(ar);
                string recvStr = Encoding.Default.GetString(readBuff, 0, count);
                msgList.Add(recvStr);
                socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
            }
            catch (SocketException e)
            {
                Debug.Log("Socket Receive fail" + e.ToString());
                throw;
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="sendStr"></param>
        public static void Send(string sendStr)
        {
            if (socket == null) return;
            if (!socket.Connected) return;
            byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallBack, socket);
        }

        private static void SendCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
            }
            catch (SocketException e)
            {
                Debug.Log("Socket Send fail" + e.ToString());
            }
        }

        public void Update()
        {
            if (msgList.Count <= 0) return;
            String msgStr = msgList[0];
            msgList.RemoveAt(0);
            string[] split = msgStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            if (listeners.ContainsKey(msgName))
            {
                listeners[msgName](msgArgs);
            }
        }
    }
}
