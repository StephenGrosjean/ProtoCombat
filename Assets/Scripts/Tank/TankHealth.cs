using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
/// <summary>
/// Script to manage tank health
/// </summary>

public class TankHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10; //Max Health
    [SerializeField] private int lifes = 3; //Lifes
    [SerializeField] private GameObject explosionPrefab; //Particle to spawn at destroy
    private PhotonView photonView;
    private GameplayManager GPManager;
    private int health; //Health


    private void Start() {
        health = maxHealth;
        photonView = GetComponent<PhotonView>();
        GPManager = GameObject.Find("GameplayController").GetComponent<GameplayManager>();
    }

    void Update()
    {
        //Check if health below 0
        if(health <= 0 && lifes > 0) {
            //Instantiate(explosionPrefab); //Instantiate explosion 
            //Destroy(gameObject); //Destroy obejct
            if (photonView.IsMine) {
                if (PhotonNetwork.IsMasterClient) {
                    photonView.RPC("SetHealth", RpcTarget.All, maxHealth);
                    photonView.RPC("LowerLife", RpcTarget.All, 1);
                    StartCoroutine("WaitForRespawn", GPManager.SpawnPointMaster.position);
                }
                else {
                    photonView.RPC("SetHealth", RpcTarget.All, maxHealth);
                    photonView.RPC("LowerLife", RpcTarget.All, 1);
                    StartCoroutine("WaitForRespawn",GPManager.SpawnPointClient.position);
                }
            }
        }
    }

    public void TakeDamage(int dmg) {
        photonView.RPC("Damage", RpcTarget.All, dmg);
    }

    IEnumerator WaitForRespawn(Vector3 spawnPosition) {
        GetComponent<TankControl>().ToggleRenderersNetwork(false);
        yield return new WaitForSeconds(1);
        transform.position = spawnPosition;
        GetComponent<TankControl>().ToggleRenderersNetwork(true);
    }

    [PunRPC]
    //Take damages
    void Damage(int dmg) {
        health -= dmg;
    }

    [PunRPC]
    void SetHealth(int value) {
        health = value;
    }

    [PunRPC]
    void LowerLife(int value) {
        lifes -= value;
    }

    //Heal
    public void Heal(int heal) {
        health += heal;
    }

    public int GetHealth() {
        return health;
    }

    public int GetLifes() {
        return lifes;
    }
}
