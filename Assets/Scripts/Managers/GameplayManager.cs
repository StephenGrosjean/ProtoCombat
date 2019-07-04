using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameplayManager : MonoBehaviour {
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Transform spawnPointMaster, spawnPointClient;

    public Transform SpawnPointMaster { get { return spawnPointMaster; } }
    public Transform SpawnPointClient { get { return spawnPointClient; } }


    private GameObject tankMaster, tankClient;

    void Start()
    {
        
    }

    void Update()
    {
        if(tankMaster == null) {
            if (PhotonNetwork.IsMasterClient) {
                tankMaster = GameObject.Find("PhotonTankMaster");
            }
            else {
                tankMaster = GameObject.Find("PhotonTank(Clone)");
            }
        }

        if (tankClient == null) {
            if (PhotonNetwork.IsMasterClient) {
                tankClient = GameObject.Find("PhotonTank(Clone)");
            }
            else {
                tankClient = GameObject.Find("PhotonTankClient");
            }
        }

        if(tankClient.GetComponent<TankHealth>().GetLifes() <= 0 || tankMaster.GetComponent<TankHealth>().GetLifes() <= 0) {
            EndGame();
        }
    }

    void EndGame() {
        endGameScreen.SetActive(true);
        tankMaster.GetComponent<TankControl>().enabled = false;
        tankClient.GetComponent<TankControl>().enabled = false;

        if (tankClient.GetComponent<TankHealth>().GetLifes() <= 0) {
            if (PhotonNetwork.IsMasterClient) {
                winText.text = "You Won";
            }
            else {
                winText.text = "You Loose";
            }
        }

        if (tankMaster.GetComponent<TankHealth>().GetLifes() <= 0) {
            if (PhotonNetwork.IsMasterClient) {
                winText.text = "You Loose";
            }
            else {
                winText.text = "You Won";
            }
        }
    }
    
}
