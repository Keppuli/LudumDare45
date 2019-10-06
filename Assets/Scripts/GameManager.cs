using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum Progression { Meteor, Crater, Lake, Volcano };
    public Progression progression;
    public GameObject fireworksPE;
    public float elementGenerationTime;
    Vector2 elementUISpawnPosition = new Vector2(-453f, 320f);
    public GameObject elementSectorSpawnRotator;
    public Material hoverMaterialNeutral;
    public Material hoverMaterialGreen;
    public Material hoverMaterialRed;
    public GameObject dustPE;
    public GameObject raincloudsPE;
    public bool monolithSpawned;
    public GameObject[] sectors; // Array of different possibilities
    public GameObject[] elements; // Array of different possibilities
    public List<GameObject> existingSectors = new List<GameObject>(); // List of existing sectors in the game world
    public List<GameObject> existingElements = new List<GameObject>(); // List of existing elements in the game world

    public float startTimer = 20f;
    bool tutorialOn;
    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }


    private void Update()
    {
        if (startTimer > 0f)
        {
            startTimer -= Time.deltaTime;
            // After 5 sec start to fade
            if (startTimer <= 0f)
            {
                CreateElementObject(Element.Type.Meteor, elementUISpawnPosition);
                ConsoleManager.instance.ChangeText("But by happy accident a roque comet came by this barren realm..");

            }
            else if (startTimer < 15f && startTimer > 0f)
            {
                ConsoleManager.instance.ChangeText("For a long time there was nothing..");
            }
            

        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            foreach (var element in elements)
            {
                CreateElementObject(element.GetComponent<Element>().type, elementUISpawnPosition);
            }
        }
    }
    public void ReplaceSector(GameObject oldSector, Sector.Type type)
    {
        for (int i = 0; i < sectors.Length; i++)
        {
            //Debug.Log("ReplaceSector() iterating:  " + sectors[i].GetComponent<Sector>().type);
            if (type == sectors[i].GetComponent<Sector>().type)
            {
                var insta = Instantiate(sectors[i], oldSector.transform.position, Quaternion.identity, GameObject.Find("World").transform);
                insta.transform.rotation = oldSector.transform.rotation;
                existingSectors.Remove(oldSector);
                existingSectors.Add(insta);
                Destroy(oldSector);
                return;
            }
        }
        //Debug.Log("ERROR: ReplaceSector() failed to replace sector of type: "+ type);
    }
    public void CreateElementObject(Element.Type type, Vector2 canvasPos)
    {
        for (int i = 0; i < elements.Length; i++)
        {
            if (type == elements[i].GetComponent<Element>().type)
            {
                var insta = Instantiate(elements[i], canvasPos, Quaternion.identity, GameObject.Find("Elements").transform);
                insta.GetComponent<RectTransform>().anchoredPosition = canvasPos;
                existingElements.Add(insta);
                return;
            }
        }
        //Debug.Log("CreateElementObject() failed to find needed element to spawn! Check GameManager list for element; " + type);
    }
    public void CreateElementObject(Element.Type type, Vector2 position, bool convertToWorldPos, GameObject sector)
    {
        //Debug.Log("Creating element: "+type);
        Vector3 pos = position; // Directly spawned and merged elements use Canvas position

        if (convertToWorldPos) // Sector spawning needs converted world pos
        {
            elementSectorSpawnRotator.transform.rotation = sector.transform.rotation; // Rotate spawner to match sector rotation
            pos = Camera.main.WorldToScreenPoint(elementSectorSpawnRotator.transform.GetChild(0).transform.position); // Use spawners spawn pos
        }
      
        for (int i = 0; i < elements.Length; i++)
        {
            if (type == elements[i].GetComponent<Element>().type)
            {
                var insta = Instantiate(elements[i], pos, Quaternion.identity, GameObject.Find("Elements").transform);
                existingElements.Add(insta);
                return;
            }
        }
        //Debug.Log("CreateElementObject() failed to find needed element to spawn! Check GameManager list for element; "+ type);
    }
    public int CheckElementObjectCount(Element.Type type) // Avoid sectors spamming too many objects
    {
        int count = 0;
        for (int i = 0; i < existingElements.Count; i++)
        {
            if (type == existingElements[i].GetComponent<Element>().type)
                count++;
        }
        return count;
    }
    public int CheckSectorCount(Sector.Type type) // For tutorial game flow
    {
        int count = 0;
        for (int i = 0; i < existingSectors.Count; i++)
        {
            if (type == existingSectors[i].GetComponent<Sector>().type)
                count++;
        }
        return count;
    }

    public void DestroyElement(GameObject obj)
    {
        existingElements.Remove(obj);
        Destroy(obj);
     
    }

    public void EventMeteor(GameObject sector)
    {
        ConsoleManager.instance.ChangeText("Comet brought new elements. Things that could mixed and form even more new things..");
        CameraController.instance.shakeDuration = 1f;
        CreateElementObject(Element.Type.Fire, sector.transform.position, true, sector);
        CreateElementObject(Element.Type.Water, sector.transform.position, true, sector);
        CreateElementObject(Element.Type.Water, sector.transform.position, true, sector);
        InstantiatePE(dustPE, sector);

    }
    public void EventVolcano(GameObject sector)
    {
        ConsoleManager.instance.ChangeText("Violent volcano has erupted and is now generating fire.");
        CameraController.instance.shakeDuration = 1f;
        elementSectorSpawnRotator.transform.rotation = sector.transform.rotation;
        CreateElementObject(Element.Type.Fire, sector.transform.position, true, sector);
        CreateElementObject(Element.Type.Fire, sector.transform.position, true, sector);
        InstantiatePE(dustPE, sector);
    }
    void InstantiatePE(GameObject pe, GameObject sector)
    {
        var insta = Instantiate(pe, sector.transform.position, Quaternion.identity, GameObject.Find("World").transform);
        insta.transform.rotation = sector.transform.rotation;
    }
    #region Lake Progression
    public void EventLake(GameObject sector)
    {
        ConsoleManager.instance.ChangeText("A lake has settled and is now producing water");
        //InstantiatePE(raincloudsPE, sector);
        // Progress 
        CreateElementObject(Element.Type.Volcano, elementUISpawnPosition);
    }
    public void EventSeededLake(GameObject sector)
    {
        ConsoleManager.instance.ChangeText("A lake has been filling with plantlife and ready to host complex life");
        //InstantiatePE(raincloudsPE, sector);
    }
    public void EventLakeEcosystem(GameObject sector)
    {
        ConsoleManager.instance.ChangeText("A lake has transformed into ecosystem and is now producing lifeforms");
        //InstantiatePE(raincloudsPE, sector);
    }
    #endregion
    public void EventBurned()
    {
        ConsoleManager.instance.ChangeText("Sector has burned down, producing coal");
        //InstantiatePE(raincloudsPE, sector);

    }
    public void EventForest()
    {
        ConsoleManager.instance.ChangeText("Forest has formed, ready to host complex life");
        //InstantiatePE(raincloudsPE, sector);

    }
    public void EventForestEcosystem()
    {
        ConsoleManager.instance.ChangeText("Forest ecosystem has formed and is now producing Animals");
        //InstantiatePE(raincloudsPE, sector);
        if (!monolithSpawned)
        {
            CreateElementObject(Element.Type.Monolith, elementUISpawnPosition);
            monolithSpawned = true;
        }
    }
    public void EventSapients()
    {
        ConsoleManager.instance.ChangeText("Sapient lifeforms have evolved in forest with the help of mysterious Monolith");
        //InstantiatePE(raincloudsPE, sector);
    }

    public void EventSteppe()
    {
        ConsoleManager.instance.ChangeText("Steppe ecosystem with domesticable animals have formed");
        //InstantiatePE(raincloudsPE, sector);
    }
    public void EventTribe()
    {
        ConsoleManager.instance.ChangeText("Tribe has formed and is now producing Technology");
        //InstantiatePE(raincloudsPE, sector);
    }
    public void EventTribeForced()
    {
        ConsoleManager.instance.ChangeText("With the help of Technology, new living area has been conquered!");
        //InstantiatePE(raincloudsPE, sector);
    }
    public void EventKingdom()
    {
        ConsoleManager.instance.ChangeText("Technology is advancing");
        //InstantiatePE(raincloudsPE, sector);
    }
    public void EventCivilization()
    {
        ConsoleManager.instance.ChangeText("Technology is advancing");
        //InstantiatePE(raincloudsPE, sector);
        //Debug.Log("Civ count: "+ CheckSectorCount(Sector.Type.Civilization));
        if (CheckSectorCount(Sector.Type.Civilization) == 8)
        {
            fireworksPE.SetActive(true);
            ConsoleManager.instance.ChangeText("Sapients have conquered all living space and formed Ecumenopolis. You win!");
            ConsoleManager.instance.fadeTime = 9999f;
            ConsoleManager.instance.fadeTimer = 9999f;

        }
    }
    public void EventNuke()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Nuked");
        ConsoleManager.instance.ChangeText("Oh humanity!");
        //InstantiatePE(raincloudsPE, sector);
    }
    public void EventBurn()
    {
        ConsoleManager.instance.ChangeText("Sector has burned down and is now producing coal.");
        //InstantiatePE(raincloudsPE, sector);
    }
    public void EventRain(GameObject sector)
    {
        ConsoleManager.instance.ChangeText("Rain nourishes the sector..");
        var insta = Instantiate(raincloudsPE, sector.transform.position, Quaternion.identity, GameObject.Find("World").transform);
        insta.transform.rotation = sector.transform.rotation;
    }
    public void EventFireToWater()
    {
        ConsoleManager.instance.ChangeText("Roaming fire is extinguished by the lake, forming rain..");

    }
}
