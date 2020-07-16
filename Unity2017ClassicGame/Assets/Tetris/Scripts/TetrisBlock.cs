using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    private float previousTime;
    public float fallTime = 0.8f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            transform.position += new Vector3(-1,0,0);
        }
        else if(Input.GetMouseButtonDown(1))
        {
            transform.position += new Vector3(1,0,0);
        }

        if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10: fallTime))
        {
            transform.position += new Vector3(0,-1,0);
            previousTime = Time.time;
        }
    }
}
