using UnityEngine;

namespace Network
{
    public class RoomItem : MonoBehaviour
    {
        public string roomName;
        
        public void SetRoomName(string roomName)
        {
            this.roomName = roomName;
        }
        
        public void OnClickRoomItem()
        {
            NetworkManager.instance.JoinRoomUsingCode(roomName);
        }
        
    }
}