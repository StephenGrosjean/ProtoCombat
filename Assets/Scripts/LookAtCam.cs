using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
/// <summary>
/// Script to make the camera look at two points
/// </summary>
public class LookAtCam : MonoBehaviour
{
    [SerializeField] private Transform firstTarget, secondTarget; //Targets to look at
    [SerializeField] private float xOffset, fovOffset; //Offsets
    private Vector3 middleVector = Vector3.zero; //Vector between two target
    private float distance; //Distance between two target
    
    //Draw middle point
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(middleVector, 0.5f);
    }

    private void Start() {
        Invoke("FindTargets", 1);
    }

    void FindTargets() {
        if (PhotonNetwork.IsMasterClient) {
            firstTarget = GameObject.Find("PhotonTank1").transform;
            secondTarget = GameObject.Find("PhotonTank(Clone)").transform;
        }
        else {
            firstTarget = GameObject.Find("PhotonTank2").transform;
            secondTarget = GameObject.Find("PhotonTank(Clone)").transform;
        }
    }

    void Update()
    {
        if (firstTarget != null && secondTarget != null) {
            distance = Vector3.Distance(firstTarget.position, secondTarget.position); //Find distance between the two targets

            middleVector = distance / 2 * Vector3.Normalize(secondTarget.position - firstTarget.position) + firstTarget.position; //Find the middle vector between the two targets
            transform.position = new Vector3(middleVector.x + xOffset, transform.position.y, middleVector.z); //Set the camera position to the middle vector position plus the offset
            float fovCalc = 0.7f * (distance + fovOffset) + 21.8f; //Define the fov 
            if (fovCalc > 65) { //Set the fov of the camera 
                Camera.main.fieldOfView = fovCalc;
            }
        }
    }
}
