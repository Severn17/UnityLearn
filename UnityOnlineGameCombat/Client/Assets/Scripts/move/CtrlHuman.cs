using UnityEngine;

namespace move
{
    public class CtrlHuman : BaseHuman
    {
        new void Start()
        {
            base.Start();
        }

        new void Update()
        {
            base.Update();
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                if (hit.collider.tag == "Terrain")
                {
                    MoveTo(hit.point);
                    string sendStr = "Move|";
                    sendStr += NetManager.GetDesc() + ",";
                    sendStr += hit.point.x + ",";
                    sendStr += hit.point.y + ",";
                    sendStr += hit.point.z + ",";
                    NetManager.Send(sendStr);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (isAttacking) return;
                if (isMoveing) return;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                transform.LookAt(hit.point);
                Attack();
                //发送协议
                string sendStr = "Attack|";
                sendStr += NetManager.GetDesc() + ",";
                sendStr += transform.eulerAngles.y + ",";
                NetManager.Send(sendStr);
                //攻击判定
                Vector3 lineEnd = transform.position + 0.5f * Vector3.up;
                Vector3 lineStart = lineEnd + 20 * transform.forward;
                if (Physics.Linecast(lineStart, lineEnd, out hit))
                {
                    GameObject hitObj = hit.collider.gameObject;
                    if (hitObj == gameObject)
                    {
                        return;
                    }

                    SyncHuman h = hitObj.GetComponent<SyncHuman>();
                    if (h == null)
                    {
                        return;
                    }

                    sendStr = "Hit|";
                    sendStr += NetManager.GetDesc() + ",";
                    sendStr += h.desc + ",";
                    NetManager.Send(sendStr);
                }
            }
        }
    }
}
