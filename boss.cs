using UnityEngine;
using System.Collections;  

public class boss : MonoBehaviour
{
    public Transform bossfight; 
    public float targetX = 10f;
    public float speed = 2f; 
    private Vector3 originalPosition; 
    private Coroutine currentCoroutine; 

    void Start()
    {
        originalPosition = bossfight.position;
    }

    public void BossFightStart()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        
        currentCoroutine = StartCoroutine(MoveBossfight(targetX));
    }

    public void ReturnToOriginalPosition()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(MoveBossfight(originalPosition.x));
    }

    private IEnumerator MoveBossfight(float targetX)
    {
        Vector3 startPosition = bossfight.position;
        Vector3 targetPosition = new Vector3(targetX, bossfight.position.y, bossfight.position.z);
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (Vector3.Distance(bossfight.position, targetPosition) > 0.01f) 
        {
            float distanceCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distanceCovered / journeyLength;
            bossfight.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            yield return null; 
        }

        bossfight.position = targetPosition;
    }
}