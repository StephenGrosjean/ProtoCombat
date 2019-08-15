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
    [SerializeField] private List<GameObject> lifes;
    public GameObject tank;

    // Start is called before the first frame update
    void Start()
    {
        //life = GetComponent<TextMeshProUGUI>();
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
            //life.text = tank.GetComponent<TankHealth>().GetLifes().ToString();
            ToggleLifes();
        }
    }

    void ToggleLifes() {
        int tankLifes = tank.GetComponent<TankHealth>().GetLifes();
        switch (tankLifes) {
                case 3:
                    lifes[0].SetActive(true);
                    lifes[1].SetActive(true);
                    lifes[2].SetActive(true);
                    break;
                case 2:
                    lifes[0].SetActive(true);
                    lifes[1].SetActive(true);
                    lifes[2].SetActive(false);
                    break;
                case 1:
                    lifes[0].SetActive(false);
                    lifes[1].SetActive(false);
                    lifes[2].SetActive(false);
                    break;

                case 0:
                    lifes[0].SetActive(false);
                    lifes[1].SetActive(false);
                    lifes[2].SetActive(false);
                    break;
            }
        
        
    }
}
