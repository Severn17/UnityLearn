using System.Net.Sockets;

public class ClientState
{
    public Socket socket;
    public ByteArray readBuff = new ByteArray();
}
