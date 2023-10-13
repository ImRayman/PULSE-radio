using UnityEngine;
using System.Collections;

public class LovenderVisualController : MonoBehaviour
{
    public GameObject flower_head;
    public GameObject flower_patel;
    public AnimationCurve flowerSoundCurve; // Animation curve for the sound
    private float jumpHeight = 0.5f; // Define how high the flower head jumps
    private bool animating = false;
    
    public void PlaySpawn(float timer)
    {
        StartCoroutine(ScaleFlower(flower_patel, timer, 1, 0)); // Scale down over time
    }

    public void FlowerSound()
    {
        StartCoroutine(ScaleFlowerWithCurve(flower_patel, 0.2f, flowerSoundCurve)); // Scale up quickly using animation curve
    }

    public void FlowerUp()
    {
        StartCoroutine(FlowerUp(flower_head, 0.5f)); // You can adjust the height value
    }

    public void FlowerDown()
    {
        StartCoroutine(FlowerDown(flower_head, 0.5f)); // You can adjust the height value
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

}
