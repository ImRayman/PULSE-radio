using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    public GameObject flowerPrefab; // Ensure you assign this in the inspector with your Lovender prefab

    // This method creates a new flower instance at the specified position
    public void SpawnFlower(Vector2 position)
    {
        // The position should be converted from screen space to world space, if the position passed isn't already in world space
        Vector2 worldPosition = position;

        // Instantiate a new flower at the specified position
        Instantiate(flowerPrefab, worldPosition, Quaternion.identity);
    }

}