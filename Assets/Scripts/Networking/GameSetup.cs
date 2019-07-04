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
        Vector3 spawnPosition;
        string name;
        int playerId;
        if (PhotonNetwork.IsMasterClient) {
            spawnPosition = spawnPoint1.position;
            name = "PhotonTank1";
            playerId = 1;

        }
        else {
            spawnPosition = spawnPoint2.position;
            name = "PhotonTank2";
            playerId = 2;
        }

        GameObject tank = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonTank"), spawnPosition, Quaternion.identity);
        tank.name = name;
        tank.GetComponent<TankControl>().playerId = playerId;
        tank.GetComponent<TankControl>().controllable = true;
    }
}
