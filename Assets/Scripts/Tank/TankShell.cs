using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script for the Tank Shells
/// </summary>
public class TankShell : MonoBehaviour
{
    //Shell Type
    public enum ShellType {
        Small,
        Large
    }
    

    [SerializeField] private float speed = 10; //Speed of the shell
    [SerializeField] private GameObject explosionParticle; //Particle to spawn at shell destroy
    [SerializeField] private ShellType typeShell; //Type of the shell
    [SerializeField] private bool skyShell; //Is SkyShell ?
    public ShellType TypeShell { get { return typeShell; } set { typeShell = value; } } //GetSet the shell Type

    private int health = 1; //Health of the shell (Used of the bounce)  (AKA number of bounce)

    private Rigidbody rigid; //Rigidbody of the shell
    private GameObject launcherParent; //Who launched it?

    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        //Add force to launch shell depending of the skyshell parameter 
        if (!skyShell) {
            rigid.AddRelativeForce(-Vector3.left * speed);
            health = -1;
        }
        else {
            rigid.AddRelativeForce(Vector3.down * speed);

        }
        //Destroy the shell after 5 sec
        Destroy(gameObject, 5);
    }

    //Collision detection
    void OnCollisionEnter(Collision collision) {
        //Check if is colliding with the launcher 
        if (collision.gameObject != launcherParent || launcherParent == null) {
            //Check with who it is colliding
            if(collision.gameObject.tag == "Destroyable") {
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
                Destroy(gameObject, 0.1f);
            }
            else if(collision.gameObject.tag == "Tank") {
                collision.gameObject.GetComponent<TankHealth>().TakeDamage(5); //Deal damages to other tank
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
                Destroy(gameObject, 0.1f);
            }
            else {
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
                Destroy(gameObject, 0.1f);
            }

            //Decrease life for the bounce
            if (health > 0) {
                rigid.velocity *= 2; //Double the velocity
                health--;

            }
            else { //Destroy if no life (bounce) remain
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
                Destroy(gameObject, 0.1f);
            }

        }
        else {
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.1f);
        }

    }
     //Set the launcher Object
    public void SetLauncherParent(GameObject launcher) {
        launcherParent = launcher;
    }
}
