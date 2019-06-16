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
        if(fragments.Count > fragmentMaxCount) {
            int position = Random.Range(1, GetFragmentCount());
            GameObject fragmentToDestroy = fragments[position].gameObject;
            fragments.RemoveAt(position);
            Destroy(fragmentToDestroy);
        }
        fragments.Add(frag);

    }

    public int GetFragmentCount() {
        return fragments.Count;
    }

    public void RemoveFragment(GameObject frag) {
        fragments.Remove(frag);
    }
}
