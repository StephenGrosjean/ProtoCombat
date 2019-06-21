using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script to manage tank health
/// </summary>

public class TankHealth : MonoBehaviour
{
    [SerializeField] private int health = 100; //Max Health
    [SerializeField] private GameObject explosionPrefab; //Particle to spawn at destroy

    
    void Update()
    {
        //Check if health below 0
        if(health <= 0) {
            Instantiate(explosionPrefab); //Instantiate explosion 
            Destroy(gameObject); //Destroy obejct
        }
    }


    //Take damages
    public void TakeDamage(int dmg) {
        health -= dmg;
    }

    //Heal
    public void Heal(int heal) {
        health += heal;
    }
}
