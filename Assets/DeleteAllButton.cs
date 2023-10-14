using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteAllButton : MonoBehaviour
{
    public Image hold_timer_visual;

    public void DisplayHolding(float t)
    {
        hold_timer_visual.fillAmount = t;
    }
}
