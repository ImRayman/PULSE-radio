using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class LovenderController : MonoBehaviour
{
    public List<AudioClip> sounds;
    public AudioSource audioSource;
    public float timer = 5f; // Timer for sound playing
    private int currentSoundIndex = 0;

    public LovenderVisualController lovenderVisualController; // Reference to the visual controller

    void Start()
    {
        if (sounds.Count > 0)
        {
            audioSource.clip = sounds[Random.Range(0,sounds.Count)];
            audioSource.Play();
        }

        StartCoroutine(StartFlower());
        
        // Visual indication that the flower has spawned
        lovenderVisualController.PlaySpawn(timer); // Pass the same timer for the visual effect
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

            // After the cooldown, you might want to wait for a certain time before the next sound is played again
            // If so, add another yield return new WaitForSeconds(waitDuration); here
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
        }
        // else do nothing if we're at the first sound
    }

    
    public void HandleLongPress()
    {
        // Perform any necessary cleanup, like stopping audio, animations, etc.
        
        // Then destroy the flower
        Destroy(gameObject);
    }
}
