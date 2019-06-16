using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyShellSpawning : MonoBehaviour
{
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private int shellNb;
    [SerializeField] private GameObject shellPrefab;
    [SerializeField] private float timer;

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapSize.x, 10, mapSize.y));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBombardment() {
        StartCoroutine("Bombard");
    }

    IEnumerator Bombard() {
        for (int i = 0; i < shellNb; i++) {
            Instantiate(shellPrefab, new Vector3(Random.Range(-mapSize.x / 2, mapSize.x / 2), 20, Random.Range(-mapSize.y / 2, mapSize.y / 2)), Quaternion.identity);
            yield return new WaitForSeconds(timer);
        }
    }
}
