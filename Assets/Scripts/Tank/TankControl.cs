using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using InControl;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for controlling the tank
/// </summary>
public class TankControl : MonoBehaviour
{
    [SerializeField] public int playerId;
    [SerializeField] private Animator animator;

    [Header("Layers")]
    [SerializeField] private int layerMaster;
    [SerializeField] private int layerClient;

    [Header("Part settings")] [SerializeField]
    private Transform turret;

    [SerializeField] private List<GameObject> loadingRings;
    //[SerializeField] private Transform body;

    [Header("Fire Settings")] [SerializeField]
    private Transform shootingPoint;

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

    [Header("Dash Settings")] [SerializeField]
    private float dashPower;
    [SerializeField] private Light dashLight;

    [SerializeField] private float dashCooldownTime;
    [SerializeField] private float timeToDash = 1.2f;
    [SerializeField] private GameObject dashRing;
    private float LoadDash = 1;
    private float MaxDash = 2;
    private bool isInvulnerable = false;
    private float TimeToInvulnerable = 0;

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
    [SerializeField] private float shieldTimeOut;
    [SerializeField] private float shieldBreakCooldownMultiplier = 4.0f;
    [SerializeField] private float shieldCooldown;
    private bool canActivateShield = true;
    

    [Header("Death Settings")]
    [SerializeField] private List<GameObject> toggleOnDeath;

    [SerializeField] private MeshRenderer bodyRenderer, turretRenderer;
    [SerializeField] private TrailRenderer trackRenderer1, trackRenderer2;
    [SerializeField] private Color masterColor, clientColor;

    private PhotonView photonView;

    [SerializeField] private float bigShotHoldTime = 2;
    [SerializeField] private int maxDamageBigShell = 4;
    [SerializeField] private Light bigShotLight;
    private float currentHoldTime;

    public bool isDash = false;
    public bool IsCritical = false;

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

    public bool canControl = false;
    private float currentAngle;

    public bool gameIsInNetwork;
    private InputDevice playerDevice;

    //INPUTS
    private bool shootInput;
    private bool bigShootInput;
    private bool bigShootInputUp;
    private bool dashInput;
    private bool dashInputUp;
    private bool shieldInput;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        /*reloadNormal = GameObject.Find("ReloadMachineGun").GetComponent<Image>();
        reloadDash = GameObject.Find("ReloadDash").GetComponent<Image>();*/

        //reloadLarge = GameObject.Find("ReloadLargeShot").GetComponent<Image>();
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        /*if(SceneManager.GetActiveScene().name == "LocalArena") {
            if(gameObject.name == "Player 1") {
                gameObject.layer = layerMaster;
            }
            else {
                gameObject.layer = layerClient;
            }
        }
        else {
            if (PhotonNetwork.IsMasterClient) {
                gameObject.layer = layerMaster;
            }
            else {
                gameObject.layer = layerClient;
            }
        }*/
        LoadDash = 0;
    }

    void SetBodyColor(Color color)
    {
        bodyRenderer.material.color = color;
        bodyRenderer.material.SetColor("_EmissionColor", color);
    }

    void SetTurretColor(Color color)
    {
        turretRenderer.material.color = color;
        turretRenderer.material.SetColor("_EmissionColor", color);
    }

    void SetTrackColor(Color color)
    {
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
        if (!canControl && ((gameIsInNetwork && !photonView.IsMine && playerDevice == null) || (!gameIsInNetwork && playerDevice != null)))
        {
            return;
        }
        if (!canControl) { return; }
        //Debug.Log("Passed : true");
        if (isInvulnerable)
        {
            TimeToInvulnerable -= Time.deltaTime;
            if (TimeToInvulnerable <= 0)
            {
                if (gameIsInNetwork)
                {
                    photonView.RPC("SyncInvulnerability", RpcTarget.All, false);
                }
                else
                {
                    isInvulnerable = false;
                }
            }
        }

        //Increase shield activation Time
        if (shieldEnabled && shieldActivationTime < shieldActivationSpeed)
        {
            shieldActivationTime += Time.deltaTime;
        }
        //Decrease shield activation Time
        else if (!shieldEnabled && shieldActivationTime >= 0)
        {
            shieldActivationTime -= Time.deltaTime;
        }

        //Increase dash reload time
        if (dashReloadTime < dashCooldownTime)
        {
            dashReloadTime += Time.deltaTime;
            if (dashReloadTime > timeToDash)
            {
                if (gameIsInNetwork)
                {
                    photonView.RPC("SyncDash", RpcTarget.All, false);
                    photonView.RPC("SyncCritical", RpcTarget.All, false);
                }
                else
                {
                    isDash = false;
                    IsCritical = false;
                }
            }
        }

        //Increase small shell reloadTime
        if (quickFireReloadTime < quickShootingSpeed)
        {
            quickFireReloadTime += Time.deltaTime;
        }

        //Increase big shell reloadTime
        if (bigFireReloadTime < bigShootingSpeed)
        {
            bigFireReloadTime += Time.deltaTime;
        }

        //Increase Hold Time
        if (bigShootInput)
        {
            currentHoldTime += Time.deltaTime;
        }

        //MOVE
        if (GameInput.GetDirection(GameInput.DirectionType.L_INPUT, Vector2.zero, playerDevice).magnitude > 0.01f)
        {
            if (gameIsInNetwork)
                photonView.RPC("MoveTankRPC", RpcTarget.All,
                    GameInput.GetDirection(GameInput.DirectionType.L_INPUT, Vector2.zero));
            else
                MoveTank(GameInput.GetDirection(GameInput.DirectionType.L_INPUT, Vector2.zero, playerDevice));
        }

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

                obj.GetComponent<PhotonView>().RPC("InitializeShellRPC", RpcTarget.All, this.playerId,
                    turret.transform.rotation, 1);
            }
            else
            {
                obj = Instantiate(smallShellPrefab, shootingPoint.position,
                    Quaternion.Euler(new Vector3(0, turret.transform.rotation.y + 180.0f, 0)));
                obj.GetComponent<TankShell>().InitializeShell(this.playerId, turret.transform.rotation);
            }

            rigid.AddRelativeForce(-Vector3.right * knockBackNormal);
        }

        //BIG_FIRE
        if (bigShootInput && currentHoldTime > 0.2f) {
            if (gameIsInNetwork) {
                photonView.RPC("StopVelocity", RpcTarget.All);
            }
            else {
                rigid.velocity = Vector3.zero;
            }
        }

        if (bigShootInputUp)
        {
            if (currentHoldTime >= (bigShotHoldTime / maxDamageBigShell))
            {
                bigFireReloadTime = 0;
                Camera.main.GetComponent<CameraShake>().ShakeCam(.4f, 1f);
                int damageDealt = (int)(currentHoldTime / (bigShotHoldTime / (float)maxDamageBigShell));
                if (damageDealt > maxDamageBigShell)
                    damageDealt = maxDamageBigShell;
                
                GameObject obj;
                if (gameIsInNetwork)
                {
                    obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "bigShell"),
                        shootingPoint.position, Quaternion.Euler(new Vector3(0, turret.transform.rotation.y + 180.0f, 0)));

                    obj.GetComponent<PhotonView>().RPC("InitializeShellRPC", RpcTarget.All, this.playerId, turret.transform.rotation, damageDealt);
                }
                else
                {
                    obj = Instantiate(bigShellPrefab, shootingPoint.position,
                        Quaternion.Euler(new Vector3(0, turret.transform.rotation.y + 180.0f, 0)));
                    obj.GetComponent<TankShell>().InitializeShell(this.playerId, turret.transform.rotation, damageDealt);
                }
                rigid.AddRelativeForce(-Vector3.right * knockBackLarge);
            }

            currentHoldTime = 0;
        }

        //LOAD DASH
        //TODO : LE RENDRE NETWORKED
        if (dashInput && dashReloadTime >= dashCooldownTime)
        {
            if (gameIsInNetwork)
            {
                photonView.RPC("StopVelocity", RpcTarget.All);
            }
            else
            {
                rigid.velocity = Vector3.zero;
            }

            if (LoadDash <= MaxDash)
            {
                
                LoadDash += Time.deltaTime;
                if (gameIsInNetwork) {
                    photonView.RPC("SyncAnimation", RpcTarget.All, "TankShake");
                }
                else {
                    SyncAnimation("TankShake");
                }
            }
            else
            {
                if (gameIsInNetwork)
                {
                    photonView.RPC("SyncCritical", RpcTarget.All, true);
                    photonView.RPC("SyncAnimation", RpcTarget.All, "None");

                }
                else
                {
                    IsCritical = true;
                    SyncAnimation("None");
                }
            }
        }

        if(LoadDash > 0) {
            if (gameIsInNetwork) {
                photonView.RPC("SyncDashRing", RpcTarget.All);
            }
            else {
                SyncDashRing();
            }
        }

        //FORCEFIELD
        if (shieldInput && !shieldEnabled && canActivateShield)
        {
            shieldEnabled = true;
            canActivateShield = false;
            StartCoroutine("ShieldTimeOut", 0);
            soundManager.PlaySound(SoundManager.SoundList.SHIELD);
        }
        forceField.transform.localScale = Vector3.Lerp(Vector3.zero, forcefieldSize, shieldActivationTime);

        shootInput = false;
        dashInput = false;
        shieldInput = false;
        bigShootInput = false;
        bigShootInputUp = false;
        dashInputUp = false;
    }

    private void Update()
    {
        
        //UPDATE INPUTS
        shootInput = shootInput || GameInput.GetInputUp(GameInput.InputType.SHOOT, playerDevice);
        bigShootInput = bigShootInput || GameInput.GetInput(GameInput.InputType.SHOOT, playerDevice);
        bigShootInputUp = bigShootInputUp || GameInput.GetInputUp(GameInput.InputType.SHOOT, playerDevice);
        shieldInput = shieldInput ||  GameInput.GetInputDown(GameInput.InputType.DEFENSE, playerDevice);
        dashInput = dashInput || GameInput.GetInput(GameInput.InputType.DASH, playerDevice);
        dashInputUp = dashInputUp || GameInput.GetInputUp(GameInput.InputType.DASH, playerDevice);
        
        if (gameIsInNetwork) {
            photonView.RPC("EnableRings", RpcTarget.All);
        }
        else
        {
            EnableRings();
        }

        //Add tank forward speed if under the maxSpeed limit
        if (rigid.velocity.magnitude >= maxSpeed) {
            rigid.velocity = rigid.velocity.normalized * maxSpeed;
        }


        //DASH
        if (dashInputUp) {
            if (LoadDash > 1) {
                LoadDash = 1;
                dashReloadTime = 0;
                if (gameIsInNetwork) {
                    photonView.RPC("SyncDash", RpcTarget.All, true);
                }
                else {
                    Debug.Log("IsDash = true");
                    isDash = true;
                    SyncDashRing(true);
                }

                rigid.AddRelativeForce(Vector3.right * dashPower * LoadDash, moveForceMode);
                LoadDash = 0;
            }
            else {
                SyncDashRing(true);
                LoadDash = 0;
                if (gameIsInNetwork) {
                    photonView.RPC("SyncAnimation", RpcTarget.All, "None");
                    SyncDashRing(true);

                }
                else {
                    SyncAnimation("None");
                    SyncDashRing(true);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Coucou");
        Debug.Log(playerId);
        Debug.Log(isInvulnerable);
        if (!isInvulnerable)
        {
            TankControl otherPlayer = collision.gameObject.GetComponent<TankControl>();
            Debug.Log(otherPlayer);
            Debug.Log(collision.gameObject.layer == LayerMask.NameToLayer("Tank"));
            Debug.Log(otherPlayer.isDash);
            Debug.Log("----------------");
            //soundManager.PlaySound(SoundManager.SoundList.STRIKE);
            if (collision.gameObject.layer == LayerMask.NameToLayer("Tank") && otherPlayer.isDash)
            {
                TimeToInvulnerable = 1;
                isInvulnerable = true;
                if (gameIsInNetwork)
                {
                    photonView.RPC("SyncInvulnerability", RpcTarget.All, true);
                }

                if (shieldEnabled)
                {
                    Debug.Log("Shield break");
                    shieldEnabled = false;
                    canActivateShield = false;
                    shieldActivationTime = 0.0f;
                    StartCoroutine("ShieldTimeOut", shieldBreakCooldownMultiplier);
                    //Take one extra damage
                    GetComponent<TankHealth>().TakeDamage(1);
                }

                if (otherPlayer.IsCritical)
                {
                    GetComponent<TankHealth>().TakeDamage(2);
                }
                else
                {
                    GetComponent<TankHealth>().TakeDamage(1);
                }
                rigid.AddRelativeForce(otherPlayer.transform.forward * dashPower / 2, moveForceMode);
            }
          
        }

        if (gameIsInNetwork)
        {
            photonView.RPC("SyncDash", RpcTarget.All, false);
        }
        else
        {
            isDash = false;
        }
    }

    public void ToggleRenderersNetwork(bool value)
    {
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
        float lag = (float) (PhotonNetwork.Time - info.SentServerTime);

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
        Vector3 newDir = Vector3.RotateTowards(turret.transform.forward, new Vector3(direction.x, 0, direction.y) * -1,
            step, 0.0f);
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
        Vector3 newDir = Vector3.RotateTowards(transform.forward, new Vector3(movementInput.x, 0, movementInput.y),
            step, 0.0f);
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

       // canControl = true;
        //SetupColor();
    }

    /*private void SetupColor()
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
    }*/

    [PunRPC]
    private void EnableRings()
    {
        int percentage = Mathf.RoundToInt((13 / bigShotHoldTime) * currentHoldTime);
        int percentageLight = Mathf.RoundToInt((88 / bigShotHoldTime) * currentHoldTime);

        //Debug.Log("HoldTime : " + currentHoldTime + " Percentage : " + percentage);
        foreach (GameObject ring in loadingRings)
        {
            DisableRing(ring);
        }

        for (int i = 0; i < percentage; i++)
        {
            EnableRing(loadingRings[i]);
        }

        bigShotLight.spotAngle = percentageLight;
    }

    void EnableRing(GameObject ring)
    {
        Renderer ringRenderer = ring.GetComponent<Renderer>();
        ringRenderer.material.color = Color.red;
        ringRenderer.material.SetColor("_EmissionColor", Color.red * 100);
    }

    void DisableRing(GameObject ring)
    {
        Renderer ringRenderer = ring.GetComponent<Renderer>();
        ringRenderer.material.color = Color.white;
        ringRenderer.material.SetColor("_EmissionColor", Color.white);
    }

    [PunRPC]
    void StopVelocity(PhotonMessageInfo info)
    {
        rigid.velocity = Vector3.zero;
    }

    [PunRPC]
    public void SyncDash(bool value, PhotonMessageInfo info)
    {
        isDash = value;
    }

    [PunRPC]
    public void SyncInvulnerability(bool value, PhotonMessageInfo info)
    {
        isInvulnerable = value;
    }


    [PunRPC]
    void SyncCritical(bool value, PhotonMessageInfo info)
    {
        IsCritical = value;
    }

    [PunRPC]
    void SyncAnimation(string anim) {
        //animator.Play(anim);
    }

    [PunRPC]
    void SyncDashRing(bool cancel = false) {
        //dashLight.enabled = value;
        if (cancel) {
            dashRing.transform.localScale = Vector3.zero;
        }
        else {
            if (LoadDash > 0) {
                float calcScale = (1 / (MaxDash / LoadDash)) / 4;
                dashRing.transform.localScale = new Vector3(calcScale, calcScale, calcScale);
            }
            else {
                dashRing.transform.localScale = Vector3.zero;
            }

        }
    }

    IEnumerator ShieldTimeOut(float multiplier = 1.0f) {
        yield return new WaitForSeconds(shieldTimeOut);
        shieldEnabled = false;
        yield return new WaitForSeconds(shieldCooldown * multiplier);
        canActivateShield = true;
        
    }
}
