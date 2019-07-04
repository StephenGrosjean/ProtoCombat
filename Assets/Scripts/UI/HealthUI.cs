﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private bool isMasterUI;
    public GameObject tank;
    private TextMeshProUGUI health;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tank == null) {
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
            health.text = tank.GetComponent<TankHealth>().GetHealth().ToString();
        }
    }
}
