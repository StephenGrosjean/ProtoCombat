using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks {
    //[SerializeField] private List<RoomInfo> listRooms;
    //[SerializeField] private int maxRooms;

    void Start() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();


        Debug.Log("Connected to master");
    }
}
