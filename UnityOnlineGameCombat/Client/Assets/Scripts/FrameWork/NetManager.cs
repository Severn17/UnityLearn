using System;
using System.Collections;
using System.Collections.Generic;
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
            }
            catch (SocketException e)
            {

                Debug.Log("Connect fail " + e.ToString());
                FireEvnet(NetEvent.ConnectFail, "");
                isConnecting = false;
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
    }
}