using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager instance;
    public float fadeTime = 20f;
    public float fadeTimer = 20f;
    float alpha = 1f;
    TextMeshProUGUI console;
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }
    private void Start()
    {
        console = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (fadeTimer > 0f)
        {
            fadeTimer -= Time.deltaTime;
            // After 5 sec start to fade
            if (fadeTimer < 10f)
            {
                alpha -= Time.deltaTime;
                console.color = new Color(1f,1f,1f, alpha);
            }
        }
    }

    public void ChangeText(string text)
    {
        console.text = text;
        console.color = new Color(1f, 1f, 1f, 1f);
        fadeTimer = fadeTime; // Reset text and fade
    }

}
