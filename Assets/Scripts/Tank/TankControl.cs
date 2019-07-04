﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.IO;


/// <summary>
/// Script for controlling the tank
/// </summary>
public class TankControl : MonoBehaviour
{
    [SerializeField] public int playerId;

    [Header("Part settings")] [SerializeField]
    private Transform turret;
    //[SerializeField] private Transform body;

    [Header("Fire Settings")] 
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float quickShootingSpeed;
    [SerializeField] private float bigShootingSpeed;

    [SerializeField] private GameObject smallShellPrefab;
    [SerializeField] private GameObject bigShellPrefab;

    [Header("Control Settings")] [SerializeField]
    private float speed;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float knockBack;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private ForceMode moveForceMode;

    [Header("Dash Settings")]
    [SerializeField] private float dashPower;
    [SerializeField] private float dashCooldownTime;

    [Header("UI Settings")] [SerializeField]
    private bool useUI;

    [SerializeField] private Image reloadNormal; //Reload image for the normal shell
    [SerializeField] private Image reloadLarge; //Reload image for the big shell
    [SerializeField] private Image reloadDash; 

    [Header("ForceField Settings")] [SerializeField]
    private GameObject forceField; //Forcefield object

    [SerializeField] private Vector3 forcefieldSize; //Size of the forceField
    [SerializeField] private float shieldActivationSpeed; //Time for the shield to pop
    private bool shieldEnabled; //Is the shield is active?


    [SerializeField] private MeshRenderer bodyRenderer, turretRenderer;
    [SerializeField] private TrailRenderer trackRenderer1, trackRenderer2;
    [SerializeField] private Color masterColor, clientColor;

    private PhotonView photonView;

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
    private float quickFireReloadTime, bigFireReloadTime, dashReloadTime; //Current reload time of each shot
    private float shieldActivationTime; //Current shield activation time

    private float angle;
    public bool controllable; // Is true if the tank is spawned

    private SoundManager soundManager;

    private bool canControl = true;
    private float currentAngle;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        reloadNormal = GameObject.Find("ReloadMachineGun").GetComponent<Image>();
        reloadDash = GameObject.Find("ReloadDash").GetComponent<Image>();

        //reloadLarge = GameObject.Find("ReloadLargeShot").GetComponent<Image>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        if (PhotonNetwork.IsMasterClient) {
            if (photonView.IsMine) {
                playerId = 0;
                SetBodyColor(masterColor);
                SetTurretColor(masterColor);
                SetTrackColor(masterColor);
            }
            else {
                playerId = 1;
                SetBodyColor(clientColor);
                SetTurretColor(clientColor);
                SetTrackColor(clientColor);
            }
        }
        else {
            if (photonView.IsMine) {
                playerId = 1;
                SetBodyColor(clientColor);
                SetTurretColor(clientColor);
                SetTrackColor(clientColor);
            }
            else {
                playerId = 0;
                SetBodyColor(masterColor);
                SetTurretColor(masterColor);
                SetTrackColor(masterColor);
            }
        }
    }

    void SetBodyColor(Color color) {
        bodyRenderer.material.color = color;
        bodyRenderer.material.SetColor("_EmissionColor", color);
    }

    void SetTurretColor(Color color) {
        turretRenderer.material.color = color;
        turretRenderer.material.SetColor("_EmissionColor", color);
    }

    void SetTrackColor(Color color) {
        trackRenderer1.material.color = color;
        trackRenderer1.material.SetColor("_EmissionColor", color);

        trackRenderer2.material.color = color;
        trackRenderer2.material.SetColor("_EmissionColor", color);
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    void FixedUpdate() {
        if (!photonView.IsMine || !controllable) {
            return;
        }

        //Increase shield activation Time
        if (shieldEnabled && shieldActivationTime < shieldActivationSpeed) {
            shieldActivationTime += Time.deltaTime;
        }
        //Decrease shield activation Time
        else if (!shieldEnabled && shieldActivationTime >= 0) {
            shieldActivationTime -= Time.deltaTime;
        }

        //Increase dash reload time
        if (dashReloadTime < dashCooldownTime) {
            dashReloadTime += Time.deltaTime;
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
            rigid.AddRelativeForce(new Vector3(GameInput.GetAxis(GameInput.AxisType.L_VERTICAL), 0, 0) * speed, moveForceMode);
        }

        if (canControl) {
            //Rotate the tank 
            transform.Rotate(new Vector3(0, GameInput.GetAxis(GameInput.AxisType.L_HORIZONTAL), 0) * rotationSpeed);

            //turret.Rotate(new Vector3(0, GameInput.GetAxis(GameInput.AxisType.R_HORIZONTAL), 0) * rotationSpeed);

            //DASH
            if (GameInput.GetInputDown(GameInput.InputType.DASH) && dashReloadTime >= dashCooldownTime) {
                dashReloadTime = 0;
                rigid.AddRelativeForce(Vector3.right * dashPower, moveForceMode);
            }

            //RELOAD UI
            if (useUI) {
                reloadNormal.fillAmount = quickFireReloadTime / quickShootingSpeed;
                //reloadLarge.fillAmount = bigFireReloadTime / bigShootingSpeed;
                reloadDash.fillAmount = dashReloadTime / dashCooldownTime;

            }

            //MOVE CANNON
            /*Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 direction = GameInput.GetDirection(GameInput.DirectionType.R_INPUT, screenPos);
            Debug.Log(direction);
            if (Math.Abs(direction.x) > 0.0f && Math.Abs(direction.y) > 0.0f) {
                angle = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90.0f;
                turret.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
            }*/

            float input = GameInput.GetAxis(GameInput.AxisType.R_HORIZONTAL) * 10;
            turret.Rotate(0.0f, angle, 0.0f);

        }
    }

    private void Update() {
        if (canControl) { 
            //FIRE
            if (GameInput.GetInputDown(GameInput.InputType.SHOOT) && quickFireReloadTime >= quickShootingSpeed) {
                quickFireReloadTime = 0;
                Camera.main.GetComponent<CameraShake>().ShakeCam(.1f, 0.1f);
                //GameObject obj = Instantiate(smallShell, shootingPoint.position, transform.rotation);
                GameObject obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "smallShell"),
                    shootingPoint.position, Quaternion.Euler(new Vector3(0, turret.transform.rotation.y + 180.0f, 0)));

                obj.GetComponent<TankShell>().GetComponent<PhotonView>().RPC("InitializeShell", RpcTarget.All, this.playerId, turret.transform.rotation);

                rigid.AddRelativeForce(-turret.transform.forward * knockBack);
            }

            //LARGE FIRE
            /*
            if (GameInput.GetInputDown(GameInput.InputType.SHOOT) && bigFireReloadTime >= bigShootingSpeed) {
                bigFireReloadTime = 0;
                Camera.main.GetComponent<CameraShake>().ShakeCam(.2f, 0.5f);
                //GameObject obj = Instantiate(largeShell, shootingPoint.position, transform.rotation);
                GameObject obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "largeShell"), shootingPoint.position, turret.rotation);

                obj.GetComponent<TankShell>().TypeShell = TankShell.ShellType.Large;
                obj.GetComponent<TankShell>().SetLauncherParent(this.gameObject);
                rigid.AddRelativeForce(-Vector3.left * knockBack * 5);
            }*/

            //FORCEFIELD
            /*if (GameInput.GetInputDown(GameInput.InputType.DEFENSE)) {
                shieldEnabled = !shieldEnabled;
                soundManager.PlaySound(SoundManager.SoundList.SHIELD);
            }

            forceField.transform.localScale = Vector3.Lerp(Vector3.zero, forcefieldSize, shieldActivationTime);
            */

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
            /* if (GameInput.GetInputDown(GameInput.InputType.DASH)) {
                 GetComponent<SkyShellSpawning>().StartBombardment();
             }*/
        }
    }

    public void Fire(Vector3 position, float aAngle)
    {
        //float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        Camera.main.GetComponent<CameraShake>().ShakeCam(.1f, 0.1f);

        GameObject shell = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "smallShell"),
            position, Quaternion.Euler(new Vector3(0, aAngle + 180.0f, 0)));
        
        //rigid.AddRelativeForce(-Vector3.left * knockBack);
    }

    void OnCollisionEnter(Collision collision)
    {
        soundManager.PlaySound(SoundManager.SoundList.STRIK);
    }

    public void ToggleRenderers(bool value) {
        photonView.RPC("ToggleRenderersRPC", RpcTarget.All, value);
    }

    [PunRPC]
    void ToggleRenderersRPC(bool value) {
        trackRenderer1.enabled = value;
        trackRenderer2.enabled = value;
        bodyRenderer.enabled = value;
        turretRenderer.enabled = value;
        canControl = value;
    }
}
