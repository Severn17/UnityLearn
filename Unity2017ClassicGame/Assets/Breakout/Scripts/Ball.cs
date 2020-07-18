using System;
using UnityEngine;

namespace Breakout.Scripts
{
    public class Ball : MonoBehaviour
    {
        private Transform player;
        private Rigidbody2D rb;
        public float score;
        private void Start()
        {
            player = transform.parent;
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collider)
        {
            if (collider.gameObject.name == "Block")
            {
                Destroy(collider.gameObject);
                score += 5;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Finish"))
            {
                transform.SetParent(player);
                transform.position = player.position;
                rb.velocity = Vector2.zero;
            }
        }
    }
    
}