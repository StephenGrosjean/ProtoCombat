using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using InControl;


/// <summary>
/// Script for controlling the tank
/// </summary>
public class TankControl : MonoBehaviour
{
    [SerializeField] public int playerId;

    [Header("Part settings")] [SerializeField]
    private Transform turret;
    [SerializeField] private List<GameObject> loadingRings;
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
    [SerializeField] private float knockBackNormal, knockBackLarge;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float turretRotationSpeed;

    [SerializeField] private ForceMode moveForceMode;

    [Header("Dash Settings")]
    [SerializeField] private float dashPower;
    [SerializeField] private float dashCooldownTime;

    [Header("UI Settings")] [SerializeField]
    private bool useUI;

    /*[SerializeField] private Image reloadNormal; //Reload image for the normal shell
    [SerializeField] private Image reloadLarge; //Reload image for the big shell
    [SerializeField] private Image reloadDash; */

    [Header("ForceField Settings")] [SerializeField]
    private GameObject forceField; //Forcefield object

    [SerializeField] private Vector3 forcefieldSize; //Size of the forceField
    [SerializeField] private float shieldActivationSpeed; //Time for the shield to pop
    private bool shieldEnabled; //Is the shield is active?

    [Header("Death Settings")]
    [SerializeField] private List<GameObject> toggleOnDeath;

    [SerializeField] private MeshRenderer bodyRenderer, turretRenderer;
    [SerializeField] private TrailRenderer trackRenderer1, trackRenderer2;
    [SerializeField] private Color masterColor, clientColor;

    private PhotonView photonView;

    [SerializeField] private float bigShotHoldTime = 2;
    [SerializeField] private Light bigShotLight;
    private float currentHoldTime;
    private bool canShootBig;

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

    private SoundManager soundManager;

    public bool canControl = true;
    private float currentAngle;

    public bool gameIsInNetwork;
    private InputDevice playerDevice;


    //INPUTS
    private bool shootInput;
    private bool bigShootInput;
    private bool dashInput;


    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        /*reloadNormal = GameObject.Find("ReloadMachineGun").GetComponent<Image>();
        reloadDash = GameObject.Find("ReloadDash").GetComponent<Image>();*/

        //reloadLarge = GameObject.Find("ReloadLargeShot").GetComponent<Image>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
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

    void FixedUpdate()
        {
        // Debug.Log("playerId :" + playerId);
        // Debug.Log("isMaster :" + PhotonNetwork.IsMasterClient);
        // Debug.Log("isMine :" + photonView.IsMine);
        // Debug.Log("gameIsInNetwork :" + gameIsInNetwork);
        if (!canControl && ((gameIsInNetwork && !photonView.IsMine && playerDevice == null) ||
            (!gameIsInNetwork && playerDevice != null)))
        {
            //Debug.Log("Passed : false");
            return;
        }

        //Debug.Log("Passed : true");


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

        //Increase Hold Time
        if (bigShootInput) {
            if (currentHoldTime < bigShotHoldTime) {
                currentHoldTime += Time.deltaTime;
            }
            else if(currentHoldTime >= bigShotHoldTime){
                canShootBig = true;
            }
        }
        else {
            currentHoldTime = 0;
            canShootBig = false;
        }

        

        //MOVE
        if (GameInput.GetDirection(GameInput.DirectionType.L_INPUT, Vector2.zero, playerDevice).magnitude > 0.01f) {
            if(gameIsInNetwork)
                photonView.RPC("MoveTankRPC", RpcTarget.All, GameInput.GetDirection(GameInput.DirectionType.L_INPUT, Vector2.zero));
            else
                MoveTank(GameInput.GetDirection(GameInput.DirectionType.L_INPUT, Vector2.zero, playerDevice));
        }

        /*
        rigid.AddRelativeForce(new Vector3(GameInput.GetAxis(GameInput.AxisType.L_VERTICAL), 0, 0) * speed, moveForceMode);

        if (Math.Abs(GameInput.GetAxis(GameInput.AxisType.L_VERTICAL)) > 0.01f)
        {
            photonView.RPC("SyncMovementRPC", RpcTarget.Others, rigid.velocity, rigid.position);
        }
        */


        //Rotate the tank 
        /*transform.Rotate(new Vector3(0, GameInput.GetAxis(GameInput.AxisType.L_HORIZONTAL), 0) * rotationSpeed);

        if (Math.Abs(GameInput.GetAxis(GameInput.AxisType.L_HORIZONTAL)) > 0.01f)
        {
            photonView.RPC("SyncRotationRPC", RpcTarget.Others, transform.rotation);
        }*/

        //RELOAD UI
        /*if (useUI) {
            reloadNormal.fillAmount = quickFireReloadTime / quickShootingSpeed;
            //reloadLarge.fillAmount = bigFireReloadTime / bigShootingSpeed;
            reloadDash.fillAmount = dashReloadTime / dashCooldownTime;
        }*/

        //MOVE CANNON
        /*Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = GameInput.GetDirection(GameInput.DirectionType.R_INPUT, screenPos, playerDevice);
        if (Math.Abs(direction.x) > 0.0f && Math.Abs(direction.y) > 0.0f)
        {
            angle = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;

            if (gameIsInNetwork)
                photonView.RPC("RotateTurretRPC", RpcTarget.All, direction);
            else
                RotateTurret(direction);
        }*/

        //FIRE
        if (shootInput && quickFireReloadTime >= quickShootingSpeed)
        {
            quickFireReloadTime = 0;
            Camera.main.GetComponent<CameraShake>().ShakeCam(.2f,.5f);

            GameObject obj;
            if (gameIsInNetwork)
            {
                obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "smallShell"),
                    shootingPoint.position, Quaternion.Euler(new Vector3(0, turret.transform.rotation.y + 180.0f, 0)));

                obj.GetComponent<PhotonView>().RPC("InitializeShellRPC", RpcTarget.All, this.playerId, turret.transform.rotation);
            }
            else
            {
                obj = Instantiate(smallShellPrefab, shootingPoint.position, Quaternion.Euler(new Vector3(0, turret.transform.rotation.y + 180.0f, 0)));
                obj.GetComponent<TankShell>().InitializeShell(this.playerId, turret.transform.rotation);
            }

            rigid.AddRelativeForce(-Vector3.right * knockBackNormal);
        }

        //BIG_FIRE
        if (bigShootInput && canShootBig) {
            bigFireReloadTime = 0;
            Camera.main.GetComponent<CameraShake>().ShakeCam(.4f, 1f);

            GameObject obj;
            if (gameIsInNetwork) {
                obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "bigShell"),
                    shootingPoint.position, Quaternion.Euler(new Vector3(0, turret.transform.rotation.y + 180.0f, 0)));

                obj.GetComponent<PhotonView>().RPC("InitializeShellRPC", RpcTarget.All, this.playerId, turret.transform.rotation);
            }
            else {
                obj = Instantiate(bigShellPrefab, shootingPoint.position, Quaternion.Euler(new Vector3(0, turret.transform.rotation.y + 180.0f, 0)));
                obj.GetComponent<TankShell>().InitializeShell(this.playerId, turret.transform.rotation);
            }
            currentHoldTime = 0;
            canShootBig = false;
            rigid.AddRelativeForce(-Vector3.right * knockBackLarge);
        }

        //DASH
        //TODO : LE RENDRE NETWORKED
        if (dashInput && dashReloadTime >= dashCooldownTime)
        {
            dashReloadTime = 0;
            rigid.AddRelativeForce(Vector3.right * dashPower, moveForceMode);
            
        }

        shootInput = false;
        dashInput = false;
    }

    private void Update()
    {
        //UPDATE INPUTS
        shootInput = shootInput || GameInput.GetInputDown(GameInput.InputType.SHOOT, playerDevice);
        bigShootInput = GameInput.GetInput(GameInput.InputType.BIG_SHOOT, playerDevice);
        dashInput = dashInput ||  GameInput.GetInputDown(GameInput.InputType.DASH, playerDevice);


        if (gameIsInNetwork) {
            photonView.RPC("EnableRings", RpcTarget.All);
        }
        else {
            EnableRings();
        }

        //Add tank forward speed if under the maxSpeed limit
        if (rigid.velocity.magnitude >= maxSpeed)
        {
            rigid.velocity = rigid.velocity.normalized * maxSpeed;
        }
        /*
            //LARGE FIRE

            if (GameInput.GetInputDown(GameInput.InputType.SHOOT) && bigFireReloadTime >= bigShootingSpeed) {
                bigFireReloadTime = 0;
                Camera.main.GetComponent<CameraShake>().ShakeCam(.2f, 0.5f);
                //GameObject obj = Instantiate(largeShell, shootingPoint.position, transform.rotation);
                GameObject obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "largeShell"), shootingPoint.position, turret.rotation);

                obj.GetComponent<TankShell>().TypeShell = TankShell.ShellType.Large;
                obj.GetComponent<TankShell>().SetLauncherParent(this.gameObject);
                rigid.AddRelativeForce(-Vector3.left * knockBack * 5);
            }

            //FORCEFIELD
            if (GameInput.GetInputDown(GameInput.InputType.DEFENSE)) {
                shieldEnabled = !shieldEnabled;
                soundManager.PlaySound(SoundManager.SoundList.SHIELD);
            }

            forceField.transform.localScale = Vector3.Lerp(Vector3.zero, forcefieldSize, shieldActivationTime);


            //TRAIL (NOT USED)
             if(rigid.velocity.magnitude != 0 && spawnTrail) {
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
    }

    /*
    void OnCollisionEnter(Collision collision)
    {
        soundManager.PlaySound(SoundManager.SoundList.STRIKE);
    }
    */

    public void ToggleRenderersNetwork(bool value) {
        photonView.RPC("ToggleRenderersRPC", RpcTarget.All, value);
    }

    [PunRPC]
    void ToggleRenderersRPC(bool value)
    {
        ToggleRenderers(value);
    }

    public void ToggleRenderers(bool value)
    {
        foreach(GameObject obj in toggleOnDeath) {
            obj.SetActive(value);
        }
        canControl = value;
    }

    [PunRPC]
    void SyncMovementRPC(Vector3 velocity, Vector3 position, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        rigid.velocity = velocity;
        rigid.position = position + velocity * lag;
    }

    [PunRPC]
    void SyncRotationRPC(Quaternion rotation, PhotonMessageInfo info)
    {
        transform.rotation = rotation;
    }

    [PunRPC]
    void RotateTurretRPC(Vector2 direction)
    {
        RotateTurret(direction);
    }

    void RotateTurret(Vector2 direction)
    {
        //Rotate turret toward direction
        float step = turretRotationSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(turret.transform.forward, new Vector3(direction.x, 0, direction.y) * -1, step, 0.0f);
        turret.transform.rotation = Quaternion.LookRotation(newDir);
    }

    [PunRPC]
    void MoveTankRPC(Vector2 movementInput, PhotonMessageInfo info)
    {
        MoveTank(movementInput);
    }

    void MoveTank(Vector2 movementInput)
    {
        //Rotate tank toward direction
        float step = rotationSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, new Vector3(movementInput.x, 0, movementInput.y), step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);

        //Move tank
        rigid.AddForce(transform.right * speed, moveForceMode);
    }

    [PunRPC]
    public void SetupNetworkRPC()
    {
        gameIsInNetwork = true;
        GetComponent<TankNetwork>().enabled = true;
        GetComponent<PhotonTransformView>().enabled = true;
        playerDevice = null;
    }

    public void SetupPlayer(int playerId, bool inNetwork, InputDevice device = null)
    {
        this.playerId = playerId;

        if (!inNetwork && device != null)
        {
            playerDevice = device;
            gameIsInNetwork = false;
            GetComponent<TankNetwork>().enabled = false;
            GetComponent<PhotonTransformView>().enabled = false;
        }
        else
        {
            photonView.RPC("SetupNetworkRPC", RpcTarget.All);
        }

        GetComponent<TankHealth>().inNetwork = inNetwork;

        canControl = true;
        SetupColor();
    }

    private void SetupColor()
    {
        switch (PhotonNetwork.IsMasterClient)
        {
            case false:
                GetComponent<TankColor>().SetColor(masterColor);
                break;
            case true:
                GetComponent<TankColor>().SetColor(clientColor);

                break;
        }
    }

    [PunRPC]
    private void EnableRings() {

        int percentage = Mathf.RoundToInt((13 / bigShotHoldTime) * currentHoldTime);
        int percentageLight = Mathf.RoundToInt((88 / bigShotHoldTime) * currentHoldTime);

        //Debug.Log("HoldTime : " + currentHoldTime + " Percentage : " + percentage);
        foreach(GameObject ring in loadingRings) {
            DisableRing(ring);
        }
        for(int i = 0; i < percentage; i++) {
            EnableRing(loadingRings[i]);
        }

        bigShotLight.spotAngle = percentageLight;

    }

    void EnableRing(GameObject ring) {
        Renderer ringRenderer = ring.GetComponent<Renderer>();
        ringRenderer.material.color = Color.red;
        ringRenderer.material.SetColor("_EmissionColor", Color.red * 100);
    }

    void DisableRing(GameObject ring) {
        Renderer ringRenderer = ring.GetComponent<Renderer>();
        ringRenderer.material.color = Color.white;
        ringRenderer.material.SetColor("_EmissionColor", Color.white);
    }
}
