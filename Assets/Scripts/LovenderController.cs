using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class LovenderController : MonoBehaviour
{
    public List<Color> sound_colors;
    public List<AudioClip> sounds;
    public AudioSource audioSource;
    public float timer = 5f;
    private int currentSoundIndex = 0;

    public LovenderVisualController lovenderVisualController;
    public BoxCollider2D boxCollider;
    
    private bool isDragging = false;

    public float thresholdDistance = 20f; // The required change in Y position to change the sound index

    private float initialChildY;
    private float initialColliderSizeY;
    private float initialColliderOffsetY;

    private void Awake()
    {
        //Globals.change_flower_timer.AddListener(SetTimer);
        Globals.change_flower_pitch.AddListener(SetPitch);
        
        initialChildY = lovenderVisualController.transform.position.y;
        initialColliderSizeY = boxCollider.size.y;
        initialColliderOffsetY = boxCollider.offset.y;
    }

    private void OnEnable()
    {
        SetTimer(Globals.flower_timer);
        SetPitch(Globals.flower_pitch);
        if (sounds.Count > 0)
        {
            currentSoundIndex = Random.Range(0, sounds.Count-1);
            audioSource.clip = sounds[currentSoundIndex];
            SetFlowerHeightAndColorToAudioIx();
        }
    }

    private void OnDestroy()
    {
        //Globals.change_flower_timer.RemoveListener(SetTimer);
        Globals.change_flower_pitch.RemoveListener(SetPitch);

    }

    private void SetTimer(float timer)
    {
        this.timer = timer;
    }

    void Start()
    {
        UpdateCollider();
        audioSource.Play();
        StartCoroutine(StartFlower());
        StartCoroutine(DelayedCollider());
        
        // Visual indication that the flower has spawned
        lovenderVisualController.PlaySpawn(timer); // Pass the same timer for the visual effect
    }

    private IEnumerator DelayedCollider()
    {
        yield return new WaitForSeconds(0.18f);
        UpdateCollider();
    }

    public void HandleDrag(Vector3 currentPos,ref Vector3 lastPosition)
    {
        // If the player has stopped dragging, or this is the first frame of the drag, update lastPosition and exit
        if (currentPos == lastPosition)
        {
            lastPosition = currentPos;
            return;
        }

        // Check the distance moved in the Y direction
        float deltaY = currentPos.y - lastPosition.y;

        // Determine the direction based on the change in Y position
        float dragDirection = deltaY > 0 ? 1f : -1f;

        // Check if the movement exceeds the threshold distance
        if (Mathf.Abs(deltaY) < thresholdDistance)
        {
            return; // Not enough movement to change the sound
        }

        // Calculate the new sound index
        int newSoundIndex = currentSoundIndex + (int)dragDirection;
        newSoundIndex = Mathf.Clamp(newSoundIndex, 0, sounds.Count - 1);
        
        // If we're at an edge, don't allow the index to continue past it
        if (currentSoundIndex != newSoundIndex)
        {
            // Update the sound index
            currentSoundIndex = newSoundIndex;

            // Update the sound if we have a new sound index
            UpdateSound();

            // Since the player moved more than the threshold and it resulted in a sound change, we update lastPosition
            lastPosition = currentPos;
        }
    }

    private void UpdateSound()
    {
        if (sounds.Count > 0)
        {
            audioSource.clip = sounds[currentSoundIndex];
            audioSource.Play();

            lovenderVisualController.SetColor(sound_colors[currentSoundIndex],currentSoundIndex);
            lovenderVisualController.PositionHead(currentSoundIndex);
            UpdateCollider();
        }
    }
    
    private IEnumerator StartFlower()
    {
        while (true)
        {
            PlaySound();
            float time = 0f; // Reset the time at the start of each cooldown

            while (time < timer)
            {
                // Update the visual cooldown effect
                lovenderVisualController.CooldownPatels(time / timer);

                // Wait for the next frame
                yield return null; // This makes Unity wait until the next frame before continuing the loop

                // Increment the time by the time passed since the last frame
                time += Time.deltaTime; // Time.deltaTime is the time in seconds it took to complete the last frame
            }
        }
    }

    void PlaySound()
    {
        if (sounds.Count > 0)
        {
            audioSource.clip = sounds[currentSoundIndex];
            audioSource.Play();

            // Visual feedback for sound
            lovenderVisualController.FlowerSound(); // Indicate that a sound is played
        }
    }
    public void HandleLongPress()
    {
        DeleteFlower();
    }

    public void DeleteFlower()
    {
        lovenderVisualController.PlayDeleteAnim();
        Destroy(gameObject,0.5f);
    }

    private void SetFlowerHeightAndColorToAudioIx()
    {
        lovenderVisualController.SetColor(sound_colors[currentSoundIndex],currentSoundIndex);
        lovenderVisualController.SetHeight(currentSoundIndex);
    }
    
    private void UpdateCollider()
    {
        float deltaY = lovenderVisualController.transform.position.y - initialChildY;

        // Adjust the collider's size and offset based on the change in Y position
        boxCollider.size = new Vector2(boxCollider.size.x, initialColliderSizeY + deltaY);
        boxCollider.offset = new Vector2(boxCollider.offset.x, initialColliderOffsetY + deltaY / 2);
    }

    private void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }
}
