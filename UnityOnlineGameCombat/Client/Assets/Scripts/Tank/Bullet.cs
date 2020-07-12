using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 100f;
    public BaseTank tank;
    private GameObject skin;
    private Rigidbody _rigidbody;

    // 初始化
    public void Init()
    {
        GameObject skinRes = ResManager.LoadPrefab("bulletPrefab");
        skin = (GameObject) Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;
        //物理
        _rigidbody = gameObject.AddComponent<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        // 打到坦克
        GameObject collObj = other.gameObject;
        BaseTank hitTank = collObj.GetComponent<BaseTank>();
        if (hitTank==tank)
        {
            return;
        }

        if (hitTank != null)
        {
            SendMsgHit(tank, hitTank);
            //hitTank.Attacked(35);
        }

        GameObject explode = ResManager.LoadPrefab("fire");
        Instantiate(explode, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void SendMsgHit(BaseTank tank, BaseTank hitTank)
    {
        if (hitTank == null || hitTank==null)
        {
            return;
        }
        // 不是自己发射的炮弹
        if (tank.id != GameMain.id)
        {
            return;
        }
        MsgHit msg = new MsgHit();
        msg.targetId = hitTank.id;
        msg.id = tank.id;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        NetManager.Send(msg);
    }
}

