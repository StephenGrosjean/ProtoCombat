using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShell : MonoBehaviour
{

    public enum ShellType {
        Small,
        Large
    }
    
    [SerializeField] private float speed = 10;
    [SerializeField] private GameObject explosionParticle;
    [SerializeField] private ShellType typeShell;
    [SerializeField] private bool skyShell;
    public ShellType TypeShell { get { return typeShell; } set { typeShell = value; } }

    private int health = 1;

    private Rigidbody rigid;
    private GameObject launcherParent;


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        if (!skyShell) {
            rigid.AddRelativeForce(-Vector3.left * speed);

        }
        else {
            rigid.AddRelativeForce(Vector3.down * speed);

        }
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject != launcherParent) {
            if(collision.gameObject.tag == "Destroyable") {
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
                Destroy(gameObject, 0.1f);
            }
            if(collision.gameObject.tag == "Tank") {
                collision.gameObject.GetComponent<TankHealth>().TakeDamage(5);
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
                Destroy(gameObject, 0.1f);
            }

            if (health > 0) {
                rigid.velocity *= 2;
                health--;

            }
            else {
                Instantiate(explosionParticle, transform.position, Quaternion.identity);
                Destroy(gameObject, 0.1f);
            }

        }
        else {
            Instantiate(explosionParticle, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.1f);
        }

    }

    Vector3 CalculateBounce(Vector3 contactVelocity, Vector3 contactNormal) {
       return Vector3.Reflect(contactVelocity, contactNormal);
    }

    public void SetLauncherParent(GameObject launcher) {
        launcherParent = launcher;
    }
}
