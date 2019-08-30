using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

/// <summary>
/// Destructible script
/// </summary>
public class Destructible : MonoBehaviour
{
    [SerializeField] GameObject lifePowerup;
    [SerializeField] Transform[] cubes; //What are the fragment of the object
    [SerializeField] int force; //With what force should they explode
    [SerializeField] float breakForce; //At what force should they break
    [SerializeField] private GameObject light; //Light object
    [SerializeField] private int timeToDestroy = 20;
    private FragmentManager fragmentManager; //Fragment manager script

    private GameObject inControlObject;

    void Start()
    {
        fragmentManager = GameObject.Find("FragmentManager").GetComponent<FragmentManager>(); //Get fragment Manager script
        inControlObject = GameObject.Find("InControl");
    }

    private void OnCollisionEnter(Collision collision)
    { 
        if (inControlObject.GetComponent<MultiControllerManager>() == null && !PhotonNetwork.IsMasterClient)
            return;

        //Check with what it is colliding
        if(collision.transform.tag == "Shell")
        {

            float modifier = 1.0f;
            if (collision.gameObject.GetComponent<TankShell>().TypeShell != TankShell.ShellType.Small)
                modifier = 100.0f;

            if (inControlObject.GetComponent<MultiControllerManager>() == null)
                GetComponent<PhotonView>().RPC("EnableObjectAndExplodeRPC", RpcTarget.All, collision.transform.position, collision.transform.GetComponent<Rigidbody>().velocity.magnitude * modifier);
            else
                EnableObjectAndExplode(collision.transform.position, collision.transform.GetComponent<Rigidbody>().velocity.magnitude * modifier);


            float RDMN = Random.Range(0.0f, 100.0f);
            if (RDMN < 15) {
                if (SceneManager.GetActiveScene().name == "LocalArena") {
                    InstantiatePickup(false);
                }
                else {
                    GetComponent<PhotonView>().RPC("InstantiatePickup", RpcTarget.All, true);
                }
            }
        }
        else if(collision.transform.tag != "Fragment"){
            //Check break force
            if(GetComponent<Rigidbody>().GetPointVelocity(collision.GetContact(0).point).magnitude >= breakForce) {
                EnableObjectAndBreak(GetComponent<Rigidbody>().GetPointVelocity(collision.GetContact(0).point));
            }
        }

    }
    
    //Explode into Fragments
    [PunRPC]
    void EnableObjectAndExplodeRPC(Vector3 forcePosition, float explosionForce)
    {
        EnableObjectAndExplode(forcePosition, explosionForce);
    }

    void EnableObjectAndExplode(Vector3 forcePosition, float explosionForce)
    {
        transform.DetachChildren(); //Detatch each child
        float i = 0;
        foreach (Transform cube in cubes)
        {
            i += 0.1f;
            cube.gameObject.SetActive(true);
            //fragmentManager.AddFragment(cube.gameObject); //Add fragment to fragmentManager
            //PhotonNetwork.Destroy(cube.gameObject, timeToDestroy + i);
            //Destroy(cube.gameObject, timeToDestroy + i); //Destroy fragment after some time

            //Add explosion force depending of the shell type
            cube.GetComponent<Rigidbody>().AddExplosionForce(explosionForce * force, forcePosition, 3);
        }

        Destroy(light.gameObject);
        //Destroy(light); //Destroy the light
        Destroy(gameObject);
        //Destroy(gameObject); //Destroy the parent object
    }

    //Break into Fragment
    [PunRPC]
    void EnableObjectAndBreakRPC(Vector3 collisionVelocity)
    {
        EnableObjectAndBreak(collisionVelocity);
    }

    void EnableObjectAndBreak(Vector3 collisionVelocity)
    {
        transform.DetachChildren(); //Detatch each child
        float i = 0;

        foreach (Transform cube in cubes)
        {
            i += 0.1f;
            cube.gameObject.SetActive(true);
            //PhotonNetwork.Destroy(cube.gameObject, timeToDestroy + i);  //Destroy after some time
            if (PhotonNetwork.IsMasterClient)
                //fragmentManager.AddFragment(cube.gameObject); //Add fragment to fragmentManager
                cube.gameObject.GetComponent<Rigidbody>().velocity = collisionVelocity; //Keep velocity
        }

        

        Destroy(light); //Destroy the light
        Destroy(gameObject); //Destroy the parent object
    }

    [PunRPC]
    void InstantiatePickup(bool isNetwork) {
        if (isNetwork) {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Life_Pickable"), transform.position, Quaternion.identity);
        }
        else {
            Instantiate(lifePowerup, transform.position, Quaternion.identity);
            Debug.Log(transform.position);
        }
    }
}
