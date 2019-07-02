using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks {
    [SerializeField] private List<GameObject> enableOnConnection;
    [SerializeField] private TextMeshProUGUI connectText;
    [SerializeField] private Color connectedColor, disconnectedColor;

    void Start() {
        //PhotonNetwork.ConnectUsingSettings();
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

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("Disconnected");
        connectText.text = "Disconnected";
        connectText.color = disconnectedColor;

        foreach (GameObject gm in enableOnConnection) {
            gm.SetActive(false);
        }
    }
}
