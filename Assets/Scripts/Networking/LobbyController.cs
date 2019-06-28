using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyController : MonoBehaviourPunCallbacks {

    [SerializeField] private GameObject matchmakingButton, cancelButton; //UI Buttons
    [SerializeField] private int roomSize; //Size of the room to be created
    [SerializeField] private List<GameObject> roomsObj = new List<GameObject>(); //List of rooms
    [SerializeField] private GameObject roomPrefab; //Room prefab for the list
    [SerializeField] private Transform roomContainer; //Container of the rooms UI

    //Called when client is connected to master server
    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinLobby();
    }

    //Called when successfully joined the lobby
    public override void OnJoinedLobby() {
        PhotonNetwork.AutomaticallySyncScene = true;
        matchmakingButton.SetActive(true);
    }

    //Function for the matchmaking button
    public void RandomMatchmaking() {
        matchmakingButton.SetActive(false);
        cancelButton.SetActive(true);
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
        Debug.Log("Failed to create room... trying again");
        CreateRoom();
    }

    //Cancel function for the cancel Button
    public void DelayCancel() {
        cancelButton.SetActive(false);
        matchmakingButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
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
}
