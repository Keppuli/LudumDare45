using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Mesh crater;
    public Material hoverMaterialNeutral;
    public Material hoverMaterialGreen;
    public Material hoverMaterialRed;
    public GameObject dustPE;
    public GameObject raincloudsPE;

    public GameObject[] sectors; // Array of different possibilities
    public GameObject[] elements; // Array of different possibilities
    public List<GameObject> existingElements = new List<GameObject>(); // List of existing elements in the game world
    enum Progression { Meteor, Sand, Crater, Lake };
    Progression progression;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    public void CreateElementObject(Element.Type type, Vector2 position, bool newPosition)
    {
        Vector3 pos;
        if (newPosition)
            pos = Camera.main.WorldToScreenPoint(position); // Figure out position
        else
            pos = position; // Use given

        for (int i = 0; i < elements.Length; i++)
        {
            if (type == elements[i].GetComponent<Element>().type)
            {
                Debug.Log("Matching prefab for Type:");
                var insta = Instantiate(elements[i], pos, Quaternion.identity, GameObject.Find("Canvas").transform);
                insta.transform.SetSiblingIndex(0);
                existingElements.Add(elements[i]);
            }
        }
    }
    public int CheckElementObjectCount(Element.Type type) // Avoid sectors spamming too many objects
    {
        int count = 0;
        for (int i = 0; i < elements.Length; i++)
        {
            if (type == elements[i].GetComponent<Element>().type)
                count++;
        }
        return count;
    }

    public void DestroyUIObject(GameObject obj)
    {
        existingElements.Remove(obj);
        Destroy(obj);
    }

    public void EventTutorial()
    {
        ConsoleManager.instance.ChangeText("First there was nothing but a barren realm.");

    }
    public void EventMeteor(Vector3 position)
    {
        //sand.GetComponent<Sector>().enabled = false; // Destroy sector
        progression = Progression.Meteor;

        Debug.Log("Meteor event activated!");
        ConsoleManager.instance.ChangeText("Comet brought something new to this barren land. Try mixing them up.");
        CameraController.instance.shakeDuration = 1f;
        CreateElementObject(Element.Type.Fire, position,true);
        CreateElementObject(Element.Type.Water, position, true);
        CreateElementObject(Element.Type.Water, position, true);
        Instantiate(dustPE, position, Quaternion.identity);
    }

    public void EventLake(Vector3 position)
    {
        //lake.GetComponent<Interactable>().enabled = false;
        ConsoleManager.instance.ChangeText("Fire and water combine to form rainy clouds o");
        progression = Progression.Lake;
        Instantiate(raincloudsPE, position, Quaternion.identity,GameObject.Find("World").transform);
    }

    public void EventFireToWater()
    {
       
    }
}
