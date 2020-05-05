using System;

public static class MsgHandler
{
    public static void MsgEnter(ClientState c,string msgArgs)
    {
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        c.hp = 100;
        c.x = x;
        c.y = y;
        c.z = z;
        c.eulY = eulY;
        //广播
        string sendStr = "Enter|" + msgArgs;
        foreach (ClientState cs in Program.clients.Values)
        {
            Program.Send(cs, sendStr);
        }
    }
    public static void MsgList(ClientState c,string msgArgs)
    {
        string sendStr = "List|";
        foreach (ClientState cs in Program.clients.Values)
        {
            sendStr += cs.socket.RemoteEndPoint.ToString() + ",";
            sendStr += cs.x.ToString() + ",";
            sendStr += cs.y.ToString() + ",";
            sendStr += cs.z.ToString() + ",";
            sendStr += cs.eulY.ToString() + ",";
            sendStr += cs.hp.ToString() + ",";
        }
        Program.Send(c,sendStr);
        Console.WriteLine("当前玩家总数" + Program.clients.Values.Count);
    }

    public static void MsgMove(ClientState c, string msgArgs)
    {
        string[] split = msgArgs.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        c.x = x;
        c.y = y;
        c.z = z;
        string sendStr = "Move|" + msgArgs;
        foreach (ClientState cs in Program.clients.Values)
        {
            Program.Send(cs, sendStr);
        }
    }
    public static void MsgAttack(ClientState c, string msgArgs)
    {
        string sendStr = "Attack|" + msgArgs;
        foreach (ClientState cs in Program.clients.Values)
        {
            Program.Send(cs, sendStr);
        }
    }
    public static void MsgHit(ClientState c, string msgArgs)
    {
        string[] split = msgArgs.Split(',');
        string attDesc = split[0];
        string hitDesc = split[1];
        // 找出被攻击的角色
        ClientState hitCS = null;
        foreach (ClientState cs in Program.clients.Values)
        {
            if (cs.socket.RemoteEndPoint.ToString() == hitDesc)
            {
                hitCS = cs;
            }
        }

        if (hitCS == null)
        {
            return;
        }
        //扣血
        hitCS.hp -= 25;
        //死亡
        if (hitCS.hp <= 0)
        {
            string sendStr = "Die|" + hitCS.socket.RemoteEndPoint.ToString();
            foreach (ClientState cs in Program.clients.Values)
            {
                Program.Send(cs, sendStr);
            }
        }
    }
}
