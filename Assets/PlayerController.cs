using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public FlowerSpawner flowerSpawner;
    public float holdTime = 1.5f; // Time to hold before a flower is deleted

    private LovenderController selectedFlower = null;
    private Vector2 startTouchPosition;
    private bool swipeDetected = false;
    private float holdCounter = 0f; // Tracks the duration of the hold

    void Update()
    {
        // Detect if the left mouse button is pressed down
        if (Input.GetMouseButtonDown(0))
        {
            // Record the initial touch position.
            startTouchPosition = Input.mousePosition;

            // Cast a ray to the mouse position
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<LovenderController>())
            {
                // If a flower (Lovender) is clicked, set it as the selected flower
                selectedFlower = hit.collider.gameObject.GetComponent<LovenderController>();
            }
            else if (!EventSystem.current.IsPointerOverGameObject()) // Check if not clicking on UI
            {
                // If we're not clicking on a flower, spawn one at the click/tap position
                flowerSpawner.SpawnFlower(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        // Detect if the left mouse button is held down
        if (Input.GetMouseButton(0) && selectedFlower != null)
        {
            holdCounter += Time.deltaTime; // Increment the hold counter
            if (holdCounter >= holdTime)
            {
                // If held for long enough, delete the flower and reset
                selectedFlower.HandleLongPress();
                selectedFlower = null;
                holdCounter = 0f;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 endTouchPosition = Input.mousePosition;
            Vector2 swipeDelta = endTouchPosition - startTouchPosition;

            // Check if the swipe gesture is vertical and significant enough to be considered a swipe
            if (Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x) && swipeDelta.magnitude > 100f)
            {
                swipeDetected = true;
                if (selectedFlower != null && holdCounter < holdTime) // Ensure we're not swiping after a long press
                {
                    // Determine if the swipe is upwards or downwards and tell the selected flower to change its sound accordingly
                    if (swipeDelta.y > 0)
                    {
                        selectedFlower.NextSound();
                    }
                    else
                    {
                        selectedFlower.PreviousSound();
                    }
                }
            }
            else if (selectedFlower != null && !swipeDetected && holdCounter < holdTime) // Simple tap and not a long press
            {
                // Tell the flower to change to the next sound
                selectedFlower.NextSound();
            }

            // Reset variables
            selectedFlower = null;
            swipeDetected = false;
            holdCounter = 0f; // Reset the hold counter
        }
    }
}
