namespace Game
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            MsgMove msgMove = new MsgMove();
            msgMove.x = 100;
            msgMove.y = -20;
            //取得发送的字符串
            byte[] bytes = MsgBase.Encode(msgMove);
            //接收解码
            string s = System.Text.Encoding.UTF8.GetString(bytes);
            System.Console.WriteLine(s);
        }
    }
}