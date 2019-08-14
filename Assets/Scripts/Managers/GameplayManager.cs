using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameplayManager : MonoBehaviour {
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private Transform spawnPointMaster, spawnPointClient;

    [SerializeField] private TextMeshProUGUI player1Status, player2Status;
    [SerializeField] private GameObject localWinPanel;

    public Transform SpawnPointMaster { get { return spawnPointMaster; } }
    public Transform SpawnPointClient { get { return spawnPointClient; } }


    private GameObject tankMaster, tankClient;

    void Start()
    {
        
    }

    public void OnPhotonPlayerDisconnected(Player player) {
        Debug.Log("Player Disconnected " + player.NickName);
        DisconectEndGame();
    }

    void Update()
    {

        if (SceneManager.GetActiveScene().name == "LocalArena") {
            if(tankMaster == null) {
                tankMaster = GameObject.Find("Player 1");
            }

            if(tankClient == null) {
                tankClient = GameObject.Find("Player 2");
            }
        }
        else {
            if (tankMaster == null) {
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
        }

        if (tankClient != null && tankMaster != null) {
            if (tankClient.GetComponent<TankHealth>().GetLifes() <= 0 || tankMaster.GetComponent<TankHealth>().GetLifes() <= 0) {
                EndGame();
            }
        }
    }

    void DisconectEndGame() {
        endGameScreen.SetActive(true);
        winText.text = "You Won";
        if(tankMaster != null) {
            tankMaster.GetComponent<TankControl>().enabled = false;
        }
        if(tankClient != null) {
            tankClient.GetComponent<TankControl>().enabled = false;
        }
    }

    void EndGame() {
        tankMaster.GetComponent<TankControl>().enabled = false;
        tankClient.GetComponent<TankControl>().enabled = false;

        if (SceneManager.GetActiveScene().name == "LocalArena") {
            localWinPanel.SetActive(true);
            if (tankClient.GetComponent<TankHealth>().GetLifes() <= 0) {
                player1Status.text = "You Won";
                player2Status.text = "You Lose";
            }

            if (tankMaster.GetComponent<TankHealth>().GetLifes() <= 0) {
                player1Status.text = "You Lose";
                player2Status.text = "You Won";
            }
        }
        else {
            endGameScreen.SetActive(true);
            if (tankClient.GetComponent<TankHealth>().GetLifes() <= 0) {
                if (PhotonNetwork.IsMasterClient) {
                    winText.text = "You Won";
                }
                else {
                    winText.text = "You Lose";
                }
            }

            if (tankMaster.GetComponent<TankHealth>().GetLifes() <= 0) {
                if (PhotonNetwork.IsMasterClient) {
                    winText.text = "You Lose";
                }
                else {
                    winText.text = "You Won";
                }
            }
        }
    }
    
}
