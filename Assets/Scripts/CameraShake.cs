using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private bool shaking = false;
    private float elapsed = 0f;
    private float duration = 0f;
    private float magnitude = 0f;
    private Vector3 originalPos;

    public void Shake(float duration, float magnitude)
    {
        if (shaking)
            return;
        
        this.originalPos = transform.localPosition;
        this.elapsed = 0f;
        this.duration = duration;
        this.magnitude = magnitude;
        this.shaking = true;
    }

    void Update()
    {
        if (!shaking)
            return;
        
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            shaking = false;
            transform.localPosition = originalPos;
            return;
        }

        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;

        transform.localPosition = originalPos + new Vector3(x, y, originalPos.z);
    }
}
