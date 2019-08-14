using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Cinemachine;
/// <summary>
/// Script to make the camera look at two points
/// </summary>
public class LookAtCam : MonoBehaviour
{
    [SerializeField] private Transform firstTarget, secondTarget; //Targets to look at
    [SerializeField] private CinemachineTargetGroup targetGroup;

    private void Start() {
        Invoke("FindTargets", 1);
    }

    void FindTargets() {
        if (SceneManager.GetActiveScene().name == "LocalArena") {
            if (GameObject.Find("Player 1") != null) {
                firstTarget = GameObject.Find("Player 1").transform;
                Debug.Log("Camera found first target");
            }

            if (GameObject.Find("Player 2") != null) {
                secondTarget = GameObject.Find("Player 2").transform;
                Debug.Log("Camera found second target");
            }
        }
        else {
            if (PhotonNetwork.IsMasterClient) {
                if(GameObject.Find("PhotonTankMaster") != null) {
                    firstTarget = GameObject.Find("PhotonTankMaster").transform;
                    Debug.Log("Camera found first target");
                }
                if (GameObject.Find("PhotonTank(Clone)")) {
                    secondTarget = GameObject.Find("PhotonTank(Clone)").transform;
                    Debug.Log("Camera found second target");
                }
            }
            else {
                if(GameObject.Find("PhotonTankClient") != null) {
                    firstTarget = GameObject.Find("PhotonTankClient").transform;
                    Debug.Log("Camera found first target");
                }
                if (GameObject.Find("PhotonTank(Clone)") != null) {
                    secondTarget = GameObject.Find("PhotonTank(Clone)").transform;
                    Debug.Log("Camera found second target");
                }

            }
        }
        
    }

    void Update()
    {
        if (firstTarget != null && secondTarget != null) {
            targetGroup.m_Targets[0].target = firstTarget;
            targetGroup.m_Targets[1].target = secondTarget;  
        }
        else {
            FindTargets();
        }
    }


   
}
