using System;

namespace Game
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //MsgMove msgMove = new MsgMove();
            //msgMove.x = 100;
            //msgMove.y = -20;
            ////取得发送的字符串
            //byte[] bytes = MsgBase.Encode(msgMove);
            ////接收解码
            //string s = System.Text.Encoding.UTF8.GetString(bytes);
            //System.Console.WriteLine(s);
            if (!DbManager.Connect("game","127.0.0.1",3306,"root",""))
            {
                return;
            }
            // 测试
            // if (DbManager.Register("lpy","123456"))
            // {
            //     Console.WriteLine("注册成功");
            // }
            // if (DbManager.CreatePlayer("lpy"))
            // {
            //     Console.WriteLine("创建成功");
            // }
            DbManager.CreatePlayer("aglab");
            PlayerData pd = DbManager.GetPlayerData("aglab");
            pd.coin = 256;
            DbManager.UpdatePlayerData("aglab", pd);
            NetManager.StartLoop(888);
        }
    }
}