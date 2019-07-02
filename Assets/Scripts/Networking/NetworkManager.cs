using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks {
    [SerializeField] private List<GameObject> enableOnConnection;
    [SerializeField] private TextMeshProUGUI connectText;
    [SerializeField] private Color connectedColor, disconnectedColor;
    [SerializeField] private TextMeshProUGUI lobbyPlayersText;

    void Start() {
        //PhotonNetwork.ConnectUsingSettings();
    }

    private void Update() {
        if(PhotonNetwork.CountOfPlayersOnMaster < 2) {
            lobbyPlayersText.text = PhotonNetwork.CountOfPlayersOnMaster + " Player in lobby";
        }
        else {
            lobbyPlayersText.text = PhotonNetwork.CountOfPlayersOnMaster + " Players in lobby";
        }
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        connectText.text = "Connected (" + PhotonNetwork.CloudRegion + ") ";
        connectText.color = connectedColor;

        foreach(GameObject gm in enableOnConnection) {
            gm.SetActive(true);
        }

        Debug.Log("Connected to " + PhotonNetwork.CloudRegion + " server");
    }

    public override void OnJoinedLobby() {
        base.OnJoinedLobby();

        Debug.Log("Lobby Joined");
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("Disconnected");
        connectText.text = "Disconnected";
        connectText.color = disconnectedColor;

        foreach (GameObject gm in enableOnConnection) {
            gm.SetActive(false);
        }
    }
}
