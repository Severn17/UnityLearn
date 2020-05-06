using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

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
    }
}