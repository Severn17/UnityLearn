using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breakout.Scripts
{
    public class BreakoutPlayer : MonoBehaviour
    {
        public float force;

        public float moveSpeed;
        public bool isStartGame;
        public GameObject ball;

        void Update()
        {
            if (!isStartGame)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    transform.DetachChildren();
                    ball.GetComponent<Rigidbody2D>().AddForce(Vector2.up * force,ForceMode2D.Impulse);
                }
            }

            float horizontalValue = Input.GetAxis("Horizontal");
            transform.Translate(horizontalValue * moveSpeed * Time.deltaTime,0,0);
        }
    }
}


