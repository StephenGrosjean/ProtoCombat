using UnityEngine;
using Photon.Pun;

public class RoomController : MonoBehaviourPunCallbacks {
    [SerializeField] private int waitRoomSceneIndex;

    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined Room");
        PhotonNetwork.LoadLevel(waitRoomSceneIndex);
    }
}
