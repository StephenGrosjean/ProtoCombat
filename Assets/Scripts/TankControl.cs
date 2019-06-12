using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankControl : MonoBehaviour
{
    [SerializeField] private GameObject shell;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float shootingSpeed;
    [SerializeField] private float knockBack;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private ForceMode forceMode;
    private Rigidbody rigid;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(time < shootingSpeed) {
            time += Time.deltaTime;
        }

        if (rigid.velocity.magnitude <= maxSpeed) {
            rigid.AddRelativeForce(new Vector3(Input.GetAxis("Vertical"), 0, 0) * speed, forceMode);
        }
        transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal"), 0)*rotationSpeed);

        if (Input.GetMouseButton(0) && time >= shootingSpeed) {
            time = 0;
            Instantiate(shell, firePoint.position, transform.rotation);
            rigid.AddRelativeForce(Vector3.left * knockBack);
        }

    }


}
