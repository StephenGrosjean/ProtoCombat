using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private bool isMasterUI;
    public GameObject tank;
    private Image health;

    private float minFilling = 0.38f;
    private float maxFilling = 0.9f;

    public float h;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tank == null) {
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
        }

        if (tank != null) {
            health.fillAmount = FindHealth(tank.GetComponent<TankHealth>().GetHealth());
        }
    }

    float FindHealth(float x) {
        float m = (maxFilling - minFilling) / 10;
        float b = minFilling;
        float result = m * x + b;
        h = result;
        return result;
    }
}
