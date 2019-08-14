using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
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
    public bool inNetwork;

    private void Start() {
        health = maxHealth;
        photonView = GetComponent<PhotonView>();
        GPManager = GameObject.Find("GameplayController").GetComponent<GameplayManager>();
    }

    void Update()
    {
        //Check if health below 0
        if(health <= 0 && lifes > 0) {
            if (inNetwork)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("SetHealth", RpcTarget.All, maxHealth);
                    photonView.RPC("LowerLife", RpcTarget.All, 1);
                }
            }
            else
            {
                SetHealth(maxHealth);
                LowerLife(1);
            }

            if (SceneManager.GetActiveScene().name == "LocalArena") {
                if(gameObject.name == "Player 1") {
                    StartCoroutine("WaitForRespawn", GPManager.SpawnPointMaster);
                }
                else {
                    StartCoroutine("WaitForRespawn", GPManager.SpawnPointClient);
                }
            }
            else {
                if (PhotonNetwork.IsMasterClient) {
                    StartCoroutine("WaitForRespawn", GPManager.SpawnPointMaster);
                }
                else {
                    StartCoroutine("WaitForRespawn", GPManager.SpawnPointClient);
                }
            }
        }
    }

    IEnumerator WaitForRespawn(Transform spawnTransform) {
        if(inNetwork)
            photonView.RPC("ToggleRenderersRPC", RpcTarget.All, false);
        else
            GetComponent<TankControl>().ToggleRenderers(false);

        yield return new WaitForSeconds(1);
        transform.position = spawnTransform.position;
        transform.rotation = spawnTransform.rotation;

        if (inNetwork)
            photonView.RPC("ToggleRenderersRPC", RpcTarget.All, true);
        else
            GetComponent<TankControl>().ToggleRenderers(true);
    }

    public void TakeDamage(int dmg)
    {
        if (inNetwork)
            photonView.RPC("Damage", RpcTarget.All, dmg);
        else
            Damage(dmg);
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
