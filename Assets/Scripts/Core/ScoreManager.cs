using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Player;
using UnityEngine;
using UnityEngine.InputSystem.UI;


public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance;
    private Dictionary<int,int> playerScores =  new Dictionary<int,int>();
    public float roundDuration = 300f;
    private float remainingTime;
    private float storedRemainingTime;
    private bool hasRoundEnded = false;
    private bool hasRoundsStarted = false;
    public PhotonView photonView;
    
    public event Action<int,int> OnScoreChanged;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
    }

    public void StartTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            remainingTime = roundDuration;
            hasRoundEnded = false;
            hasRoundsStarted = true;
            CursorLock.CursorLockStatus(true);
        }
    }

    void Update()
    {
        if(!PhotonNetwork.IsMasterClient || hasRoundEnded)
            return;
        if (hasRoundsStarted)
        {
            remainingTime -= Time.deltaTime;
            storedRemainingTime = remainingTime;
            photonView.RPC("RPC_UpdateRoundDuration", RpcTarget.All, remainingTime);
        }
        else
            remainingTime = 0;
        if (remainingTime <= 0 && hasRoundsStarted)
        {
            hasRoundEnded = true;
            remainingTime = 0f;
            photonView.RPC(nameof(EndRound), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_UpdateRoundDuration(float _remainingTime)
    {
        remainingTime = _remainingTime;
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public void AddKill(int killerActorNumber)
    {
        // if (!PhotonNetwork.IsMasterClient) return;
        
        if(!playerScores.ContainsKey(killerActorNumber))
            playerScores[killerActorNumber] = 0;
        
        playerScores[killerActorNumber]++;
        
        
        photonView.RPC(nameof(UpdateKillCountRPC),RpcTarget.All,killerActorNumber,playerScores[killerActorNumber]);
    }

    [PunRPC]
    public void UpdateKillCountRPC(int actorNumber, int newScore)
    {
        OnScoreChanged?.Invoke(actorNumber, newScore);
        playerScores[actorNumber] =  newScore;
        
        Photon.Realtime.Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
        string nickname = player.NickName;
        
        Debug.Log($"updated Score : Player {nickname} : {newScore}");
    }

    [PunRPC]
    private void EndRound()
    {
        Debug.Log("Round Ended");
    }

    public Dictionary<int, int> GetAllScores()
    {
        return playerScores;
    }

    public int GetScore(int actorNumber)
    {
        return playerScores.TryGetValue(actorNumber, out var score) ? score : 0;

    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.Log($"OnMasterClientSwitched to {newMasterClient}");
        if (PhotonNetwork.IsMasterClient)
        {
            remainingTime = storedRemainingTime;
        }
    }
}
