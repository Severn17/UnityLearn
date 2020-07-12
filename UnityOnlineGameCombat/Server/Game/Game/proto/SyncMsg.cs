public class MsgSyncTank : MsgBase
{
    public MsgSyncTank (){ protoName = "MsgSyncTank";}
    // 位置 旋转 炮塔旋转
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    public float ex = 0f;
    public float ey = 0f;
    public float ez = 0f;
    public float turretY = 0f;
    public string id = ""; // 坦克id
}

public class MsgFire : MsgBase
{
    public MsgFire (){ protoName = "MsgFire";}
    // 位置 旋转 炮塔旋转
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    public float ex = 0f;
    public float ey = 0f;
    public float ez = 0f;
    public float turretY = 0f;
    public string id = ""; // 坦克id
}

public class MsgHit : MsgBase
{
    public MsgHit (){ protoName = "MsgHit";}
    // 击中
    public string targetId = "";
    // 位置 旋转 炮塔旋转
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;
    
    public string id = ""; // 坦克id
    public int hp = 0;     // 击中坦克血量
    public int damage = 0; //受到的伤害
}