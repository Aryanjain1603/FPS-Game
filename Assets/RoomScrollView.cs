using System.Collections.Generic;
using Network;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomScrollView : MonoBehaviourPunCallbacks
{
   public GameObject roomItemPrefab;
   public Transform contentObject;
   public List<RoomItem> rooms = new List<RoomItem>();

   public override void OnRoomListUpdate(List<RoomInfo> roomList)
   {
      UpdateRoomList(roomList);
   }
   public void UpdateRoomList(List<RoomInfo> roomList)
   {
      foreach (var room in rooms)
      {
         Destroy(room.gameObject);
      }
      rooms.Clear();
      foreach (var room in roomList)
      {
         GameObject newRoom = Instantiate(roomItemPrefab,contentObject);
         RoomItem roomItem = newRoom.GetComponent<RoomItem>();
         roomItem.SetRoomName(room.Name);
         rooms.Add(roomItem);
      }
   }
}
