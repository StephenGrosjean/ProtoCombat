using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Fragment Script
/// </summary>

public class Fragment : MonoBehaviour
{

    void Start()
    {
        //Invoke DisableRigid after 5sec
        Invoke("DisableRigid", 5);
        Destroy(gameObject, 10);
    }

    //Remove the fragment from the FragmentManager List
    private void OnDestroy() {
        //GameObject.Find("FragmentManager").GetComponent<FragmentManager>().RemoveFragment(gameObject);
    }

    //Disable Rigidbody to save performances
    void DisableRigid() {
        Destroy(GetComponent<Rigidbody>());
    }
}
