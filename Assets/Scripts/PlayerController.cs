using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [FormerlySerializedAs("ambiant_controller")] public AmbianceController ambianceController;
    public CursorDisplay cursor_display;
    public DeleteAllButton delete_all_button;
    public FlowerSpawner flowerSpawner;
    public float holdTime = 2f; // Time to hold before a flower is deleted

    private LovenderController selectedFlower = null;
    private Vector3 startTouchPosition;
    private bool enable_delete = false;
    private bool deleteAllButton = false;
    private float holdCounter = 0f; // Tracks the duration of the hold
    private float delete_all_holdCounter = 0f; // Tracks the duration of the hold
    private bool isDraggingFlower = false;
    private float dragDistance = 0;
    private Vector3 lastPosition = Vector3.zero;

    
    private void Start()
    {
        cursor_display.DisplayHolding(0);
    }
    
    private void Update()
    {
        SpawnFlower();
        UIInteractions();
        HandleFlowerInteraction();
    }

    private void SpawnFlower()
    {
        // Handle touch input
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                {
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    worldPosition.z = 0;

                    RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId) && hit.collider == null)
                    {
                        flowerSpawner.SpawnFlower(worldPosition);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))// Handle mouse input
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (!EventSystem.current.IsPointerOverGameObject() && hit.collider == null) 
            {
                flowerSpawner.SpawnFlower(worldPosition);
            }
        }
    }


    private void HandleFlowerInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
        else if (Input.GetMouseButton(0))
        {
            ContinueDrag();
        }
    }

    private void TryStartDrag()
    {
        startTouchPosition = Input.mousePosition;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit.collider != null)
        {
            selectedFlower = hit.collider.gameObject.GetComponent<LovenderController>();
            if (selectedFlower != null)
            {
                isDraggingFlower = true;
                enable_delete = true;
            }
        }
    }

    private void EndDrag()
    {
        selectedFlower = null;
        isDraggingFlower = false;
        enable_delete = false;

        // Reset variables
        holdCounter = 0f;
        dragDistance = 0;
        cursor_display.DisplayHolding(0);
    }

    private void ContinueDrag()
    {
        dragDistance = Vector3.Distance(startTouchPosition, Input.mousePosition);

        if (enable_delete)
        {
            if (dragDistance > 0.2f) // Assuming this is some threshold for drag detection
            {
                enable_delete = false;
            }
            else
            {
                HandleDelete();
            }
        }
        else
        {
            cursor_display.DisplayHolding(0); // Assuming this updates some UI element
        }

        // If we're dragging, don't trigger other global interactions
        if (isDraggingFlower && selectedFlower != null)
        {
            selectedFlower.HandleDrag(Input.mousePosition, ref lastPosition);
        }
        else
        {
            lastPosition = Input.mousePosition;
        }
    }

    private void HandleDelete()
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

    private void UIInteractions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray to the mouse position
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if (hit.collider != null && hit.collider.gameObject.tag == "DeleteButton")
            {
                deleteAllButton = true;
            }
            else if (hit.collider != null && hit.collider.gameObject.tag == "AmbianceButton")
            {
                ambianceController.NextAmbiance();
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
}
