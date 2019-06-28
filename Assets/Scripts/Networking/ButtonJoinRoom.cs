using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ButtonJoinRoom : MonoBehaviour
{
    public void JoinRoom() {
        PhotonNetwork.JoinRoom(gameObject.name);
    }
}
