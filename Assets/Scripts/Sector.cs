using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    //AudioSource audioSource;
    public AudioClip sfx_interact; // Hover over sound
    public bool audioPlayed = false;
    Material startingMaterial;
    public enum Type { Sand, Crater, Lake, };
    public Type type;
    public string description;

    public float generateTime = 20f;
    public float generateTimer = 20f;

    void Start()
    {
        startingMaterial = GetComponent<Renderer>().material;
        //audioSource = GetComponent<AudioSource>();
        if (!GetComponent<BoxCollider>())
            gameObject.AddComponent(typeof(MeshCollider));
    }
    private void Update()
    {
        if (generateTimer > 0f)
        {
            generateTimer -= Time.deltaTime;
            // After 5 sec start to fade
            if (generateTimer <= 0f)
            {
                GenerateElement();
                generateTimer = generateTime;
            }
        }
    }

    void GenerateElement()
    {
        if (type == Type.Lake)
        {
            if (GameManager.instance.CheckElementObjectCount(Element.Type.Rain) < 1)
                GameManager.instance.CreateElementObject(Element.Type.Rain, transform.position, true);
            return;
        }
    }

    public void ChangeType(Type newType)
    {
        type = newType;
        GetComponent<MeshFilter>().mesh = GameManager.instance.crater;
    }
    public void ChangeMaterialToHover(int type)
    {
        if (type == 0)
            GetComponent<Renderer>().material = GameManager.instance.hoverMaterialNeutral;
        else if (type == 1)
            GetComponent<Renderer>().material = GameManager.instance.hoverMaterialGreen;
        else if (type == 1)
            GetComponent<Renderer>().material = GameManager.instance.hoverMaterialRed;
    }
    public void ResetMaterial()
    {
        GetComponent<Renderer>().material = startingMaterial;
    }

    public void DebugLog(string message)
    {
        Debug.Log(gameObject + " " + message);
    }

    public void PlayHoverSound()
    {
        if (!audioPlayed)       // Play only once when mouse is starting hover
        {
            //audioSource.Play();
            audioPlayed = true; // Reset by calling script
        }
    }
    public void Interact()
    {
        PlayInteractSound();
    }
    public void PlayInteractSound()
    {
        //audioSource.PlayOneShot(sfx_interact);
    }
}
