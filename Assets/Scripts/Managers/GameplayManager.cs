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
    [SerializeField] private Animator lightAnim;

    public Transform SpawnPointMaster { get { return spawnPointMaster; } }
    public Transform SpawnPointClient { get { return spawnPointClient; } }


    private GameObject tankMaster, tankClient;
    private PhotonView photonView;

    private float maxTimeContact = 1;
    private float currentTimeContact;
    private Vector3 middleDistance;

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(middleDistance, 0.5f);
    }

    void Start() {

    }

    public void OnPhotonPlayerDisconnected(Player player) {
        Debug.Log("Player Disconnected " + player.NickName);
        DisconectEndGame();
    }

    void Update() {




        if (SceneManager.GetActiveScene().name == "LocalArena") {
            if (tankMaster == null) {
                tankMaster = GameObject.Find("Player 1");
            }

            if (tankClient == null) {
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


        if (tankClient != null && tankMaster != null) {
            float distance = Vector3.Distance(tankMaster.transform.position, tankClient.transform.transform.position);
            middleDistance = new Vector3((tankClient.transform.position.x + tankMaster.transform.position.x) / 2, 6.5f, (tankClient.transform.position.z + tankMaster.transform.position.z) / 2);

            SetLightPosition(middleDistance);

            if (distance < 6) {
                if (currentTimeContact < maxTimeContact) {
                    currentTimeContact += Time.deltaTime;
                }
                else {
                    ApplyForceOnTank(tankMaster.transform);
                    ApplyForceOnTank(tankClient.transform);
                    currentTimeContact = 0;
                }
            }
            else {
                currentTimeContact = 0;
            }
        }
    }

    void DisconectEndGame() {
        endGameScreen.SetActive(true);
        winText.text = "You Won";
        if (tankMaster != null) {
            tankMaster.GetComponent<TankControl>().enabled = false;
        }
        if (tankClient != null) {
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

    void ApplyForceOnTank(Transform tank) {
        tank.GetComponent<Rigidbody>().AddRelativeForce(-Vector3.right * 3000);
        DeployLightForce();
    }

    void DeployLightForce() {
        lightAnim.Play("Pulse");
    }

    void SetLightPosition(Vector3 pos) {
        lightAnim.transform.position = pos;
    }

}
