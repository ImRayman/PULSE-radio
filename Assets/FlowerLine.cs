using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlowerLine : MonoBehaviour
{
    public GameObject flowerHead; // Assign the flower head in the inspector

    private LineRenderer lineRenderer;
    private Vector3 fixedPointLocal = new Vector3(0, -1.86f, 0);
    private Vector3 fixedPointWorld;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        // Initialize the line renderer with 2 points.
        lineRenderer.positionCount = 2;
        
        // Convert the local point to world space.
        fixedPointWorld = transform.TransformPoint(fixedPointLocal);
        lineRenderer.SetPosition(1, fixedPointWorld); // second point at the fixed location

        // Optionally, style your line here (width, color, material, etc.)
    }

    void Update()
    {
        lineRenderer.SetPosition(0, flowerHead.transform.position); // first point at flower head
    }
}