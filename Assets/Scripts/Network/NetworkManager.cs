using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager :
        MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;

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
            //connect to photon cloud
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("connected to photon master service");
            PhotonNetwork.JoinLobby();
        }
        
        public override void OnJoinedLobby()
        {
            Debug.Log("joined lobby");
            // PhotonNetwork.JoinRandomRoom();
        }

        public void CreateRoom(string roomName)
        {          
            roomName = "Room";
            RoomOptions option = new RoomOptions();
            option.MaxPlayers = 4;
            PhotonNetwork.CreateRoom(roomName, option, TypedLobby.Default);
        }
        
        public void JoinRoom(string roomName)
        {
            roomName = "Room";
            PhotonNetwork.JoinRoom(roomName);
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
            PhotonNetwork.LoadLevel("Demo");

            // Subscribe to scene load event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Only spawn player if we're in the game scene
            if (scene.name == "Demo")
            {
                PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            }

            // Unsubscribe after spawning
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        
    }
    