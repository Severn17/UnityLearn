using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static Dictionary<string, BaseTank> tanks = new Dictionary<string, BaseTank>();

    public static void Init()
    {
        //添加监听
        NetManager.AddMsgListener("MsgEnterBattle", OnMsgEnterBattle);
        NetManager.AddMsgListener("MsgBattleResult", OnMsgBattleResult);
        NetManager.AddMsgListener("MsgLeaveBattle", OnMsgLeaveBattle);

        NetManager.AddMsgListener("MsgSyncTank", OnMsgSyncTank);
        NetManager.AddMsgListener("MsgFire", OnMsgFire);
        NetManager.AddMsgListener("MsgHit", OnMsgHit);
    }

    private static void OnMsgSyncTank(MsgBase msgBase)
    {
        MsgSyncTank msg = (MsgSyncTank) msgBase;
        if (msg.id == GameMain.id)
        {
            return;
        }

        // 找坦克
        SyncTank tank = (SyncTank) GetTank(msg.id);
        if (tank == null)
        {
            return;
        }

        // 同步
        tank.SyncPos(msg);
    }

    private static void OnMsgFire(MsgBase msgBase)
    {
        MsgFire msg = (MsgFire) msgBase;
        if (msg.id == GameMain.id)
        {
            return;
        }

        // 找坦克
        SyncTank tank = (SyncTank) GetTank(msg.id);
        if (tank == null)
        {
            return;
        }

        // 同步
        tank.SyncFire(msg);
    }

    private static void OnMsgHit(MsgBase msgBase)
    {
        MsgHit msg = (MsgHit) msgBase;
        // 找坦克
        SyncTank tank = (SyncTank) GetTank(msg.id);
        if (tank == null)
        {
            return;
        }

        // 同步
        tank.Attacked(msg.damage);
    }

    public static void AddTank(string id, BaseTank tank)
    {
        tanks[id] = tank;
    }

    public static void RemoveTank(string id)
    {
        tanks.Remove(id);
    }

    public static BaseTank GetTank(string id)
    {
        if (tanks.ContainsKey(id))
        {
            return tanks[id];
        }

        return null;
    }

    public static BaseTank GetCtrlTank()
    {
        return GetTank(GameMain.id);
    }

    public static void Reset()
    {
        foreach (BaseTank tank in tanks.Values)
        {
            Destroy(tank.gameObject);
        }

        tanks.Clear();
    }


    private static void OnMsgBattleResult(MsgBase msgBase)
    {
        MsgBattleResult msg = (MsgBattleResult) msgBase;
        bool isWin = false;
        BaseTank tank = GetCtrlTank();
        if (tank != null && tank.camp == msg.winCamp)
        {
            isWin = true;
        }

        PanelManager.Open<ResultPanel>(isWin);
    }

    private static void OnMsgLeaveBattle(MsgBase msgBase)
    {
        MsgLeaveBattle msg = (MsgLeaveBattle) msgBase;
        // 查找坦克
        BaseTank tank = GetTank(msg.id);
        if (tank == null)
        {
            return;
        }

        RemoveTank(msg.id);
        Destroy(tank.gameObject);
    }

    public static void OnMsgEnterBattle(MsgBase msgBase)
    {
        MsgEnterBattle msg = (MsgEnterBattle) msgBase;
        EnterBattle(msg);
    }

    public static void EnterBattle(MsgEnterBattle msg)
    {
        BattleManager.Reset();
        PanelManager.Close("RoomPanel");
        PanelManager.Close("ResultPanel");
        for (int i = 0; i < msg.tanks.Length; i++)
        {
            GenerateTank(msg.tanks[i]);
        }
    }

    private static void GenerateTank(TankInfo tankInfo)
    {
        string objName = "Tank_" + tankInfo.id;
        GameObject tankObj = new GameObject(objName);
        //AddComponent
        BaseTank tank = null;
        if (tankInfo.id == GameMain.id)
        {
            tank = tankObj.AddComponent<CtrlTank>();
        }
        else
        {
            tank = tankObj.AddComponent<SyncTank>();
        }

        // camera
        if (tankInfo.id == GameMain.id)
        {
            CameraFollow cf = tankObj.AddComponent<CameraFollow>();
        }

        tank.camp = tankInfo.camp;
        tank.id = tankInfo.id;
        tank.hp = tankInfo.hp;
        // Pos
        Vector3 pos = new Vector3(tankInfo.x, tankInfo.y, tankInfo.z);
        Vector3 rot = new Vector3(tankInfo.ex, tankInfo.ey, tankInfo.ez);
        tank.transform.position = pos;
        tank.transform.eulerAngles = rot;
        //init
        if (tankInfo.camp == 1)
        {
            tank.Init("tankPrefab");
        }
        else
        {
            tank.Init("tankPrefab2");
        }

        AddTank(tankInfo.id, tank);
    }
}
