using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgMove : MsgBase
{
    public MsgMove() { protoName = "BattleMsg"; }
    public int x = 0;
    public int y = 0;
    public int z = 0;
}


public class MsgAttack: MsgBase
{
    public MsgAttack() { protoName = "MsgAttack"; }

    public string desc = "127.0.0.1:6543";
}
