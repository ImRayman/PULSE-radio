using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class LovenderVisualController : MonoBehaviour
{
    public SortingGroup sorting_group;
    public GameObject flower_head;
    public GameObject flower_head_scale;
    public GameObject flower_cooldown_obj;
    public SpriteRenderer flower_patel;
    public AnimationCurve flowerSoundCurve; // Animation curve for the sound
    public float jumpHeight = 0.5f; // Define how high the flower head jumps
    public float jumpScale = 0.2f; // Define how high the flower head jumps
    public float scale_min, scale_max;
    private bool animating = false;

    public void SetColor(Color color,int num)
    {
        flower_patel.color = color;
        ScaleHead(scale_max - (jumpScale * num));
    }
    public void PlaySpawn(float timer)
    {
        sorting_group.sortingOrder = Mathf.RoundToInt(transform.parent.transform.position.y) * -1;
        StartCoroutine(ScaleFlower(flower_cooldown_obj, timer, 1, 0)); // Scale down over time
    }

    public void SetHeight(int num)
    {
        flower_head.transform.localPosition = new Vector3(0,2 + jumpHeight * num,0);
    }

    public void FlowerSound()
    {
        StartCoroutine(ScaleFlowerWithCurve(flower_cooldown_obj, 0.2f, flowerSoundCurve)); // Scale up quickly using animation curve
    }

    public void FlowerUp()
    {
        StartCoroutine(FlowerUp(flower_head, jumpHeight)); // You can adjust the height value
    }

    public void FlowerDown()
    {
        StartCoroutine(FlowerDown(flower_head, jumpHeight)); // You can adjust the height value
    }

    public void CooldownPatels(float timer)
    {
        if(animating) return;
        
        float currentScale = Mathf.Lerp(1, 0, timer);
        flower_patel.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
    }

    IEnumerator ScaleFlower(GameObject target, float duration, float initialScale, float targetScale)
    {
        animating = true;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            float currentScale = Mathf.Lerp(initialScale, targetScale, timeElapsed / duration);
            target.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        target.transform.localScale = new Vector3(targetScale, targetScale, targetScale); // Ensure the target scale is set
        animating = false;
    }

    IEnumerator ScaleFlowerWithCurve(GameObject target, float duration, AnimationCurve curve)
    {
        animating = true;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            float currentScale = curve.Evaluate(timeElapsed / duration);
            target.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        float finalScale = curve.Evaluate(1); // Ensure the final scale matches the end of the curve
        target.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        animating = false;
    }

    IEnumerator FlowerUp(GameObject target, float height)
    {
        Vector3 startPosition = target.transform.localPosition;
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y + height, startPosition.z);
        
        float timeToMove = 0.2f;
        for (float t = 0; t <= 1; t += Time.deltaTime / timeToMove)
        {
            target.transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        target.transform.localPosition = endPosition; // Ensure it's at the final position
    }

    IEnumerator FlowerDown(GameObject target, float height)
    {
        Vector3 startPosition = target.transform.localPosition;
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y - height, startPosition.z); // Moving down by 'height'

        float timeToMove = 0.2f;
        for (float t = 0; t <= 1; t += Time.deltaTime / timeToMove)
        {
            target.transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        target.transform.localPosition = endPosition; // Ensure it's at the final position
    }

    private void ScaleHead(float endScale)
    {
        float clamped = Mathf.Clamp(endScale, scale_min, scale_max);
        flower_head_scale.transform.localScale = new Vector3(clamped, clamped, 0);
    }

}
