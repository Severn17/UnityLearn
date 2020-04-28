using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Echo2 : MonoBehaviour
{
    // 定义套接字
    private Socket socket;

    public InputField inputfield;
    public Text text;

    private byte[] readBuff = new byte[1024];
    private string recvStr = "";

    public void Connection()
    {
        // socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // Connect
        socket.BeginConnect("127.0.0.1",8888,ConnectCallBack,socket);
    }
    
    // Connect回调 
    private void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket _socket = (Socket) ar.AsyncState;
            _socket.EndConnect(ar);
            Debug.Log("Socket Connect Success");
            socket.BeginReceive(readBuff, 0, 1024,0, ReceiveCallBack, socket);
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Connect fail" + e.ToString());
        }
    }

    public void Send()
    {
        // Send
        string sendStr = inputfield.text;
        byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
        socket.BeginSend(sendBytes,0,sendBytes.Length,0,SendCallBack,socket);
    }

    private void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket _socket = (Socket) ar.AsyncState;
            int count = _socket.EndSend(ar);
            Debug.Log("Socket Connect Success"+ count);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket _socket = (Socket) ar.AsyncState;
            int count = _socket.EndReceive(ar);
            string s = Encoding.Default.GetString(readBuff, 0, count);
            recvStr = s + "\n" + recvStr;
            _socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, _socket);
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Connect fail" + e.ToString());

        }
    }

    private void Update()
    {
        text.text = recvStr;
    }
}
