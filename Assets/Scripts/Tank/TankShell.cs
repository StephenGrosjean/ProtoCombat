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
    [SerializeField] private ShellType typeShell; //Type of the shell
    [SerializeField] private bool skyShell; //Is SkyShell ?
    [SerializeField] private GameObject smallShellBurst; //Particle to spawn at shell destroy
    public ShellType TypeShell { get { return typeShell; } set { typeShell = value; } } //GetSet the shell Type

    private int health = 1; //Health of the shell (Used of the bounce)  (AKA number of bounce)

    private Rigidbody rigid; //Rigidbody of the shell

    private PhotonView photonView;

    private int playerOwnerId; //Who launched it?
    private bool inNetwork;

    void Awake()
    {
        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlaySound(SoundManager.SoundList.FIRE); //0.1719f
        photonView = GetComponent<PhotonView>();

        rigid = GetComponent<Rigidbody>();
    }

    void Start()
    {

        //Add force to launch shell depending of the skyshell parameter 
        /*
        if (!skyShell)
        {
            rigid.AddRelativeForce(-Vector3.left * speed);
            health = -1;
        }
        else
        {
            rigid.AddRelativeForce(Vector3.down * speed);

        }
        */
        //Destroy the shell after 5 sec
        //Destroy(gameObject, 5);
    }

    //Collision detection
    void OnCollisionEnter(Collision collision) {
        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlaySound(SoundManager.SoundList.EXPLOSION);//0.1719f
        if (inNetwork)
        {
            if (photonView.Owner == PhotonNetwork.MasterClient) {
                //Check if is colliding with the launcher 
                if (collision.gameObject.GetComponent<TankControl>() && collision.gameObject.GetComponent<TankControl>().playerId != playerOwnerId)
                {
                    //Check with who it is colliding
                    if (collision.gameObject.tag == "Destroyable")
                    {
                        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SmallShellBurst"), transform.position, Quaternion.identity);
                        //Destroy(gameObject);
                        PhotonNetwork.Destroy(photonView);
                        return;
                    }
                    else if (collision.gameObject.tag == "Tank")
                    {
                        if (collision.gameObject.GetComponent<TankControl>().playerId != playerOwnerId) {
                            collision.gameObject.GetComponent<TankHealth>().TakeDamage(1); //Deal damages to other tank
                        }
                        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SmallShellBurst"), transform.position, Quaternion.identity);
                        // Destroy(gameObject);
                        PhotonNetwork.Destroy(photonView);
                        return;
                    }
                    else
                    {
                        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SmallShellBurst"), transform.position, Quaternion.identity);
                        //Destroy(gameObject);
                        PhotonNetwork.Destroy(photonView);
                        return;
                    }
                }
                else
                {
                    PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SmallShellBurst"), transform.position, Quaternion.identity);
                    //Destroy(gameObject);
                    PhotonNetwork.Destroy(photonView);
                    return;
                }
            }
        }
        else
        {
            if (collision.gameObject.GetComponent<TankControl>() && collision.gameObject.GetComponent<TankControl>().playerId != playerOwnerId)
            {
                //Check with who it is colliding
                if (collision.gameObject.tag == "Destroyable")
                {
                    Instantiate(smallShellBurst, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    return;
                }
                else if (collision.gameObject.tag == "Tank")
                {
                    if (collision.gameObject.GetComponent<TankControl>().playerId != playerOwnerId)
                    {
                        collision.gameObject.GetComponent<TankHealth>().TakeDamage(1); //Deal damages to other tank
                    }
                    Instantiate(smallShellBurst, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    Instantiate(smallShellBurst, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                    return;
                }

                //Decrease life for the bounce
                if (health > 0)
                {
                    rigid.velocity *= 2; //Double the velocity
                    health--;
                }
            }
            else
            {
                Instantiate(smallShellBurst, transform.position, Quaternion.identity);
                Destroy(gameObject);
                return;
            }
        }
    }

    public void DestroyShell()
    {
        if (inNetwork)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SmallShellBurst"), transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(photonView);
        }
        else
        {
            Instantiate(smallShellBurst, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void InitializeShellRPC(int playerOwnerId, Quaternion rotation, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        InitializeShell(playerOwnerId, rotation, true);
        
        rigid.position += rigid.velocity * lag;

        if (!PhotonNetwork.IsMasterClient)
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
    }

    public void InitializeShell(int playerOwnerId, Quaternion rotation, bool inNetwork = false)
    {
        this.playerOwnerId = playerOwnerId;

        rigid.rotation = rotation;
        rigid.AddRelativeForce(Vector3.left * speed);

        //Destroy the shell after 5 sec
        Destroy(gameObject, 5);
    }
}
