﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

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
        //消息分发
        MethodInfo mei =  typeof(EventHandler).GetMethod("OnTimer");
        object[] ob = {};
        mei.Invoke(null, ob);
    }

    private static void ReadClientfd(Socket clientfd)
    {
        ClientState state = clients[clientfd];
        ByteArray readBuff = state.readBuff;
        //接收
        int count = 0;
        //缓冲区不够，清除，若依旧不够，只能返回
        //缓冲区长度只有1024，单条协议超过缓冲区长度时会发生错误，根据需要调整长度
        if(readBuff.remain <=0){
            OnReceiveData(state);
            readBuff.MoveBytes();
        };
        if(readBuff.remain <=0){
            Console.WriteLine("Receive fail , maybe msg length > buff capacity");
            Close(state);
            return;
        }

        try{
            count = clientfd.Receive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0);
        }catch(SocketException ex){
            Console.WriteLine("Receive SocketException " + ex.ToString());
            Close(state);
            return;
        }
        //客户端关闭
        if(count <= 0){
            Console.WriteLine("Socket Close " + clientfd.RemoteEndPoint.ToString());
            Close(state);
            return;
        }
        //消息处理
        readBuff.writeIdx+=count;
        //处理二进制消息
        OnReceiveData(state);
        //移动缓冲区
        readBuff.CheckAndMoveBytes();
    }

    private static void Close(ClientState state)
    {
        // 消息分发
        MethodInfo mei = typeof(EventHandler).GetMethod("OnDisconnect");
        object[] obj = {state};
        mei.Invoke(null, obj);
        // 关闭
        state.socket.Close();
        clients.Remove(state.socket);
    }

    private static void OnReceiveData(ClientState state)
    {
        ByteArray readBuff = state.readBuff;
        //消息长度
        if(readBuff.length <= 2) {
            return;
        }
        Int16 bodyLength = readBuff.ReadInt16();
        //消息体
        if(readBuff.length < bodyLength){
            return;
        }
        //解析协议名
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if(protoName == ""){
            Console.WriteLine("OnReceiveData MsgBase.DecodeName fail");
            Close(state);
            return;
        }
        readBuff.readIdx += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        //分发消息
        MethodInfo mi =  typeof(MsgHandler).GetMethod(protoName);
        object[] o = {state, msgBase};
        Console.WriteLine("Receive " + protoName);
        if(mi != null){
            mi.Invoke(null, o);
        }
        else{
            Console.WriteLine("OnReceiveData Invoke fail " + protoName);
        }
        //继续读取消息
        if(readBuff.length > 2){
            OnReceiveData(state);
        }
    }

    private static void ReadListenfd(Socket s)
    {
        try
        {
            Socket clientfd = listenfd.Accept();
            Console.WriteLine("Accept " + clientfd.RemoteEndPoint.ToString());
            ClientState state = new ClientState();
            state.socket = clientfd;
            clients.Add(clientfd,state);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Accept fail" + ex.ToString());
        }
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
