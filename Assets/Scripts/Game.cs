
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Basic script to control global behaviour
/// </summary>
public class Game : MonoBehaviour
{

    void Update()
    {
        //Quit game if press escape
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        } 
    }
}
