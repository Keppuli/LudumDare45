using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public float shakeDuration = 0f;    // How long the object should shake for
    public float shakeAmount = 0.7f;    // Amplitude of the shake. A larger value shakes the camera harder
    public float decreaseFactor = 1.0f; // Shake decrease time

    private Vector3 originalPos;        // Stores camera's coordinates before shakes and follows
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            // Randomize new position inside sphere shape, which size is calculated using shakeAmount
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            // Calculate time to stop shaking
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }
}
