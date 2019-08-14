using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class LifeUI : MonoBehaviour
{
    [SerializeField] private bool isMasterUI;
    public GameObject tank;
    private TextMeshProUGUI life;

    // Start is called before the first frame update
    void Start()
    {
        life = GetComponent<TextMeshProUGUI>();
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
            life.text = tank.GetComponent<TankHealth>().GetLifes().ToString();
        }
    }
}
