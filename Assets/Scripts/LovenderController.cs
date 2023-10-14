using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class LovenderController : MonoBehaviour
{
    public List<Color> sound_colors;
    public List<AudioClip> sounds;
    public AudioSource audioSource;
    public float timer = 5f;
    private int currentSoundIndex = 0;

    public LovenderVisualController lovenderVisualController;
    public CapsuleCollider2D collider;
    
    private bool isDragging = false;
    private float maxDragDistance = 100f; 

    private void Awake()
    {
        Globals.change_flower_timer.AddListener(SetTimer);
    }

    private void OnEnable()
    {
        SetTimer(Globals.flower_timer);
    }

    private void OnDestroy()
    {
        Globals.change_flower_timer.RemoveListener(SetTimer);
    }

    private void SetTimer(float timer)
    {
        this.timer = timer;
    }

    void Start()
    {
        if (sounds.Count > 0)
        {
            currentSoundIndex = Random.Range(0, sounds.Count);
            audioSource.clip = sounds[currentSoundIndex];
            SetFlowerHeightAndColorToAudioIx();
            audioSource.Play();
        }

        StartCoroutine(StartFlower());
        
        // Visual indication that the flower has spawned
        lovenderVisualController.PlaySpawn(timer); // Pass the same timer for the visual effect
    }
    
    public void HandleDrag(float dragDistance)
    {
        // Calculate the drag distance

        // Calculate the drag percentage
        float dragPercentage = Mathf.Clamp01(dragDistance / maxDragDistance);

        // Determine the sound index based on drag percentage
        int soundIndex = (int)(dragPercentage * sounds.Count);
        soundIndex = Mathf.Clamp(soundIndex, 0, sounds.Count - 1);

        // If the sound index has changed, update the sound
        if (soundIndex != currentSoundIndex)
        {
            currentSoundIndex = soundIndex;
            UpdateSound(dragPercentage);
        }
    }
    
    private void UpdateSound(float dragPercentage)
    {
        if (sounds.Count > 0)
        {
            audioSource.clip = sounds[currentSoundIndex];
            audioSource.Play();

            lovenderVisualController.SetColor(sound_colors[currentSoundIndex],currentSoundIndex);
            lovenderVisualController.PositionHead(dragPercentage);
            UpdateColliderHeight();
        }
    }

    private void UpdateColliderHeight()
    {
        // Calculate the height based on the difference between the current child's local position and the initial position.
        float height = Mathf.Abs(lovenderVisualController.transform.localPosition.y) * 2 * 0.3f + 0.25f;

        // Update the collider size while keeping the bottom in place.
        float yOffset = height / 2 - 0.5f;
        collider.size = new Vector2(collider.size.x, height);
        collider.offset = new Vector2(collider.offset.x, yOffset);
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

    public void NextSound()
    {
        if (sounds.Count > 0 && currentSoundIndex < sounds.Count - 1) // Check if not already at the last sound
        {
            currentSoundIndex++;
            audioSource.clip = sounds[currentSoundIndex];
            audioSource.Play();

            // Visual indication that the sound has changed - next
            lovenderVisualController.FlowerUp(); // Flower jumps up
            lovenderVisualController.SetColor(sound_colors[currentSoundIndex],currentSoundIndex);
            UpdateColliderHeight();
        }
        // else do nothing if we're at the last sound
    }

    public void PreviousSound()
    {
        if (sounds.Count > 0 && currentSoundIndex > 0) // Check if not already at the first sound
        {
            currentSoundIndex--;
            audioSource.clip = sounds[currentSoundIndex];
            audioSource.Play();

            // Visual indication that the sound has changed - previous
            lovenderVisualController.FlowerDown(); // Flower jumps down
            lovenderVisualController.SetColor(sound_colors[currentSoundIndex],currentSoundIndex);
            UpdateColliderHeight();
        }
        // else do nothing if we're at the first sound
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
        UpdateColliderHeight();
    }
}
