using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class GameSetupNetwork : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint1, spawnPoint2;
    
    void Start() {
        CreatePlayer();
    }

    private void CreatePlayer() {
        Debug.Log("Creating Player");

        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayMusic(SoundManager.MusicList.INGAME);

        Vector3 spawnPosition;
        Transform spawnTransform;
        string name;
        int playerId;
        if (PhotonNetwork.IsMasterClient) {
            spawnPosition = spawnPoint1.position;
            spawnTransform = spawnPoint1;
            name = "PhotonTankMaster";
            playerId = 1;

        }
        else {
            spawnPosition = spawnPoint2.position;
            spawnTransform = spawnPoint2;
            name = "PhotonTankClient";
            playerId = 2;
        }

        GameObject tank = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonTank"), spawnPosition, spawnTransform.rotation);
        tank.name = name;
        tank.GetComponent<TankControl>().SetupPlayer(playerId, true);
    }
}
