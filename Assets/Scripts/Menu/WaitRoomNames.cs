using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class WaitRoomNames : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI masterName, clientName;

    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

        masterName.text = PhotonNetwork.MasterClient.NickName;

        if (PhotonNetwork.IsMasterClient) {
            Player[] players = PhotonNetwork.PlayerListOthers;
            if (players.Length > 0) {
                clientName.text = players[0].NickName;
            }
            else {
                clientName.text = "[Waiting]";
            }
        }
        else {
            clientName.text = PhotonNetwork.NickName;
        }
    }
}
