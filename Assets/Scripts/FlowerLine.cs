using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlowerLine : MonoBehaviour
{
    public GameObject flowerHead; // Assign the flower head in the inspector
    
    private LineRenderer lineRenderer;
    private Vector3 fixedPointLocal = Vector3.zero;
    private Vector3 fixedPointWorld;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        // Initialize the line renderer with 2 points.
        lineRenderer.positionCount = 2;
        
        // Convert the local point to world space.
        fixedPointWorld = transform.TransformPoint(fixedPointLocal);

        lineRenderer.SetPosition(1, fixedPointWorld); // second point at the fixed location
    }

    private void OnEnable()
    {
        lineRenderer.SetPosition(0, flowerHead.transform.position); // first point at flower head
    }

    void Update()
    {
        lineRenderer.SetPosition(0, flowerHead.transform.position); // first point at flower head
    }
}