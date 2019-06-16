using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHealth : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0) {
            Instantiate(explosionPrefab);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int dmg) {
        health -= dmg;
    }

    public void Heal(int heal) {
        health += heal;
    }
}
