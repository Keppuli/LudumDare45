using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    //AudioSource audioSource;
    public AudioClip sfx_interact; // Hover over sound
    public bool audioPlayed = false;
    Material startingMaterial;
    public enum Type { Sand, Crater, Lake, Grass, Volcano, Forest, Burned, LakeSeeded, LakeEcosystem,ForestEcosystem,SteppeEcosystem, Sapients, Tribe,Kingdom,Civilization  };
    public Type type;
    public string description;
    public float generateTimer;

    void Start()
    {
        generateTimer = GameManager.instance.elementGenerationTime;

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
                generateTimer = GameManager.instance.elementGenerationTime;
            }
        }
    }

    void GenerateElement()
    {
        if (type == Type.Lake)
        {
            CheckAndProduceElement(Element.Type.Water);
            return;
        }
        else if (type == Type.LakeSeeded)
        {
            CheckAndProduceElement(Element.Type.Water);
            return;
        }
        else if (type == Type.LakeEcosystem)
        {
            CheckAndProduceElement(Element.Type.Life);
            CheckAndProduceElement(Element.Type.Water);
            return;
        }
        else if (type == Type.Volcano)
        {
            CheckAndProduceElement(Element.Type.Fire);
            return;
        }
        else if (type == Type.Burned)
        {
            CheckAndProduceElement(Element.Type.Coal);
            return;
        }
        else if (type == Type.Grass)
        {
            CheckAndProduceElement(Element.Type.Seeds);
            return;
        }
        else if (type == Type.Forest)
        {
            CheckAndProduceElement(Element.Type.Wood);
            CheckAndProduceElement(Element.Type.Seeds);

            return;
        }
        else if (type == Type.ForestEcosystem)
        {
            CheckAndProduceElement(Element.Type.Animals);
            CheckAndProduceElement(Element.Type.Wood);

            return;
        }
        else if (type == Type.Sapients)
        {
            CheckAndProduceElement(Element.Type.Sapients);
            CheckAndProduceElement(Element.Type.Wood);

            return;
        }

        else if (type == Type.SteppeEcosystem)
        {
            CheckAndProduceElement(Element.Type.Animals);
            CheckAndProduceElement(Element.Type.Seeds);

            return;
        }
        else if (type == Type.Tribe || type == Type.Kingdom || type == Type.Civilization)
        {
            CheckAndProduceElement(Element.Type.Technology);
            CheckAndProduceElement(Element.Type.Sapients);

            return;
        }
        else if ( type == Type.Civilization)
        {
            CheckAndProduceElement(Element.Type.Nuke);

            return;
        }
    }
    void CheckAndProduceElement(Element.Type type)
    {
        if (GameManager.instance.CheckElementObjectCount(type) < 1)
            GameManager.instance.CreateElementObject(type, transform.position, true, gameObject);
        else
            Debug.Log("Trying to produce element:"+type+" but there exist enough.");
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
