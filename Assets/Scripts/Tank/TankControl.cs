using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControl : MonoBehaviour
{
    [SerializeField] private GameObject smallShell, largeShell;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float quickShootingSpeed, bigShootingSpeed;
    [SerializeField] private float knockBack;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private ForceMode forceMode;
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
            rigid.AddRelativeForce(new Vector3(Input.GetAxis("Vertical"), 0, 0) * speed, forceMode);
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

    }


}
