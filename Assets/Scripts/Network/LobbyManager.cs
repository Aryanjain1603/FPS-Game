using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField createRoomInputField;
    [SerializeField] private TMP_InputField joinRoomInputField;

    public void CreateRoom()
    {
        if (createRoomInputField.text != "")
        {
            NetworkManager.instance.CreateRoom(createRoomInputField.text);
        }        
    }
    public void JoinRoom()
    {
        if (joinRoomInputField.text != "")
        {
            NetworkManager.instance.JoinRoomUsingCode(joinRoomInputField.text);
        }        
    }
}
