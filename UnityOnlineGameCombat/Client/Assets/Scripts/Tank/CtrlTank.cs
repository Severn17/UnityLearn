using System;
using UnityEngine;

namespace Tank
{
    public class CtrlTank : BaskTank
    {
        private GameObject skin;

        new void Update()
        {
            base.Update();
            MoveUpdate();
            TurretUpdate();
            FireUpdate();
        }

        private void FireUpdate()
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                return;
            }

            if (Time.time - lastFireTime < fireCd)
            {
                return;;
            }

            Fire();
        }

        private void TurretUpdate()
        {
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
            float x = Input.GetAxis("Horizontal");
            transform.Rotate(0,x*steer*Time.deltaTime,0);
            float y = Input.GetAxis("Vertical");
            Vector3 s = y * transform.forward * speed * Time.deltaTime;
            transform.transform.position += s;
        }
    }
}