using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorDisplay : MonoBehaviour
{
    public Image hold_timer_visual;

    public void DisplayHolding(float t)
    {
        hold_timer_visual.fillAmount = t;
    }

    public void GotoPosition(Vector3 pos)
    {
        Vector3 new_pos = new Vector3(pos.x, pos.y, 0);
        transform.position = new_pos;
    }
}
