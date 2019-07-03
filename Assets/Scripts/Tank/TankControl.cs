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
    //[SerializeField] private int playerID; (NOT USED)

    [Header("Part settings")]
    [SerializeField] private Transform turret;
    //[SerializeField] private Transform body;

    [Header("Fire Settings")]
    [SerializeField] private GameObject smallShell;
    [SerializeField] private GameObject largeShell;
    [SerializeField] private Transform shootingPoint;
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
        reloadNormal = GameObject.Find("ReloadMachineGun").GetComponent<Image>();
        reloadLarge = GameObject.Find("ReloadLargeShot").GetComponent<Image>();
    }

    void FixedUpdate()
    {
        //Increase shield activation Time
        if (shieldEnabled && shieldActivationTime < shieldActivationSpeed) {
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
            rigid.AddRelativeForce(new Vector3(GameInput.GetAxis(GameInput.AxisType.L_VERTICAL), 0, 0) * speed, moveForceMode);
        }

        //Rotate the tank 
        transform.Rotate(new Vector3(0, GameInput.GetAxis(GameInput.AxisType.L_HORIZONTAL), 0) * rotationSpeed);

        //turret.Rotate(new Vector3(0, GameInput.GetAxis(GameInput.AxisType.R_HORIZONTAL), 0) * rotationSpeed);


        //RELOAD UI
        if (useUI) {
            reloadNormal.fillAmount = quickFireReloadTime / quickShootingSpeed;
            reloadLarge.fillAmount = bigFireReloadTime / bigShootingSpeed;
        }

        //MOVE CANNON
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = GameInput.GetDirection(GameInput.DirectionType.R_INPUT, new Vector2(screenPos.x, screenPos.y));
        float angle = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90.0f;
        turret.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));

        //FIRE
        if (GameInput.GetInputDown(GameInput.InputType.SHOOT) && quickFireReloadTime >= quickShootingSpeed) {
            quickFireReloadTime = 0;
            Camera.main.GetComponent<CameraShake>().ShakeCam(.1f, 0.1f);
            //GameObject obj = Instantiate(smallShell, shootingPoint.position, transform.rotation);
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "smallShell"), shootingPoint.position, Quaternion.Euler(new Vector3(0, angle + 180.0f, 0)));
            obj.GetComponent<TankShell>().SetLauncherParent(this.gameObject);

            rigid.AddRelativeForce(-Vector3.left * knockBack);
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
        if (GameInput.GetInputDown(GameInput.InputType.DEFENSE)) {
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
       /* if (GameInput.GetInputDown(GameInput.InputType.DASH)) {
            GetComponent<SkyShellSpawning>().StartBombardment();
        }*/
    }


}
