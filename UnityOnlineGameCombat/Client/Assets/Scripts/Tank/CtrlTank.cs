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