using System.Collections.Generic;
using UnityEngine;

namespace Tank
{
    public class BattleManager : MonoBehaviour
    {
        public static Dictionary<string,BaskTank> tanks = new Dictionary<string, BaskTank>();

        public static void Init()
        {
            NetManager.AddMsgListener("MsgEnterBattle",OnMsgEnterBattle);
            NetManager.AddMsgListener("MsgBattleResult",OnMsgBattleResult);
            NetManager.AddMsgListener("MsgLeaveBattle",OnMsgLeaveBattle);
        }

        public static void AddTank(string id, BaskTank tank)
        {
            tanks[id] = tank;
        }

        public static void RemoveTank(string id)
        {
            tanks.Remove(id);
        }

        public static BaskTank GetTank(string id)
        {
            if (tanks.ContainsKey(id))
            {
                return tanks[id];
            }

            return null;
        }

        public static BaskTank GetCtrlTank()
        {
            return GetTank(GameMain.id);
        }

        public static void Reset()
        {
            foreach (BaskTank tank in tanks.Values)
            {
                Destroy(tank.gameObject);
            }
            tanks.Clear();
        }
        

        private static void OnMsgBattleResult(MsgBase msgBase)
        {
            MsgBattleResult msg = (MsgBattleResult) msgBase;
            bool isWin = false;
            BaskTank tank = GetCtrlTank();
            if (tank !=null && tank.camp == msg.winCamp)
            {
                isWin = true;
            }
            PanelManager.Open<ResultPanel>(isWin);
        }

        private static void OnMsgLeaveBattle(MsgBase msgBase)
        {
            MsgLeaveBattle msg = (MsgLeaveBattle) msgBase;
            // 查找坦克
            BaskTank tank = GetTank(msg.id);
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
            BaskTank tank = null;
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
            Vector3 pos = new Vector3(tankInfo.x,tankInfo.y,tankInfo.z);
            Vector3 rot = new Vector3(tankInfo.ex,tankInfo.ey,tankInfo.ez);
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
}