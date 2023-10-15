using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelaySlider : MonoBehaviour
{
    public Slider slider;
    private void Start()
    {
        // Initialize the slider value.
        slider.value = 1f; // You can set it to any initial value you want.

        // Add an event listener to the slider's value change event.
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        // Update the value text to show the initial value.
        ChangePitch(slider.value);
    }

    private void OnSliderValueChanged(float newValue)
    {
        ChangePitch(newValue);
    }

    private void UpdateTimer(float value)
    {
        if(value == 0 ) return;
        Globals.flower_timer = value;
        Globals.change_flower_timer.Invoke(value);
    }

    private void ChangePitch(float value)
    {
        Globals.flower_pitch = value;
        Globals.change_flower_pitch.Invoke(value);
    }
}
