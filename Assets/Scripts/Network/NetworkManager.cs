using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // ──────────────────────────────────────────────
    #region Inspector / Fields
    public static NetworkManager instance;

    [Header("UI")]
    [SerializeField] private TMP_InputField nickNameInputField;
    [SerializeField] private Button startButton;
    public Canvas networkCanvas;
    public GameObject reconnectingPanel;
    public GameObject intiPanel;
    public GameObject networkPanel;

    [Header("Gameplay")]
    public List<Transform> spawnPoints = new List<Transform>();
    public GameObject playerPrefab;
    #endregion
    // ──────────────────────────────────────────────
    private string _nickName = "Player";
    private string _cachedRoomName;          // ← remember last room for Rejoin
    private bool _isConnecting;

    // ──────────────────────────────────────────────
    #region Unity Lifecycle
    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.KeepAliveInBackground = 60f;

        // If already connected (e.g., returning from a sub‑scene)
        if (PhotonNetwork.IsConnected)
        {
            intiPanel.SetActive(false);
            networkPanel.SetActive(true);
        }
    }
    #endregion
    // ──────────────────────────────────────────────
    #region Connect / Lobby
    public void ConnectToServer()                     // bound to Start button
    {
        if (_isConnecting) return;

        _nickName = nickNameInputField.text.Trim();
        if (string.IsNullOrEmpty(_nickName)) _nickName = "Player" + Random.Range(1000, 9999);

        PhotonNetwork.NickName = _nickName;           // ← SET NICKNAME **before** connect
        _isConnecting = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master");

        reconnectingPanel.SetActive(false);
        _isConnecting = false;

        // ── reconnect flow ────────────────────────
        if (!string.IsNullOrEmpty(_cachedRoomName))
        {
            Debug.Log("Attempting RejoinRoom: " + _cachedRoomName);
            PhotonNetwork.RejoinRoom(_cachedRoomName);   // will fail gracefully if room gone
            _cachedRoomName = null;                      // clear cache either way
            return;
        }

        PhotonNetwork.JoinLobby();                       // fresh connect
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        intiPanel.SetActive(false);
        networkPanel.SetActive(true);
    }
    #endregion
    // ──────────────────────────────────────────────
    #region Room Create / Join
    public void CreateRoom(string roomName)
    {
        if (!PhotonNetwork.InLobby)
        {
            Debug.LogWarning("Not yet in lobby. Wait for OnJoinedLobby.");
            return;
        }

        var options = new RoomOptions
        {
            MaxPlayers   = 4,
            PlayerTtl    = 60_000,   // keep player for 60 s
            EmptyRoomTtl = 30_000
        };
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
    }

    public void JoinRoomUsingCode(string roomName)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("Photon not ready — delaying join");
            StartCoroutine(JoinRoomWhenReady(roomName));
            return;
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    private IEnumerator JoinRoomWhenReady(string roomName)
    {
        while (!PhotonNetwork.IsConnectedAndReady) yield return null;
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion
    // ──────────────────────────────────────────────
    #region Callbacks inside Room
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        _cachedRoomName = PhotonNetwork.CurrentRoom.Name; // remember for rejoin
        UIManager.instance.NetworkCanvasVisibility(false);

        PhotonNetwork.LoadLevel("BattleScene");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
        PhotonNetwork.LoadLevel("Network");
        SceneManager.sceneLoaded += OnSceneLoaded;
        PhotonNetwork.JoinLobby();
    }
    #endregion
    // ──────────────────────────────────────────────
    #region Reconnect / Disconnect
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected: {cause}");

        // cache current room for potential rejoin
        if (PhotonNetwork.CurrentRoom != null)
            _cachedRoomName = PhotonNetwork.CurrentRoom.Name;

        if (cause == DisconnectCause.Exception ||
            cause == DisconnectCause.ClientTimeout ||
            cause == DisconnectCause.ServerTimeout ||
            cause == DisconnectCause.ExceptionOnConnect)
        {
            reconnectingPanel.SetActive(true);
            StartCoroutine(TryReconnectRoutine());
        }
        else
        {
            // non‑recoverable — kick to menu
            SceneManager.LoadScene("Network");
        }
    }

    private IEnumerator TryReconnectRoutine()
    {
        const int maxAttempts = 5;
        int attempt = 0;

        while (!PhotonNetwork.IsConnected && attempt < maxAttempts)
        {
            attempt++;
            Debug.Log($"Reconnect attempt {attempt}/{maxAttempts}");
            PhotonNetwork.Reconnect();
            yield return new WaitForSeconds(3f);
        }

        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("All reconnect attempts failed.");
            reconnectingPanel.SetActive(false);
            SceneManager.LoadScene("Network");
        }
    }
    #endregion
    // ──────────────────────────────────────────────
    #region Scene Spawn
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            PhotonNetwork.Instantiate(playerPrefab.name, GetSpawnPoint(), Quaternion.identity);
            UIManager.instance.GameSceneCanvasVisibility(true);
        }

        if (scene.name == "Network")
        {
            intiPanel.SetActive(false);
            networkPanel.SetActive(true);
            networkCanvas.enabled = true;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private Vector3 GetSpawnPoint()
    {
        int idx = Random.Range(0, spawnPoints.Count);
        return spawnPoints[idx].position;
    }
    public Vector3 GetPosition()
    {
        int random = Random.Range(0, spawnPoints.Count);
        return  spawnPoints[random].position;
    }
    public void OnLeaveLobbyButtonClicked()
    {
        // PhotonNetwork.LeaveLobby();
        PhotonNetwork.LeaveRoom();
        Debug.Log("LeaveRoomoButtonClicked");
    }



    #endregion
}
