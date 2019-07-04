using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/// <summary>
/// Destructible script
/// </summary>
public class Destructible : MonoBehaviour
{
    [SerializeField] Transform[] cubes; //What are the fragment of the object
    [SerializeField] int force; //With what force should they explode
    [SerializeField] float breakForce; //At what force should they break
    [SerializeField] private GameObject light; //Light object
    [SerializeField] private int timeToDestroy = 20;
    private FragmentManager fragmentManager; //Fragment manager script

    void Start()
    {
        fragmentManager = GameObject.Find("FragmentManager").GetComponent<FragmentManager>(); //Get fragment Manager script
    }

    private void OnCollisionEnter(Collision collision) {
        if (PhotonNetwork.IsMasterClient)
        {
            //Check with what it is colliding
            if(collision.transform.tag == "Shell") {
                //PhotonNetwork.Destroy(collision.gameObject);
                if (collision.gameObject.GetComponent<TankShell>().TypeShell == TankShell.ShellType.Small)
                    GetComponent<PhotonView>().RPC("EnableObjectAndExplode", RpcTarget.All, collision.transform.position, collision.transform.GetComponent<Rigidbody>().velocity.magnitude);
                else
                    GetComponent<PhotonView>().RPC("EnableObjectAndExplode", RpcTarget.All, collision.transform.position, collision.transform.GetComponent<Rigidbody>().velocity.magnitude * 100.0f);
            }
            else if(collision.transform.tag != "Fragment"){
                //Check break force
                if(GetComponent<Rigidbody>().GetPointVelocity(collision.GetContact(0).point).magnitude >= breakForce) {
                    EnableObjectAndBreak(GetComponent<Rigidbody>().GetPointVelocity(collision.GetContact(0).point));
                }
            }
        }
    }
    

    //Explode into Fragments
    [PunRPC]
    void EnableObjectAndExplode(Vector3 forcePosition, float explosionForce) {
        transform.DetachChildren(); //Detatch each child
        float i = 0;
        foreach (Transform cube in cubes) {
            i+=0.1f;
            cube.gameObject.SetActive(true);
            fragmentManager.AddFragment(cube.gameObject); //Add fragment to fragmentManager
            //PhotonNetwork.Destroy(cube.gameObject, timeToDestroy + i);
            //Destroy(cube.gameObject, timeToDestroy + i); //Destroy fragment after some time

            //Add explosion force depending of the shell type
            cube.GetComponent<Rigidbody>().AddExplosionForce(explosionForce * force, forcePosition, 3);
        }

        PhotonNetwork.Destroy(light.gameObject);
        //Destroy(light); //Destroy the light
        PhotonNetwork.Destroy(gameObject);
        //Destroy(gameObject); //Destroy the parent object
    }

    //Break into Fragment
    [PunRPC]
    void EnableObjectAndBreak(Vector3 collisionVelocity) {
        transform.DetachChildren(); //Detatch each child
        float i = 0;

        foreach (Transform cube in cubes) {
            i += 0.1f;
            cube.gameObject.SetActive(true);
            //PhotonNetwork.Destroy(cube.gameObject, timeToDestroy + i);  //Destroy after some time
            if (PhotonNetwork.IsMasterClient)
                fragmentManager.AddFragment(cube.gameObject); //Add fragment to fragmentManager
            cube.gameObject.GetComponent<Rigidbody>().velocity = collisionVelocity; //Keep velocity
        }
        PhotonNetwork.Destroy(light); //Destroy the light
        PhotonNetwork.Destroy(gameObject); //Destroy the parent object
    }
}
