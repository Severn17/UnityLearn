using System;
using UnityEngine;

public class TimeManager: MonoBehaviour
{
   private float tick;
   private Action<float> func;
   private bool isRun = false;
   public  void Init()
   {
      tick = 0;
      isRun = false;
   }

   public void AddTime(float time,Action<float> func)
   {
      tick = time;
      this.func = func;
      isRun = true;
   }

   public void StopTime()
   {
      isRun = false;
   }

   private void Update()
   {
      if (isRun)
      {
         tick -= Time.deltaTime;
         func(tick);
      }
   }
   
}