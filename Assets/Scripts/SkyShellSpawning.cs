using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script sky shell bombing
/// </summary>

public class SkyShellSpawning : MonoBehaviour
{
    [SerializeField] private Vector2 mapSize; //Current Map Size
    [SerializeField] private int shellNb; //How many shells to spawn
    [SerializeField] private GameObject shellPrefab; //What to spawn
    [SerializeField] private float timer; //Time interval between spawns

    //Draw Map size
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapSize.x, 10, mapSize.y));
    }

    //Start Bombardment
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
