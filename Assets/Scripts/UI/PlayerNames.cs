using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
public class PlayerNames : MonoBehaviour
{
    [SerializeField] private bool isMasterUI;
    [SerializeField] private Vector3 offset;
    private GameObject tank;

    // Update is called once per frame
    void Update() {
        if (tank == null) {
            if (SceneManager.GetActiveScene().name == "LocalArena") {
                if (isMasterUI) {
                    tank = GameObject.Find("Player 1");
                }
                else {
                    tank = GameObject.Find("Player 2");
                }
            }
            else {
                if (PhotonNetwork.IsMasterClient) {
                    if (isMasterUI) {
                        tank = GameObject.Find("PhotonTankMaster");
                        GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
                    }
                    else {
                        tank = GameObject.Find("PhotonTank(Clone)");
                        Player[] players = PhotonNetwork.PlayerListOthers;
                        GetComponent<TextMeshProUGUI>().text = players[0].NickName;
                    }

                }
                else {
                    if (isMasterUI) {
                        tank = GameObject.Find("PhotonTank(Clone)");
                        Player[] players = PhotonNetwork.PlayerListOthers;
                        GetComponent<TextMeshProUGUI>().text = players[0].NickName;
                    }
                    else {
                        tank = GameObject.Find("PhotonTankClient");
                        GetComponent<TextMeshProUGUI>().text = PhotonNetwork.NickName;
                    }

                }
            }
        }

        if (tank != null) {
            transform.position = tank.transform.position + offset;
        }
    }
}
