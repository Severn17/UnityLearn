using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTank : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject tankObj = new GameObject("myTank");
        CtrlTank ctrlTank = tankObj.AddComponent<CtrlTank>();
        ctrlTank.Init("tankPrefab");
        tankObj.AddComponent<CameraFollow>();
        
        GameObject tankObj2 = new GameObject("myTank");
        BaseTank baskTank = tankObj2.AddComponent<BaseTank>();
        baskTank.Init("tankPrefab");
        baskTank.transform.position = new Vector3(0,10,30);
    }

    
}
