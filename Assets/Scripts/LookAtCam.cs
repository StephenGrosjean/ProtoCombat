using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    [SerializeField] private Transform firstTarget, secondTarget;
    [SerializeField] private float xOffset, fovOffset;
    public Vector3 middleVector;
    public float point;
    public float distance;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(middleVector, 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(firstTarget.position, secondTarget.position);

        middleVector = distance/2 * Vector3.Normalize(secondTarget.position - firstTarget.position) + firstTarget.position;
        transform.position = new Vector3(middleVector.x+xOffset, transform.position.y, middleVector.z);
        float fovCalc = 0.7f * (distance + fovOffset) + 21.8f;
        if (fovCalc > 65) {
            Camera.main.fieldOfView = fovCalc;
        }
    }
}
