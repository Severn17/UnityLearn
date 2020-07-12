using System;
using UnityEngine;

public class CtrlTank : BaseTank
{
    private GameObject skin;

    /// <summary>
    /// 上一次发送同步信息的时间
    /// </summary>
    private float lastSendSyncTime = 0;
    /// <summary>
    /// 同步帧率
    /// </summary>
    public static float syncInterval = 0.1f;

    new void Update()
    {
        base.Update();
        MoveUpdate();
        TurretUpdate();
        FireUpdate();
        SyncUpdate();
    }

    private void SyncUpdate()
    {
        if (Time.time - lastSendSyncTime < syncInterval)
        {
            return;
        }

        lastSendSyncTime = Time.time;
        MsgSyncTank msg = new MsgSyncTank();
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        msg.ex = transform.eulerAngles.x;
        msg.ey = transform.eulerAngles.y;
        msg.ez = transform.eulerAngles.z;
        msg.turretY = transform.localEulerAngles.y;
        NetManager.Send(msg);
    }

    private void FireUpdate()
    {
        if (IsDie())
            return;
        if (!Input.GetKey(KeyCode.Space))
        {
            return;
        }

        if (Time.time - lastFireTime < fireCd)
        {
            return;;
        }

        Bullet bullet = Fire();
        MsgFire msg = new MsgFire();
        msg.x = bullet.transform.position.x;
        msg.y = bullet.transform.position.y;
        msg.z = bullet.transform.position.z;
        msg.ex = bullet.transform.eulerAngles.x;
        msg.ey = bullet.transform.eulerAngles.y;
        msg.ez = bullet.transform.eulerAngles.z;
    }

    private void TurretUpdate()
    {
        if (IsDie())
            return;
        float axis = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            axis = -1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            axis = 1;
        }

        Vector3 le = turret.localEulerAngles;
        le.y += axis * Time.deltaTime * turretSpeed;
        turret.localEulerAngles = le;
    }

    private void MoveUpdate()
    {
        if (IsDie())
            return;
        float x = Input.GetAxis("Horizontal");
        transform.Rotate(0,x*steer*Time.deltaTime,0);
        float y = Input.GetAxis("Vertical");
        Vector3 s = y * transform.forward * speed * Time.deltaTime;
        transform.transform.position += s;
    }
}