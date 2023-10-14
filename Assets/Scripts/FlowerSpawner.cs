using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    public GameObject flowerPrefab; // Ensure you assign this in the inspector with your Lovender prefab
    
    public void DeleteAllFlowers()
    {
        StartCoroutine(DeleteAllAnim());
    }

    private IEnumerator DeleteAllAnim()
    {
        foreach (LovenderController flower in transform.GetComponentsInChildren<LovenderController>().Reverse())
        {
            flower.DeleteFlower();
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void SpawnFlower(Vector2 position)
    {
        // The position should be converted from screen space to world space, if the position passed isn't already in world space
        Vector2 worldPosition = position;

        // Instantiate a new flower at the specified position
        Instantiate(flowerPrefab, worldPosition, Quaternion.identity,transform);
    }

}