using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentManager : MonoBehaviour
{
    [SerializeField] private int fragmentMaxCount;

    public List<GameObject> fragments;
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fragments.Count > fragmentMaxCount) {
            Destroy(fragments[0]);
            fragments.RemoveAt(0);
        }
    }

    public void AddFragment(GameObject frag) {
        fragments.Add(frag);
    }
}
