using UnityEngine;

public class DeletaAllPRefabs : MonoBehaviour
{
    public GameObject WallPrefab; // Reference to the prefab to delete
    private GameObject[] spawnedPrefabs; // Store references to spawned prefabs

    // Attach this method to the button's OnClick event in the Inspector.
    public void DeleteAllPrefabs()
    {
        // Find all GameObjects in the scene that are instances of the WallPrefab.
        spawnedPrefabs = GameObject.FindGameObjectsWithTag("Wall");

        // Loop through and destroy all instances of the WallPrefab.
        foreach (GameObject prefab in spawnedPrefabs)
        {
            Destroy(prefab);
        }

        // Optionally, clear the array of references.
        spawnedPrefabs = new GameObject[0];
    }
}

