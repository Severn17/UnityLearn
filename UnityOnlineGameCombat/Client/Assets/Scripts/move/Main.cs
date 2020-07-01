using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace move
{
    public class Main : MonoBehaviour
    {
        public GameObject humanPrefab;
        public BaseHuman myHuman;
        public Dictionary<string, BaseHuman> otherHumans = new Dictionary<string, BaseHuman>();

        private void Start()
        {
            NetManager.AddListener("Enter", OnEnter);
            NetManager.AddListener("List", OnList);
            NetManager.AddListener("Move", OnMove);
            NetManager.AddListener("Leave", OnLeave);
            NetManager.AddListener("Attack", OnAttack);
            NetManager.AddListener("Die", OnDie);
            NetManager.Connect("127.0.0.1", 8888);

            // 添加一个角色
            GameObject obj = (GameObject) Instantiate(humanPrefab);
            float x = Random.Range(518, 535);
            float z = Random.Range(303, 324);
            obj.transform.position = new Vector3(x, 0, z);
            myHuman = obj.AddComponent<CtrlHuman>();
            myHuman.desc = NetManager.GetDesc();

            //发送协议
            Vector3 pos = myHuman.transform.position;
            Vector3 eul = myHuman.transform.eulerAngles;
            string sendStr = "Enter|";
            sendStr += NetManager.GetDesc() + ",";
            sendStr += pos.x + ",";
            sendStr += pos.y + ",";
            sendStr += pos.z + ",";
            sendStr += eul.y + ",";
            NetManager.Send(sendStr);
            //请求玩家列表
            NetManager.Send("List|");
        }

        private void OnDie(string msgArgs)
        {
            Debug.Log("OnDie" + msgArgs);
            string[] split = msgArgs.Split(',');
            string hitDese = split[0];
            // 自己死了
            if (hitDese == myHuman.desc)
            {
                Debug.Log("Game Over");
            }

            // 死了
            if (!otherHumans.ContainsKey(hitDese))
            {
                return;
            }

            SyncHuman h = (SyncHuman) otherHumans[hitDese];
            h.gameObject.SetActive(false);
        }

        private void OnAttack(string msgArgs)
        {
            Debug.Log("OnAttack" + msgArgs);
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float eulY = float.Parse(split[1]);
            if (!otherHumans.ContainsKey(desc))
            {
                return;
            }

            SyncHuman h = (SyncHuman) otherHumans[desc];
            h.SyncAttack(eulY);
        }

        private void OnList(string msgArgs)
        {
            Debug.Log("OnList" + msgArgs);
            string[] split = msgArgs.Split(',');
            int count = (split.Length - 1) / 6;
            for (int i = 0; i < count; i++)
            {
                string desc = split[i * 6 + 0];
                float x = float.Parse(split[i * 6 + 1]);
                float y = float.Parse(split[i * 6 + 2]);
                float z = float.Parse(split[i * 6 + 3]);
                float eulY = float.Parse(split[i * 6 + 4]);
                int hp = int.Parse(split[i * 6 + 5]);
                // 是自己
                if (desc == NetManager.GetDesc())
                {
                    continue;
                }

                // 添加一个角色
                GameObject obj = (GameObject) Instantiate(humanPrefab);
                obj.transform.position = new Vector3(x, y, z);
                obj.transform.eulerAngles = new Vector3(0, eulY, 0);
                BaseHuman h = obj.AddComponent<SyncHuman>();
                h.desc = desc;
                otherHumans.Add(desc, h);
            }
        }

        private void OnLeave(string msgArgs)
        {
            Debug.Log("OnLeave" + msgArgs);
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            if (!otherHumans.ContainsKey(desc))
            {
                return;
            }

            BaseHuman h = otherHumans[desc];
            Destroy(h.gameObject);
            otherHumans.Remove(desc);
        }

        private void OnMove(string msgArgs)
        {
            Debug.Log("OnMove" + msgArgs);
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            if (!otherHumans.ContainsKey(desc))
            {
                return;
            }

            BaseHuman h = otherHumans[desc];
            Vector3 targetPos = new Vector3(x, y, z);
            h.MoveTo(targetPos);
        }

        private void OnEnter(string msgArgs)
        {
            Debug.Log("OnEnter" + msgArgs);
            string[] split = msgArgs.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float eulY = float.Parse(split[4]);
            // 是自己
            if (desc == NetManager.GetDesc())
            {
                return;
                ;
            }

            // 添加一个角色
            GameObject obj = (GameObject) Instantiate(humanPrefab);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, eulY, 0);
            BaseHuman h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }
    }
}
