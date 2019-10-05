using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Element : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler //IPointerClickHandler? 
{
    //public Item item;              
    public GameObject textObj;
    public bool followCursor;
    public Vector3 originalPosition;
    public enum Type {None, NoMatch, Meteor, Water, Sand, Earth, Fire, Coal, Rain}
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

    public void OnBeginDrag(PointerEventData eventData)
    {
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
            // Make sure clicks are not blocked when holding the item, for mouse overs etc not to react.
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
            Debug.Log("DEBUG: Trying to take new obj somehow with already holding one.");
    }

    Type ElementCombiner(Type a, Type b)
    {
        Debug.Log("Checking match for: "+ a +" & "+ b);

        if (a == b) // We are not combining indentical elements
            return Type.NoMatch;

        else if (a == Type.Water)
        {
            if (b == Type.Fire)
                return Type.Rain;
        }
        else if (b == Type.Water)
        {
            if (a == Type.Fire)
                return Type.Rain;
        }
        // By default return no match
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
                    GameManager.instance.EventMeteor(sector.transform.position);
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
            if (sector.tag == "Water")
            {
                if (!onlyCheck)
                    GameManager.instance.EventFireToWater();
                return true;
            }
        }
        if (element == Type.Rain)
        {
        
            if (sectorType == Sector.Type.Sand) //  && GameManager.instance.CheckSectorCount(Sector.Type.Crater) < 1
            {
                if (!onlyCheck)
                {
                    GameManager.instance.ReplaceSector(sector, Sector.Type.Grass);
                    GameManager.instance.EventRain(sector);
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

        // Play sound when letting go of item
        //AudioManager.instance.InvItemPlaceSound(item.category, amount);
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
                Debug.Log("Combining to: " + combinedType);
                // Destroy original UI objects and create new
                combinePosition = GetMidPoint(transform.position, hoveredUIObject.transform.position);

                GameManager.instance.CreateElementObject(combinedType, combinePosition, false);

                GameManager.instance.DestroyUIObject(gameObject);
                GameManager.instance.DestroyUIObject(MouseController.instance.hoveredElement);

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
            if (ElementSectorCombiner(type, MouseController.instance.hoveredSector,false)) // Activates matching event
                Destroy(gameObject);

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
            ReleaseItem();
        else
            ReturnItem();
    }

    public void ReturnItem() // For returning UI Obj to pickup position
    {
        GetComponent<RectTransform>().transform.position = originalPosition;
        ReleaseItem();
    }
    public void ReleaseItem()
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