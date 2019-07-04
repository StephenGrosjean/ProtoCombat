using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNames : MonoBehaviour
{
    [SerializeField] private bool isMasterUI;
    [SerializeField] private Vector3 offset;
    private GameObject tank;

    // Update is called once per frame
    void Update() {
        if (tank == null) {
            if (PhotonNetwork.IsMasterClient) {
                if (isMasterUI) {
                    tank = GameObject.Find("PhotonTankMaster");

                }
                else {
                    tank = GameObject.Find("PhotonTank(Clone)");

                }
            }
            else {
                if (isMasterUI) {
                    tank = GameObject.Find("PhotonTank(Clone)");

                }
                else {
                    tank = GameObject.Find("PhotonTankClient");

                }
            }
        }

        if (tank != null) {
            transform.position = tank.transform.position + offset;
        }
    }
}
