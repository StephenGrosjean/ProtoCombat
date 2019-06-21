﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for controling the tank
/// </summary>

public class TankControl : MonoBehaviour
{
    //[SerializeField] private int playerID; (NOT USED)

    [Header("Fire Settings")]
    [SerializeField] private GameObject smallShell;
    [SerializeField] private GameObject largeShell;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float quickShootingSpeed;
    [SerializeField] private float bigShootingSpeed;

    [Header("Control Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float knockBack;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private ForceMode moveForceMode;

    [Header("UI Settings")]
    [SerializeField] private bool useUI;
    [SerializeField] private Image reloadNormal; //Reload image for the normal shell
    [SerializeField] private Image reloadLarge; //Reload image for the big shell

    [Header("ForceField Settings")]
    [SerializeField] private GameObject forceField; //Forcefield object
    [SerializeField] private Vector3 forcefieldSize; //Size of the forceField
    [SerializeField] private float shieldActivationSpeed; //Time for the shield to pop
    private bool shieldEnabled; //Is the shield is active?

    //NOT USED
    /*[Header("Trail Settings")]
    [SerializeField] private bool spawnTrail;
    [SerializeField] private GameObject trailLight;
    [SerializeField] private Transform[] tracks;
    [SerializeField] private float lightSpacing;
    [SerializeField] private int maxTrailLights = 100;

    [Header("Containers")]
    [SerializeField] private Transform trailContainer;

    private List<GameObject> trailLights = new List<GameObject>();
    private float spawnLightTimer;*/

    private Rigidbody rigid; //Rigidbody of the tank
    private float quickFireReloadTime, bigFireReloadTime; //Current reload time of each shot
    private float shieldActivationTime; //Current shield activation time
    

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Increase shield activation Time
        if (shieldActivationTime < shieldActivationSpeed && shieldEnabled) {
            shieldActivationTime += Time.deltaTime;
        }
        //Decrease shield activation Time
        else if (!shieldEnabled && shieldActivationTime >= 0) {
            shieldActivationTime -= Time.deltaTime;
        }

        //Increase small shell reloadTime
        if (quickFireReloadTime < quickShootingSpeed) {
            quickFireReloadTime += Time.deltaTime;
        }

        //Increase big shell reloadTime
        if (bigFireReloadTime < bigShootingSpeed) {
            bigFireReloadTime += Time.deltaTime;
        }

        //Add tank forward speed if under the maxSpeed limit
        if (rigid.velocity.magnitude <= maxSpeed) {
            rigid.AddRelativeForce(new Vector3(Input.GetAxis("Vertical"), 0, 0) * speed, moveForceMode);
        }

        //Rotate the tank 
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0)*rotationSpeed);

        //RELOAD UI
        if (useUI) {
            reloadNormal.fillAmount = quickFireReloadTime / quickShootingSpeed;
            reloadLarge.fillAmount = bigFireReloadTime / bigShootingSpeed;
        }

        //FIRE
        if (Input.GetMouseButton(0) && quickFireReloadTime >= quickShootingSpeed) {
            quickFireReloadTime = 0;
            Camera.main.GetComponent<CameraShake>().ShakeCam(.1f, 0.1f);
            GameObject obj = Instantiate(smallShell, firePoint.position, transform.rotation);
            obj.GetComponent<TankShell>().SetLauncherParent(this.gameObject);

            rigid.AddRelativeForce(Vector3.left * knockBack);
        }

        //LARGE FIRE
        if (Input.GetKeyDown(KeyCode.X) && bigFireReloadTime >= bigShootingSpeed) {
            bigFireReloadTime = 0;
            Camera.main.GetComponent<CameraShake>().ShakeCam(.2f, 0.5f);
            GameObject obj = Instantiate(largeShell, firePoint.position, transform.rotation);
            obj.GetComponent<TankShell>().TypeShell = TankShell.ShellType.Large;
            obj.GetComponent<TankShell>().SetLauncherParent(this.gameObject);
            rigid.AddRelativeForce(Vector3.left * knockBack * 5);
        }


        //FORCEFIELD
        if (Input.GetKeyDown(KeyCode.F)) {
            shieldEnabled = !shieldEnabled;
        }
        forceField.transform.localScale = Vector3.Lerp(Vector3.zero, forcefieldSize, shieldActivationTime);


        //TRAIL (NOT USED)
        /* if(rigid.velocity.magnitude != 0 && spawnTrail) {
             spawnLightTimer += Time.deltaTime;
             if(spawnLightTimer > lightSpacing) {
                 spawnLightTimer = 0;
                 GameObject lightTrail1 = Instantiate(trailLight, new Vector3(tracks[0].position.x, -2.25f, tracks[0].position.z), trailLight.transform.rotation);
                 GameObject lightTrail2 = Instantiate(trailLight, new Vector3(tracks[1].position.x, -2.25f, tracks[1].position.z), trailLight.transform.rotation);
                 trailLights.Add(lightTrail1);
                 trailLights.Add(lightTrail2);

                 lightTrail1.transform.SetParent(trailContainer);
                 lightTrail2.transform.SetParent(trailContainer);

             }
         }

         if(trailLights.Count > maxTrailLights) {
             trailLights.RemoveAt(0);
             Destroy(trailLights[0]);
         }
         */

        //SKY BOMBING
        if (Input.GetKeyDown(KeyCode.B)) {
            GetComponent<SkyShellSpawning>().StartBombardment();
        }
    }


}
