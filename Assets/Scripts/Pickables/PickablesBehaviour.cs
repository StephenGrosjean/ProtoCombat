using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class PickablesBehaviour : MonoBehaviour
{

    [SerializeField] private type pickupType;

    private enum type {
        HEAL
    }


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Tank") {

            switch (pickupType) {
                case type.HEAL:
                    other.gameObject.GetComponent<TankHealth>().Heal(1);
                    SoundManager._instance.PlaySound(SoundManager.SoundList.PICKUP);
                    break;
            }

            if(SceneManager.GetActiveScene().name == "LocalArena") {
                Destroy(transform.parent.gameObject);
            }
            else {
                PhotonNetwork.Destroy(transform.parent.gameObject);
            }
        }
    }
}
