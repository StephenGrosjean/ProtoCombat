using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankNetwork : MonoBehaviour
{
    private PhotonView myView;

    // Start is called before the first frame update
    void Start()
    {
        myView = GetComponent<PhotonView>();

        if (!myView.IsMine) {
            GetComponent<TankControl>().enabled = false;
            GetComponent<SkyShellSpawning>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
