using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.SceneManagement;

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
    [SerializeField] private Transform particles;
    [SerializeField] private int layerMaster, layerClient;

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

        /*if (SceneManager.GetActiveScene().name == "LocalArena") {
            if (playerOwnerId == 0) {
                gameObject.layer = layerMaster;
            }
            else {
                gameObject.layer = layerClient;
            }
        }
        else {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("SetLayer", RpcTarget.All, layerMaster);
            }
            else {

                photonView.RPC("SetLayer", RpcTarget.All, layerClient);
            }
        }*/
    }

    //Collision detection
    void OnCollisionEnter(Collision collision)
    {
        if (inNetwork && !PhotonNetwork.IsMasterClient)
            return;

        GameObject.Find("SoundManager").GetComponent<SoundManager>().PlaySound(SoundManager.SoundList.EXPLOSION);//0.1719f
        
        //Check if is colliding with the launcher 
        if (collision.gameObject.tag == "Tank" && collision.gameObject.GetComponent<TankControl>().playerId != playerOwnerId)
        {
            switch (typeShell) {
                case ShellType.Large:
                    collision.gameObject.GetComponent<TankHealth>().TakeDamage(2); //Deal damages to other tank

                    break;
                case ShellType.Small:
                    collision.gameObject.GetComponent<TankHealth>().TakeDamage(1); //Deal damages to other tank

                    break;
            }
        }

        DestroyShell();
    }

    public void DestroyShell()
    {
        if (inNetwork)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SmallShellBurst"), transform.position, Quaternion.identity);
            if (typeShell == ShellType.Large) {
                photonView.RPC("DetatchParticles", RpcTarget.All);
            }
            PhotonNetwork.Destroy(photonView);
        }
        else
        {
            Instantiate(smallShellBurst, transform.position, Quaternion.identity);
            if (typeShell == ShellType.Large) {
                DetatchParticles();
            }
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

    [PunRPC]
    void DetatchParticles() {
        particles.SetParent(null);
    }

    [PunRPC]
    void SetLayer(int layerId) {
        gameObject.layer = layerId;
    }
}
