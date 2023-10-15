using UnityEngine;
using UnityEngine.Events;

public class Globals
{ 
    public static float flower_timer = 4;
    public static UnityEvent<float> change_flower_timer = new();
    
    public static float flower_pitch = 1;
    public static UnityEvent<float> change_flower_pitch = new();
}