using System.Collections.Generic;

public class Room
{
   public enum Status
   {
      PREPARE = 0,
      FIGHT = 1,
   }
   public int id = 0;
   public int maxPlayer = 6;
   public Dictionary<string,bool>playerIds = new Dictionary<string, bool>();
   public string owerId = "";
   public Status status = Status.PREPARE;
}