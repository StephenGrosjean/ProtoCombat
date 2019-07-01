using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks {
    [SerializeField] private List<GameObject> enableOnConnection;

    void Start() {
        //PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        foreach(GameObject gm in enableOnConnection) {
            gm.SetActive(true);
        }

        Debug.Log("Connected to " + PhotonNetwork.CloudRegion + " server");
    }

    public override void OnDisconnected(DisconnectCause cause) {
        foreach (GameObject gm in enableOnConnection) {
            gm.SetActive(false);
        }
    }
}
