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
    
    void Start()
    {
        startingMaterial = GetComponent<Renderer>().material;
        //audioSource = GetComponent<AudioSource>();
        if (!GetComponent<BoxCollider>())
            gameObject.AddComponent(typeof(MeshCollider));
        
    }

    public void ChangeType(Type newType)
    {
        type = newType;
        GetComponent<MeshFilter>().mesh = GameManager.instance.crater;
    }
    public void ChangeMaterialToHover()
    {
        GetComponent<Renderer>().material = GameManager.instance.hoverMaterial;
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
