using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class TankColor : MonoBehaviour
{
    [SerializeField] private List<Renderer> neons;
    [SerializeField] private Color masterColor, clientColor;
    private PhotonView photonView;

    private void Start() {
        photonView = GetComponent<PhotonView>();

        if(SceneManager.GetActiveScene().name == "LocalArena") {
            if(gameObject.name == "Player 1") {
                SetColors(masterColor);
            }
            else {
                SetColors(clientColor);
            }
        }
        else {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("SetColors", RpcTarget.All, masterColor);
            }
            else {
                photonView.RPC("SetColors", RpcTarget.All, clientColor);
            }
        }
    }

    [PunRPC]
    public void SetColors(Color color) {
        foreach(Renderer neon in neons) {
            neon.material.color = color;
            neon.material.SetColor("_EmissionColor", color);

        }
    }
}
