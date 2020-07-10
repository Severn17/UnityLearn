using System;
using UnityEngine;

namespace Tank
{
    public class BaskTank : MonoBehaviour
    {
        private GameObject skin;
        public float steer = 20;
        public float speed = 3f;
        protected Rigidbody rigidbody;
        //炮塔旋转速度
        public float turretSpeed = 30f;
        public Transform turret;
        public Transform gun;
        public Transform firePoint;
        
        public float fireCd = 0.5f;
        public float lastFireTime = 0;
        void Start()
        {
            
        }

        public void Update()
        {
            
        }

        public virtual void Init(string path)
        {
            GameObject skinRes = ResManager.LoadPrefab(path);
            skin = (GameObject) Instantiate(skinRes);
            skin.transform.parent = this.transform;
            skin.transform.localPosition = Vector3.zero;
            skin.transform.localEulerAngles = Vector3.zero;
            //物理
            rigidbody = gameObject.AddComponent<Rigidbody>();
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0,2.5f,1.47f);
            boxCollider.size = new Vector3(7, 5, 12);
            //炮塔炮管
            turret = skin.transform.Find("Turret");
            gun = turret.transform.Find("Gun");
            firePoint = gun.transform.Find("FirePoint");
        }

        public Bullet Fire()
        {
            if (IsDie())
            {
                return null;
            }
            GameObject bulletObj = new GameObject("bullet");
            Bullet bullet = bulletObj.AddComponent<Bullet>();
            bullet.Init();
            bullet.tank = this;
            //位置
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            //更新时间
            lastFireTime = Time.time;
            return bullet;
        }

        private bool IsDie()
        {
            return false;
        }
    }
}