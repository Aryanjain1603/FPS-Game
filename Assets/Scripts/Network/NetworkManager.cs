using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NetworkManager :
        MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    
    [SerializeField] private TMP_InputField NickNameInputField;
    [SerializeField] private Button startButton;
    private string nickName;
    
    
    public List<Transform> spawnPoints = new List<Transform>();

    public GameObject intiPanel;
    public GameObject networkPanel;
    
    public GameObject playerPrefab;
        
        private void Awake()
        {
            if(instance == null)
            instance = this;
            
            else Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {

            if (PhotonNetwork.IsConnected)
            {
                intiPanel.SetActive(false);
                networkPanel.SetActive(true);
                
            }
        }

        //Called by start button
        public void ConnectToServer()
        {
            if (NickNameInputField.text.Length >= 0)
            {
                nickName = NickNameInputField.text;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        public override void OnConnectedToMaster()
        {
            Debug.Log("connected to photon master service");
            PhotonNetwork.JoinLobby();
        }
        
        public override void OnJoinedLobby()
        {
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = true;

            Debug.Log("joined lobby");
            intiPanel.SetActive(false);
            networkPanel.SetActive(true);
            // PhotonNetwork.JoinRandomRoom();
        }

        public void CreateRoom(string _roomName)
        {
            
            RoomOptions option = new RoomOptions();
            option.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(_roomName, option, TypedLobby.Default);
            
                
        }
        public void JoinRoomUsingCode(string _roomName)
        {
            PhotonNetwork.JoinRoom(_roomName);
        }
        
        public override void OnJoinedRoom()
        {
            UIManager.instance.NetworkCanvasVisibility(false);
            Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
            PhotonNetwork.LoadLevel("BattleScene");

            // Subscribe to scene load event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        

        public void OnLeaveLobbyButtonClicked()
        {
            // PhotonNetwork.LeaveLobby();
            PhotonNetwork.LeaveRoom();
            Debug.Log("LeaveRoomoButtonClicked");
        }

        public override void OnLeftRoom()
        {
            Debug.Log("LeftRoom callback called");
            PhotonNetwork.LoadLevel("Network");
            PhotonNetwork.JoinLobby();


        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Only spawn player if we're in the game scene
            if (scene.name == "BattleScene")
            {
                
                PhotonNetwork.Instantiate(playerPrefab.name, GetPosition(), Quaternion.identity);
                PhotonNetwork.LocalPlayer.NickName = nickName;
                UIManager.instance.GameSceneCanvasVisibility(true);
            }

            // Unsubscribe after spawning
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public Vector3 GetPosition()
        {
            int random = Random.Range(0, spawnPoints.Count);
            return  spawnPoints[random].position;
        }

        
    }
    