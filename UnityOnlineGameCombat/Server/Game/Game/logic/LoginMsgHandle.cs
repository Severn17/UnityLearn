﻿public partial class MsgHandler
{
    public static void MsgRegister(ClientState c,MsgBase msgBase)
    {
        MsgRegister msg = (MsgRegister) msgBase;
        if (DbManager.Register(msg.id,msg.pw))
        {
            DbManager.CreatePlayer(msg.id);
            msg.result = 0;
        }
        else
        {
            msg.result = 1;
        }
        NetManager.Send(c,msg);
    }
}