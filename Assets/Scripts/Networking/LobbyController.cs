using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class LobbyController : MonoBehaviourPunCallbacks {

    [SerializeField] private GameObject matchmakingButton, cancelButton, disconnectButton, createRoomButton, scrollView, roomNameObj; //UI Buttons
    [SerializeField] private int roomSize; //Size of the room to be created
    [SerializeField] private List<GameObject> roomsObj = new List<GameObject>(); //List of rooms
    [SerializeField] private GameObject roomPrefab; //Room prefab for the list
    [SerializeField] private Transform roomContainer; //Container of the rooms UI
    [SerializeField] private TMP_InputField roomName; //Name of custom room;
    [SerializeField] private TextMeshProUGUI errorMessage; //Error message text
    [SerializeField] private GameObject errorMessagePanel; //Error message panel
    [SerializeField] private TMP_InputField playerName; //Player name field
    
    //Called when client is connected to master server
    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby();
    }

    //Called when successfully joined the lobby
    public override void OnJoinedLobby() {
        PhotonNetwork.AutomaticallySyncScene = true;
        matchmakingButton.SetActive(true);

        string name = "";
        if (!PlayerPrefs.HasKey("PlayerName")) {
            PlayerPrefs.SetString("PlayerName", "Player");
        }

        playerName.text = PlayerPrefs.GetString("PlayerName");
        PhotonNetwork.NickName = playerName.text;
        Debug.Log("Player Name is : " + PhotonNetwork.NickName);
        
    }

    //Function for the matchmaking button
    public void RandomMatchmaking() {
        matchmakingButton.SetActive(false);
        disconnectButton.SetActive(false);
        cancelButton.SetActive(true);
        createRoomButton.SetActive(false);
        scrollView.SetActive(false);
        roomNameObj.SetActive(false);
        PhotonNetwork.JoinRandomRoom(); //Join a random room 
        Debug.Log("Started matchmaking");
    }

    //Called if join random room failed
    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    //Create a room 
    void CreateRoom() {
        Debug.Log("Creating room now");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps); //Creating a room with the desired options
    }

    //Called when CreateRoom failed, probably because a room as already the same name
    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Return code : " + returnCode.ToString() + " message : " + message);
        if(returnCode == 32766) {
            StartCoroutine("DisplayErrorMessage", "Room with that name already exist");
        }
        cancelButton.SetActive(false);
        disconnectButton.SetActive(true);
        matchmakingButton.SetActive(true);
        createRoomButton.SetActive(true);
        scrollView.SetActive(true);
        roomNameObj.SetActive(true);
        //CreateRoom();
    }

    //Cancel function for the cancel Button
    public void DelayCancel() {
        cancelButton.SetActive(false);
        disconnectButton.SetActive(true);
        matchmakingButton.SetActive(true);
        createRoomButton.SetActive(true);
        scrollView.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public void CreateCustomRoom() {
        if(roomName.text != "" && !ConsistsOfWhiteSpace(roomName.text)) {
            if (!GetComponent<BadWordsFilter>().CheckIfIsBadWord(roomName.text)) {
                matchmakingButton.SetActive(false);
                disconnectButton.SetActive(false);
                cancelButton.SetActive(true);
                createRoomButton.SetActive(false);
                scrollView.SetActive(false);
                RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)roomSize };
                PhotonNetwork.CreateRoom(roomName.text, roomOps); //Creating a room with the desired options
            }
            else {
                StartCoroutine("DisplayErrorMessage", "O.o How dare you!");
            }

        }
        else {
            StartCoroutine("DisplayErrorMessage", "Room name can't be empty");
        }

    }

    IEnumerator DisplayErrorMessage(string text) {
        errorMessage.text = text;
        errorMessagePanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        errorMessagePanel.SetActive(false);
    }

    //Called when a player enter or leave the lobby (Can only be called when connected to the lobby).
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        foreach (RoomInfo room in roomList) {
            if (room.PlayerCount > 0 && room.IsOpen && room.PlayerCount < roomSize) {
                Debug.Log(room.Name + " : " + room.PlayerCount);
                CreateRoomObj(room);
            }
            else {
                RemoveRoomObj(room);
            }
        }

    }

    //Create UI Room
    void CreateRoomObj(RoomInfo room) {
        GameObject roomObj = Instantiate(roomPrefab, roomContainer);
        roomObj.name = room.Name;
        roomObj.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;
        roomsObj.Add(roomObj);
    }

    //Remove UI Room
    void RemoveRoomObj(RoomInfo room) {
        GameObject roomToDelete = null;
        foreach (GameObject r in roomsObj) {
            if (r.name == room.Name) {
                roomToDelete = r;
            }
        }

        roomsObj.Remove(roomToDelete);
        Destroy(roomToDelete);

    }

    //Remoe all UI Rooms
    void RemoveAllRooms() {
        for (int i = 0; i < roomsObj.Count; i++) {
            Destroy(roomsObj[i]);
            roomsObj.Remove(roomsObj[i]);
        }
    }

    //Check if string is only white space
    public bool ConsistsOfWhiteSpace(string s) {
        foreach (char c in s) {
            if (c != ' ') return false;
        }
        return true;

    }

    public void SetPlayerName() {
        PhotonNetwork.NickName = playerName.text;
        PlayerPrefs.SetString("PlayerName", PhotonNetwork.NickName);
        Debug.Log("Player Name set to " + PhotonNetwork.NickName);
    }

}
