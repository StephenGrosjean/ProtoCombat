using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class GameSetup : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint1, spawnPoint2;
    
    void Start() {
        CreatePlayer();
    }

    private void CreatePlayer() {
        Debug.Log("Creating Player");

        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayMusic(SoundManager.MusicList.INGAME);

        Vector3 spawnPosition;
        string name;
        if (PhotonNetwork.IsMasterClient) {
            spawnPosition = spawnPoint1.position;
            name = "PhotonTank1";

        }
        else {
            spawnPosition = spawnPoint2.position;
            name = "PhotonTank2";
        }

        GameObject tank = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonTank"), spawnPosition, Quaternion.identity);
        tank.name = name;
    }
}
