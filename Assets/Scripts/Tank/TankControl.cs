using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankControl : MonoBehaviour
{
    [SerializeField] private int playerID;

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
    [SerializeField] private Image reloadNormal;
    [SerializeField] private Image reloadLarge;

    [Header("ForceField Settings")]
    [SerializeField] private GameObject forceField;
    [SerializeField] private Vector3 forcefieldSize;
    [SerializeField] private float shieldActivationSpeed;
    private bool shieldEnabled;

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

    private Rigidbody rigid;
    private float quickFireReloadTime, bigFireReloadTime;
    public float shieldActivationTime;
    

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (shieldActivationTime < shieldActivationSpeed && shieldEnabled) {
            shieldActivationTime += Time.deltaTime;
        }else if (!shieldEnabled && shieldActivationTime >= 0) {
            shieldActivationTime -= Time.deltaTime;
        }

        if (quickFireReloadTime < quickShootingSpeed) {
            quickFireReloadTime += Time.deltaTime;
        }

        if (bigFireReloadTime < bigShootingSpeed) {
            bigFireReloadTime += Time.deltaTime;
        }

        if (rigid.velocity.magnitude <= maxSpeed) {
            rigid.AddRelativeForce(new Vector3(Input.GetAxis("Vertical"), 0, 0) * speed, moveForceMode);
        }
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0)*rotationSpeed);

        //RELOAD UI
        if (useUI) {
            reloadNormal.fillAmount = quickFireReloadTime / quickShootingSpeed;
            reloadLarge.fillAmount = bigFireReloadTime / bigShootingSpeed;
        }

        //FIRE
        if (Input.GetMouseButton(0) && quickFireReloadTime >= quickShootingSpeed) {
            quickFireReloadTime = 0;
            GameObject obj = Instantiate(smallShell, firePoint.position, transform.rotation);
            obj.GetComponent<TankShell>().SetLauncherParent(this.gameObject);

            rigid.AddRelativeForce(Vector3.left * knockBack);
        }

        //LARGE FIRE
        if (Input.GetKeyDown(KeyCode.X) && bigFireReloadTime >= bigShootingSpeed) {
            bigFireReloadTime = 0;
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

        //TRAIL
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
