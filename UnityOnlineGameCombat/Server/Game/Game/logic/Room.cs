using System;
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

   public bool AddPlayer(string id)
   {
      // 获取玩家
      Player player = PlayerManager.GetPlayer(id);
      if (player== null)
      {
         Console.WriteLine("room AddPlayer fail player is null");
         return false;
      }

      if (playerIds.Count >= maxPlayer)
      {
         Console.WriteLine("room AddPlayer fail, reach maxPlayer");
         return false;
      }

      if (status != Status.PREPARE)
      {
         Console.WriteLine("room AddPlayer fail, not PREPARE");
         return false;
      }

      if (playerIds.ContainsKey(id))
      {
         Console.WriteLine("room AddPlayer fail, already in this room");
         return false;
      }

      playerIds[id] = true;
      // 设置玩家数据
      player.camp = SwitchCamp();
      player.roomId = this.id;
      if (owerId == "")
      {
         owerId = player.id;
      }

      Broadcast(ToMsg());
      return true;
   }

   public bool RemovePlayer(string id)
   {
      Player player = PlayerManager.GetPlayer(id);
      if (player== null)
      {
         Console.WriteLine("room AddPlayer fail player is null");
         return false;
      }
      if (!playerIds.ContainsKey(id))
      {
         Console.WriteLine("room AddPlayer fail, already in this room");
         return false;
      }

      playerIds.Remove(id);
      // 设置玩家数据
      player.camp = 0;
      player.roomId = -1;
      // 设置房主
      if (isOwner(player))
      {
         owerId = SwitchOwner();
      }

      if (playerIds.Count == 0)
      {
         RoomManager.RemoveRoom(this.id);
      }
      Broadcast(ToMsg());
      return true;
   }

   private bool isOwner(Player player)
   {
      return player.id == owerId;
   }

   private string SwitchOwner()
   {
      foreach (string id in playerIds.Keys)
      {
         return id;
      }
      // 房间没人
      return "";
   }


   private void Broadcast(MsgBase msg)
   {
      foreach (string id in playerIds.Keys)
      {
         Player player = PlayerManager.GetPlayer(id);
         player.Send(msg);
      }
   }

   public MsgBase ToMsg()
   {
      MsgGetRoomInfo msg = new MsgGetRoomInfo();
      int count = playerIds.Count;
      msg.players = new PlayerInfo[count];
      // players
      int i = 0;
      foreach (string id in playerIds.Keys)
      {
         Player player = PlayerManager.GetPlayer(id);
         PlayerInfo playerInfo = new PlayerInfo();
         playerInfo.id = player.id;
         playerInfo.camp = player.camp;
         playerInfo.win = player.data.win;
         playerInfo.lost = player.data.lost;
         playerInfo.isOwner = 0;
         if (isOwner(player))
         {
            playerInfo.isOwner = 1;
         }

         msg.players[i] = playerInfo;
         i++;
      }

      return msg;
   }

   private int SwitchCamp()
   {
      int count1 = 0;
      int count2 = 0;
      foreach (var id in playerIds.Keys)
      {
         Player player = PlayerManager.GetPlayer(id);
         if (player.camp == 1)
         {
            count1++;
         }
         if (player.camp == 2)
         {
            count2++;
         }
      }

      if (count1 <= count2)
      {
         return 1;
      }
      else
      {
         return 2;
      }
   }
}