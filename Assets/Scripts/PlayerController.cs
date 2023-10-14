using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public CursorDisplay cursor_display;
    public DeleteAllButton delete_all_button;
    public FlowerSpawner flowerSpawner;
    public float holdTime = 2f; // Time to hold before a flower is deleted

    private LovenderController selectedFlower = null;
    private Vector2 startTouchPosition;
    private bool swipeDetected = false;
    private bool deleteAllButton = false;
    private float holdCounter = 0f; // Tracks the duration of the hold
    private float delete_all_holdCounter = 0f; // Tracks the duration of the hold
    private bool isDraggingFlower = false;
    private float dragDistance = 0;

    private void Start()
    {
        cursor_display.DisplayHolding(0);
    }

    void Update()
    {
        UIInteractions();
        FlowerDragging();
        //FlowerInteractions();
    }

    private void FlowerDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<LovenderController>())
            {
                selectedFlower = hit.collider.gameObject.GetComponent<LovenderController>();
                isDraggingFlower = true;
            }
        }

        // Check for the end of a drag
        if (Input.GetMouseButtonUp(0))
        {
            selectedFlower = null;
            isDraggingFlower = false;
            
            // Reset variables
            holdCounter = 0f;
            dragDistance = 0;
            cursor_display.DisplayHolding(0);
        }
        
        // Detect if the left mouse button is held down
        if (Input.GetMouseButton(0) && dragDistance < 0.1f)
        {
            holdCounter += Time.deltaTime;

            if (selectedFlower != null && holdCounter / holdTime > 0.2f)
            {
                cursor_display.DisplayHolding(holdCounter / holdTime);
                cursor_display.GotoPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            
            if (holdCounter >= holdTime)
            {
                if (selectedFlower != null)
                {
                    selectedFlower.HandleLongPress();
                    selectedFlower = null;
                }

                holdCounter = 0f;
                isDraggingFlower = false;
                cursor_display.DisplayHolding(0);
            }
        }
        else
        {
            cursor_display.DisplayHolding(0);
        }

        // If we're dragging, don't trigger other global interactions
        if (isDraggingFlower)
        {
            dragDistance = Vector3.Distance(startTouchPosition, Input.mousePosition);
            selectedFlower.HandleDrag(dragDistance);
        }
    }

    private void UIInteractions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray to the mouse position
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<LovenderController>())
            {
                
            }
            else if (hit.collider != null && hit.collider.gameObject.tag == "DeleteButton")
            {
                deleteAllButton = true;
            }
            else if (!EventSystem.current.IsPointerOverGameObject()) // Check if not clicking on UI
            {
                // If we're not clicking on a flower, spawn one at the click/tap position
                flowerSpawner.SpawnFlower(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        // Detect if the left mouse button is held down
        if (Input.GetMouseButton(0))
        {
            delete_all_holdCounter += Time.deltaTime;
            if (deleteAllButton)
            {
                delete_all_button.DisplayHolding(delete_all_holdCounter / holdTime);
            }

            if (delete_all_holdCounter >= holdTime)
            {
                // If held for long enough, delete the flower and reset
                if (deleteAllButton)
                {
                    flowerSpawner.DeleteAllFlowers();
                    deleteAllButton = false;
                }
                
                delete_all_holdCounter = 0f;
                delete_all_button.DisplayHolding(0);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Reset variables
            deleteAllButton = false;
            delete_all_holdCounter = 0f; // Reset the hold counter
            delete_all_button.DisplayHolding(0);
        }
    }

    private void FlowerInteractions()
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
            else if (hit.collider != null && hit.collider.gameObject.tag == "DeleteButton")
            {
                deleteAllButton = true;
            }
            else if (!EventSystem.current.IsPointerOverGameObject()) // Check if not clicking on UI
            {
                // If we're not clicking on a flower, spawn one at the click/tap position
                flowerSpawner.SpawnFlower(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        // Detect if the left mouse button is held down
        if (Input.GetMouseButton(0))
        {
            holdCounter += Time.deltaTime;
            if (deleteAllButton)
            {
                delete_all_button.DisplayHolding(holdCounter / holdTime);
            }

            if (selectedFlower != null)
            {
                cursor_display.DisplayHolding(holdCounter / holdTime);
                cursor_display.GotoPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            
            if (holdCounter >= holdTime)
            {
                // If held for long enough, delete the flower and reset
                if (selectedFlower != null)
                {
                    selectedFlower.HandleLongPress();
                    selectedFlower = null;
                }
                else if (deleteAllButton)
                {
                    flowerSpawner.DeleteAllFlowers();
                    deleteAllButton = false;
                }
                
                holdCounter = 0f;
                delete_all_button.DisplayHolding(0);
                cursor_display.DisplayHolding(0);
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

            // Reset variables
            selectedFlower = null;
            swipeDetected = false;
            deleteAllButton = false;
            holdCounter = 0f; // Reset the hold counter
            delete_all_button.DisplayHolding(0);    
            cursor_display.DisplayHolding(0);
        }
    }
}
