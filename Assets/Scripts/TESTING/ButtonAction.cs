using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    public GameObject photonTank;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void dmg() {
        photonTank = GameObject.Find("PhotonTank(Clone)");
        photonTank.GetComponent<TankHealth>().TakeDamage(5);
    }
}
