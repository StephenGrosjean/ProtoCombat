using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

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

    private PhotonView photonView;

    private int playerOwnerId; //Who launched it?


    void Start()
    {
        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlaySound(SoundManager.SoundList.FIRE);//0.1719f
        photonView = GetComponent<PhotonView>();

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }

        rigid = GetComponent<Rigidbody>();

        //Add force to launch shell depending of the skyshell parameter 
        if (!skyShell)
        {
            rigid.AddRelativeForce(-Vector3.left * speed);
            health = -1;
        }
        else
        {
            rigid.AddRelativeForce(Vector3.down * speed);

        }
        //Destroy the shell after 5 sec
        Destroy(gameObject, 5);
    }

    //Collision detection
    void OnCollisionEnter(Collision collision) {
        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlaySound(SoundManager.SoundList.EXPLOSION);//0.1719f
        if (photonView.Owner == PhotonNetwork.MasterClient) {
            //Check if is colliding with the launcher 
            if (collision.gameObject != launcherParent || launcherParent == null)
            {
                //Check with who it is colliding
                if (collision.gameObject.tag == "Destroyable")
                {
                    Instantiate(explosionParticle, transform.position, Quaternion.identity);
                    //Destroy(gameObject);
                    PhotonNetwork.Destroy(photonView);
                    return;
                }
                else if (collision.gameObject.tag == "Tank")
                {
                    collision.gameObject.GetComponent<TankHealth>().TakeDamage(5); //Deal damages to other tank
                    Instantiate(explosionParticle, transform.position, Quaternion.identity);
                    // Destroy(gameObject);
                    PhotonNetwork.Destroy(photonView);
                    return;
                }
                else
                {
                    Instantiate(explosionParticle, transform.position, Quaternion.identity);
                    //Destroy(gameObject);
                    PhotonNetwork.Destroy(photonView);
                    return;
                }

                //Decrease life for the bounce
                if (health > 0)
                {
                    rigid.velocity *= 2; //Double the velocity
                    health--;

                }
                else
                { //Destroy if no life (bounce) remain
                    Instantiate(explosionParticle, transform.position, Quaternion.identity);
                    //Destroy(gameObject);
                    PhotonNetwork.Destroy(photonView);
                    return;
                }

            }
            else
            {
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
                //Destroy(gameObject);
                PhotonNetwork.Destroy(photonView);
                return;
            }
        }
    }

    public void DestroyShell()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SmallShellBurst"), transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(photonView);
    }

     //Set the launcher Object
    public void SetLauncherParent(GameObject launcher) {
        launcherParent = launcher;
    }

    public void InitializeShell(int playerOwnerId, GameObject launcher, float lag)
    {
        this.playerOwnerId = playerOwnerId;
        this.SetLauncherParent(launcher);

        //transform.forward = originalDirection;
        rigid.AddRelativeForce(-Vector3.left * speed);

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * 200.0f;
        rigidbody.position += rigidbody.velocity * lag;
    }
}
