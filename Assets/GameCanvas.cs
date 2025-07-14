using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class GameCanvas : MonoBehaviour
{
    [SerializeField] Button LeaveLobbyButton;
    [SerializeField] private GameObject leavePanel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leavePanel.SetActive(false);
        LeaveLobbyButton.onClick.AddListener(OnLeaveLobbyButtonClicked);
    }

    private void OnLeaveLobbyButtonClicked()
    {
        NetworkManager.instance.OnLeaveLobbyButtonClicked();
    }
}
