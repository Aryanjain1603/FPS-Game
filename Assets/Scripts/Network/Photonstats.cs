// using ExitGames.Client.Photon;
// using Photon.Pun;
// using Photon.Realtime;
// using UnityEngine;
//
// public class PhotonNetworkStats : MonoBehaviour
// {
//     private LoadBalancingPeer peer;
//
//     private float logInterval = 1f;
//     private float logTimer = 0f;
//
//     void Start()
//     {
//         peer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
//         peer.TrafficStatsEnabled = true;
//         peer.TrafficStatsReset(); // Optional: reset at room join
//     }
//
//     void Update()
//     {
//         logTimer += Time.deltaTime;
//         if (logTimer >= logInterval)
//         {
//             LogStats();
//             logTimer = 0f;
//         }
//     }
//
//     void LogStats()
//     {
//         if (peer == null) return;
//
//         int rtt = peer.RoundTripTime;
//         int rttVar = peer.RoundTripTimeVariance;
//         int ping = PhotonNetwork.GetPing();
//
//         TrafficStatsGameLevel ts = peer.TrafficStatsGameLevel;
//
//         Debug.Log(
//             $"[PhotonStats] Ping: {ping}ms | RTT: {rtt}ms (±{rttVar})\n" +
//             $"- Sent Ops: {ts.OperationCount} | Received Events: {ts.EventCount}\n" +
//             $"- Longest Send Delay: {ts.LongestDeltaBetweenSending}ms\n" +
//             $"- Longest Dispatch Delay: {ts.LongestDeltaBetweenDispatching}ms\n" +
//             $"- Longest Event Callback: {ts.LongestEventCallback}ms (Code: {ts.LongestEventCallbackCode})\n" +
//             $"- Longest Response Callback: {ts.LongestOpResponseCallback}ms (OpCode: {ts.LongestOpResponseCallbackOpCode})\n" +
//             $"- Send Calls: {ts.SendOutgoingCommandsCalls}, Dispatch Calls: {ts.DispatchIncomingCommandsCalls}"
//         );
//     }
//
//
// }
