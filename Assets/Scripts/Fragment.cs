using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment : MonoBehaviour
{

    private Material material;
    private Color baseColor;
    public bool destroy;
    public float lerpTime;

    // Start is called before the first frame update
    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;
        baseColor = material.color;
        Invoke("DisableRigid", 5);
    }

    // Update is called once per frame
    void Update()
    {
       /* if (destroy) {
            lerpTime += 0.1f;
            material.color = Color.Lerp(baseColor, new Color(baseColor.r, baseColor.g, baseColor.b, 0), lerpTime);
        }

        if(lerpTime >= 1) {
            Destroy(gameObject);
        }*/
    }


    private void OnDestroy() {
        GameObject.Find("FragmentManager").GetComponent<FragmentManager>().RemoveFragment(gameObject);
    }

    void DisableRigid() {
        Destroy(GetComponent<Rigidbody>());
    }
}
