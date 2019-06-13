using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{

    [SerializeField] Vector3 direction;
    [SerializeField] float force;

    bool fire = false;

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + direction * 2);
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !fire) {
            fire = true;
            transform.GetComponent<Rigidbody>().velocity = direction * force;
        }
    }

    


}
