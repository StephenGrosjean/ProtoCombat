using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Fragment Manager Script
/// </summary>
public class FragmentManager : MonoBehaviour
{
    [SerializeField] private int fragmentMaxCount; //Max fragment count

    public List<GameObject> fragments; //All fragments
   
    void Update()
    {
        //Destroy fragments if above the max count allowed
        if(fragments.Count > fragmentMaxCount) {
            Destroy(fragments[0]); //Destroy fragment at index 0
            fragments.RemoveAt(0); //Remove fragment at index 0
        }
    }

    //Add fragment to list
    public void AddFragment(GameObject frag) {
        //If fragment count is bellow max allowed
        if(fragments.Count > fragmentMaxCount) {
            int position = Random.Range(1, GetFragmentCount()); //generate random position from  1 to end of list
            GameObject fragmentToDestroy = fragments[position].gameObject; //Find fragment at this random position
            fragments.RemoveAt(position); //Remove fragment at this position
            Destroy(fragmentToDestroy); //Destroy fragment
        }
        fragments.Add(frag); //Add fragment to list

    }

    //Get the fragment count
    public int GetFragmentCount() {
        return fragments.Count;
    }
    
    //Remove fragment from list
    public void RemoveFragment(GameObject frag) {
        fragments.Remove(frag);
    }
}
