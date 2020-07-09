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
        }
    }
}