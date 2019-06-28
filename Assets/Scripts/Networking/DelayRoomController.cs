using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DelayRoomController : MonoBehaviourPunCallbacks {
    private PhotonView myPhotonView;

    [SerializeField] private int multiplayerSceneIndex;
    [SerializeField] private int menuSceneIndex;

    private int playerCount;
    private int roomSize;

    [SerializeField] private int minPlayersToStart;

    [SerializeField] private TextMeshProUGUI playerCountDisplay;
    [SerializeField] private TextMeshProUGUI timerToStartDisplay;
    [SerializeField] private TextMeshProUGUI title;

    private bool readyToCountDown;
    private bool readyToStart;
    private bool startingGame;

    private float timerToStartGame;
    private float notFullGameTimer;
    private float fullGameTimer;

    [SerializeField] private float maxWaitTime;
    [SerializeField] private float maxFullGameWaitTime;

    void Start() {
        myPhotonView = GetComponent<PhotonView>();
        fullGameTimer = maxFullGameWaitTime;
        notFullGameTimer = maxWaitTime;
        timerToStartGame = maxWaitTime;
        title.text = "Waiting " + PhotonNetwork.CurrentRoom.Name;
        PlayerCountUpdate();
    }

    //Update player count
    void PlayerCountUpdate() {
        playerCount = PhotonNetwork.PlayerList.Length;
        roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        playerCountDisplay.text = playerCount + ":" + roomSize;

        if (playerCount == roomSize) {
            readyToStart = true;
        }
        else if (playerCount >= minPlayersToStart) {
            readyToCountDown = true;
        }
        else {
            readyToCountDown = false;
            readyToStart = false;
        }
    }

    //Called when a player enter a room
    public override void OnPlayerEnteredRoom(Player newPlayer) {
        PlayerCountUpdate();
        if (PhotonNetwork.IsMasterClient) {
            myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartGame);
        }
    }

    //RPC To update the timer
    [PunRPC]
    private void RPC_SendTimer(float timeIn) {
        timerToStartGame = timeIn;
        notFullGameTimer = timeIn;
        if (timeIn < fullGameTimer) {
            fullGameTimer = timeIn;
        }
    }

    //Called when a player leave the room
    public override void OnPlayerLeftRoom(Player otherPlayer) {
        PlayerCountUpdate();
    }

    private void Update() {
        WaitingForMorePlayers();
    }

    //Check total player count to start the game
    void WaitingForMorePlayers() {
        if (playerCount <= 1) {
            ResetTimer();
        }

        if (readyToStart) {
            fullGameTimer -= Time.deltaTime;
            timerToStartGame = fullGameTimer;
        }
        else if (readyToCountDown) {
            notFullGameTimer -= Time.deltaTime;
            timerToStartGame = notFullGameTimer;
        }

        string tempTimer = string.Format("{0:00}", timerToStartGame);
        timerToStartDisplay.text = tempTimer;

        if (timerToStartGame <= 0f) {
            if (startingGame) return;
            StartGame();
        }
    }

    //Reset the timer
    void ResetTimer() {
        timerToStartGame = maxWaitTime;
        notFullGameTimer = maxWaitTime;
        fullGameTimer = maxFullGameWaitTime;
    }

    //Start the game
    void StartGame() {
        startingGame = true;

        if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(multiplayerSceneIndex);
    }

    //Leave the room
    public void DelayCancel() {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(menuSceneIndex);
    }
}
