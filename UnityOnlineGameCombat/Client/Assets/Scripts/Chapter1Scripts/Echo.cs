using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Echo : MonoBehaviour
{
    // 定义套接字
    private Socket socket;

    public InputField inputfield;
    public Text text;

    public void Connection()
    {
        // socket
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1",8888);
    }

    public void Send()
    {
        // Send
        string sendStr = inputfield.text;
        byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
        // Recv
        byte[] readBuff = new byte[1024];
        int count = socket.Receive(readBuff);
        string recvStr = Encoding.Default.GetString(readBuff, 0, count);
        text.text = recvStr;
        // Close
        socket.Close();
    }
}
