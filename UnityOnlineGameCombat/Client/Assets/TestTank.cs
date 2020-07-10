using System.Collections;
using System.Collections.Generic;
using Tank;
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
    }

    
}
