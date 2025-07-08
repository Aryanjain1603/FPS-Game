using TMPro;
using UnityEngine;

namespace Network
{
    public class RoomItem : MonoBehaviour
    {
        public TextMeshProUGUI roomName;
        
        public void SetRoomName(string roomName)
        {
            this.roomName.text = roomName;
        }
        
        public void OnClickRoomItem()
        {
            NetworkManager.instance.JoinRoomUsingCode(roomName.text);
        }
        
    }
}