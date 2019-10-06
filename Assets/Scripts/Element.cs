using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FMOD;

public class Element : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler 
{
    //public Item item;              
    public GameObject textObj;
    public bool followCursor;
    public Vector3 originalPosition;
    public enum Type {None, NoMatch, Meteor, Water, Sand, Earth, Fire, Wood,Coal, Rain, Volcano, Seeds, Life, Animals, Tribe, Monolith, Sapients,Technology }
    public Type type;
    public string description;

    private void Start()
    {
        originalPosition = GetComponent<RectTransform>().transform.position; // Get Canvas position
    }

    private void Update()
    {
        if (followCursor)
            transform.position = Input.mousePosition;
    }


    void PlayPickUpSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Pick");
    }
    void PlayDropSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Drop" +
            "");
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        PlayPickUpSound();

        if (MouseController.instance.hoveredSector)
            MouseController.instance.hoveredSector.GetComponent<Sector>().ResetMaterial();

        GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
        MouseController.instance.draggingElement = true;
        GetComponent<BoxCollider2D>().isTrigger = true;

        // Play pickup sound
        //AudioManager.instance.InvItemPickupSound(item.category, amount);

        // Set dragged item top of the hierarchy (sets Canvas draw order)
        transform.SetSiblingIndex(9999);

        // No item is being held in hand (Can't hold 2 items at once)
        if (!MouseController.instance.heldItem)
        {
            // Pick up the UI item
            MouseController.instance.heldItem = gameObject;
            followCursor = true;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            //Debug.Log("DEBUG: Trying to take new obj somehow with already holding one.");
            ReturnElement();
        }
    }

    Type ElementCombiner(Type a, Type b)
    {
        //Debug.Log("Checking match for: "+ a +" & "+ b);

        if (a == b) // We are not combining indentical elements
            return Type.NoMatch;

        if (a == Type.Water)
        {
            if (b == Type.Fire)
                return Type.Rain;
        }
        if (b == Type.Water)
        {
            if (a == Type.Fire)
                return Type.Rain;
        }
        if (a == Type.Coal)
        {
            if (b == Type.Water)
                return Type.Life;
        }
        if (b == Type.Coal)
        {
            if (a == Type.Water)
                return Type.Life;
        }
        else if (a == Type.Wood)
        {
            if (b == Type.Fire)
                return Type.Coal;
        }
        else if (b == Type.Wood)
        {
            if (a == Type.Fire)
                return Type.Coal;
        }
        else if (a == Type.Sapients)
        {
            if (b == Type.Technology)
                return Type.Tribe;
        }
        else if (b == Type.Sapients)
        {
            if (a == Type.Technology)
                return Type.Tribe;
        }
        // By default return no match
        //Debug.Log("DEBUG: No Match for: "+a);
        return Type.NoMatch;
    }
    bool ElementSectorCombiner(Type element, GameObject sector, bool onlyCheck)
    {
        var sectorType = sector.GetComponent<Sector>().type;
        if (element == Type.Meteor) // Tutorial meteor
        {
            if (sectorType == Sector.Type.Sand)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Crater);
                    GameManager.instance.EventMeteor(sector);
                }
                return true;
            }
        }
        if (element == Type.Volcano)
        {
            if (sectorType == Sector.Type.Sand)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Volcano);
                    GameManager.instance.EventVolcano(sector);
                }
                return true;
            }
            if (sectorType == Sector.Type.Grass)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Volcano);
                    GameManager.instance.EventVolcano(sector);
                }
                return true;
            }
        }
        if (element == Type.Water)
        {
            if (sectorType == Sector.Type.Crater)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Lake);
                    GameManager.instance.EventLake(sector);
                }
                return true;
            }
        }
        if (element == Type.Fire)
        {
            if (sectorType == Sector.Type.Grass || sectorType == Sector.Type.Forest)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Burned);
                    GameManager.instance.EventBurned();
                }
                return true;
            }
        }
  
        if (element == Type.Rain)
        {
        
            if (sectorType == Sector.Type.Sand || sectorType == Sector.Type.Burned)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Grass);
                    GameManager.instance.EventRain(sector);
                }
                return true;
            }
        }
        if (element == Type.Seeds)
        {
            if (sectorType == Sector.Type.Lake) 
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.LakeSeeded);
                    GameManager.instance.EventSeededLake(sector);
                }
                return true;
            }
            if (sectorType == Sector.Type.Grass)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Forest);
                    GameManager.instance.EventForest();
                }
                return true;

            }
        }
        if (element == Type.Seeds)
        {
            if (sectorType == Sector.Type.Grass) 
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Forest);
                    GameManager.instance.EventForest();
                }
                return true;
            }
        }
        if (element == Type.Life)
        {
            if (sectorType == Sector.Type.Forest)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.ForestEcosystem);
                    GameManager.instance.EventForestEcosystem();
                }
                return true;
            }
            if (sectorType == Sector.Type.LakeSeeded)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.LakeEcosystem);
                    GameManager.instance.EventLakeEcosystem(sector);
                }
                return true;
            }
        }
        if (element == Type.Monolith)
        {
            if (sectorType == Sector.Type.ForestEcosystem)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Sapients);
                    GameManager.instance.EventSapients();
                }
                return true;
            }
        }
        if (element == Type.Animals)
        {
            if (sectorType == Sector.Type.Grass)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.SteppeEcosystem);
                    GameManager.instance.EventSteppe();
                }
                return true;
            }
        }
        if (element == Type.Sapients)
        {
            if (sectorType == Sector.Type.SteppeEcosystem)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Tribe);
                    GameManager.instance.EventTribe();
                }
                return true;
            }
        }
        if (element == Type.Technology)
        {
            if (sectorType == Sector.Type.Tribe)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Kingdom);
                    GameManager.instance.EventKingdom();
                }
                return true;
            }
            if (sectorType == Sector.Type.Kingdom)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Civilization);
                    GameManager.instance.EventCivilization();
                }
                return true;
            }
        }
        if (element == Type.Tribe)
        {
            if (sectorType != Sector.Type.Tribe || sectorType != Sector.Type.Kingdom || sectorType != Sector.Type.Civilization)
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Tribe);
                    GameManager.instance.EventTribeForced();
                }
                return true;
            }
        }

        // By default return false
        return false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MouseController.instance.draggingElement = false;
        PlayDropSound();
#region UI Object
        // Check if mouse over UI object
        if (MouseController.instance.hoveredElement)
        {
            //Debug.Log("Dragging on HoveredUiOBJ");

            // Dragging on other UI object will combine the two if match OK
            var hoveredUIObject = MouseController.instance.hoveredElement;
            var combinedType = ElementCombiner(type, hoveredUIObject.GetComponent<Element>().type);
            Vector2 combinePosition;
            if (combinedType != Type.NoMatch)
            {
                //Debug.Log("Combining to: " + combinedType);
                // Destroy original UI objects and create new
                combinePosition = GetMidPoint(transform.position, hoveredUIObject.transform.position);
                GameManager.instance.CreateElementObject(combinedType, combinePosition, false,null);
                GameManager.instance.DestroyElement(gameObject);
                GameManager.instance.DestroyElement(MouseController.instance.hoveredElement);

                //Debug.Log("ERROR: Matching prefab for Type:"+combinedType + " not found!");
                //Instantiate(); // Instantiate under Canvas
            }
        }
        else
            ReleaseWithCheck();
#endregion
#region 3D Object
        if (MouseController.instance.hoveredSector)
        {
            if (ElementSectorCombiner(type, MouseController.instance.hoveredSector, false)) // Activates matching event
                GameManager.instance.DestroyElement(gameObject);
        }
#endregion
        else
        {
            ReleaseWithCheck();
        }
    }
    void ReleaseWithCheck()
    {
        if (MouseController.instance.CheckMouseBounds())
            ReleaseElement();
        else
            ReturnElement();
    }

    public void ReturnElement() // For returning UI Obj to pickup position
    {
        GetComponent<RectTransform>().transform.position = originalPosition;
        ReleaseElement();
    }
    public void ReleaseElement()
    {
        if (MouseController.instance.hoveredSector)
            MouseController.instance.hoveredSector.GetComponent<Sector>().ResetMaterial();

        GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

        GetComponent<BoxCollider2D>().isTrigger = false;
        followCursor = false;    // Release mouse follow
        GetComponent<CanvasGroup>().blocksRaycasts = true;      // Take raycast again
        MouseController.instance.heldItem = null;               // Release reference to held item
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseController.instance.hoveredElement = gameObject;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MouseController.instance.hoveredElement = null;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (MouseController.instance.hoveredSector)
        {
            if (ElementSectorCombiner(type, MouseController.instance.hoveredSector, true))
                MouseController.instance.hoveredSector.GetComponent<Sector>().ChangeMaterialToHover(1); // yeas
            else
                MouseController.instance.hoveredSector.GetComponent<Sector>().ChangeMaterialToHover(2); // noes

            //Debug.Log("Check on element: "+type+ " against: "+ MouseController.instance.hoveredSector.GetComponent<Sector>().type + " is fit: "+ ElementSectorCombiner(type, MouseController.instance.hoveredSector, true));
        }
    }
    Vector3 GetMidPoint(Vector3 a, Vector3 b)
    {
        // Direction between the two transforms
        Vector3 dir = (b - a).normalized;

        // Direction that crosses our [dir] direction
        Vector3 perpDir = Vector3.Cross(dir, Vector3.right);

        // Midway point
        Vector3 midPoint = (a + b) / 2f;

        // Offset point
        return midPoint + perpDir;
    }
}