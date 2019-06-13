using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControl : MonoBehaviour
{
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

    [Header("Trail Settings")]
    [SerializeField] private GameObject trailLight;
    [SerializeField] private float lightSpacing;
    [SerializeField] private int maxTrailLights = 100;

    [Header("Containers")]
    [SerializeField] private Transform trailContainer;

    private List<GameObject> trailLights = new List<GameObject>();
    private float spawnLightTimer;

    private Rigidbody rigid;
    private float quickFireReloadTime, bigFireReloadTime;
    

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(quickFireReloadTime < quickShootingSpeed) {
            quickFireReloadTime += Time.deltaTime;
        }

        if (bigFireReloadTime < bigShootingSpeed) {
            bigFireReloadTime += Time.deltaTime;
        }

        if (rigid.velocity.magnitude <= maxSpeed) {
            rigid.AddRelativeForce(new Vector3(Input.GetAxis("Vertical"), 0, 0) * speed, moveForceMode);
        }
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0)*rotationSpeed);


        //FIRE
        if (Input.GetMouseButton(0) && quickFireReloadTime >= quickShootingSpeed) {
            quickFireReloadTime = 0;
            Instantiate(smallShell, firePoint.position, transform.rotation);
            rigid.AddRelativeForce(Vector3.left * knockBack);
        }

        //LARGE FIRE
        if (Input.GetKeyDown(KeyCode.X) && bigFireReloadTime >= bigShootingSpeed) {
            bigFireReloadTime = 0;
            GameObject obj = Instantiate(largeShell, firePoint.position, transform.rotation);
            obj.GetComponent<TankShell>().TypeShell = TankShell.ShellType.Large;
            rigid.AddRelativeForce(Vector3.left * knockBack * 5);
        }

        if(rigid.velocity.magnitude != 0) {
            spawnLightTimer += Time.deltaTime;
            if(spawnLightTimer > lightSpacing) {
                spawnLightTimer = 0;
                GameObject lightTrail = Instantiate(trailLight, new Vector3(transform.position.x, -1f, transform.position.z), trailLight.transform.rotation);
                trailLights.Add(lightTrail);
                lightTrail.transform.SetParent(trailContainer);
            }
        }

        if(trailLights.Count > maxTrailLights) {
            trailLights.RemoveAt(0);
            Destroy(trailLights[0]);
        }

    }


}
