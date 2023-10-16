using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceController : MonoBehaviour
{
    public List<AudioClip> ambiant_list;
    public AudioSource audio_source;
    private int count = 0;

    public void NextAmbiance()
    {
        count++;
        if (count >= ambiant_list.Count)
        {
            count = -1;
            audio_source.Stop();
        }
        else
        {
            audio_source.clip = ambiant_list[count];
            audio_source.Play();
        }
    }
}
