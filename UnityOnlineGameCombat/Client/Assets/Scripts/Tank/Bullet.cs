using UnityEngine;

namespace Tank
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 100f;
        public BaskTank tank;
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
            BaskTank hitTank = collObj.GetComponent<BaskTank>();
            if (hitTank==tank)
            {
                return;
            }

            if (hitTank != null)
            {
                hitTank.Attacked(35);
            }

            GameObject explode = ResManager.LoadPrefab("fire");
            Instantiate(explode, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}

