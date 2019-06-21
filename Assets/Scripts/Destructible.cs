using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        //Check with what it is colliding
        if(collision.transform.tag == "Shell") {
            Destroy(collision.gameObject);
            EnableObjectAndExplode(collision.transform.position, collision.transform.GetComponent<Rigidbody>());
            
        }
        else if(collision.transform.tag != "Fragment"){
            //Check break force
            if(GetComponent<Rigidbody>().GetPointVelocity(collision.GetContact(0).point).magnitude >= breakForce) {
                EnableObjectAndBreak(GetComponent<Rigidbody>().GetPointVelocity(collision.GetContact(0).point));
            }
        }
    }

    //Explode into Fragments
    void EnableObjectAndExplode(Vector3 forcePosition, Rigidbody collisionRigid) {
        transform.DetachChildren(); //Detatch each child
        float i = 0;
        foreach (Transform cube in cubes) {
            i+=0.1f;
            cube.gameObject.SetActive(true);

            fragmentManager.AddFragment(cube.gameObject); //Add fragment to fragmentManager
            Destroy(cube.gameObject, timeToDestroy + i); //Destroy fragment after some time

            //Add explosion force depending of the shell type
            if (collisionRigid.gameObject.GetComponent<TankShell>().TypeShell == TankShell.ShellType.Small) {
                cube.GetComponent<Rigidbody>().AddExplosionForce(collisionRigid.velocity.magnitude * force, forcePosition, 3);
            }
            else{
                cube.GetComponent<Rigidbody>().AddExplosionForce(collisionRigid.velocity.magnitude * force * 100, forcePosition, 1);
            }
        }
        Destroy(light); //Destroy the light
        Destroy(gameObject); //Destroy the parent object
    }

    //Break into Fragment
    void EnableObjectAndBreak(Vector3 collisionVelocity) {
        transform.DetachChildren(); //Detatch each child
        float i = 0;

        foreach (Transform cube in cubes) {
            i += 0.1f;
            cube.gameObject.SetActive(true);
            Destroy(cube.gameObject, timeToDestroy + i);  //Destroy after some time
            fragmentManager.AddFragment(cube.gameObject); //Add fragment to fragmentManager
            cube.gameObject.GetComponent<Rigidbody>().velocity = collisionVelocity; //Keep velocity
        }
        Destroy(light); //Destroy the light
        Destroy(gameObject); //Destroy the parent object
    }
}
