using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler //IPointerClickHandler? 
{
    //public Item item;              
    public GameObject textObj;
    public bool followCursor;
    public Vector3 originalPosition;
    public enum Type {None, NoMatch, Meteor, Water, Sand, Earth, Fire, Coal, Rain}
    public Type type;

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
    bool ElementSectorCombiner(Type element, GameObject sector)
    {
        var sectorType = sector.GetComponent<Sector>().type;
        if (element == Type.Meteor) // Tutorial meteor
        {
            if (sectorType == Sector.Type.Sand)
            {
                sector.GetComponent<Sector>().ChangeType(Sector.Type.Crater);
                GameManager.instance.EventMeteor(sector.transform.position);
                return true;
            }
        }

        if (element == Type.Water)
        {
            if (sector.tag == "Sand")
            {
                GameManager.instance.EventLake(sector.transform.position);
                return true;
            }
        }
        if (element == Type.Fire)
        {
            if (sector.tag == "Water")
            {
                GameManager.instance.EventFireToWater();
                return true;
            }
        }
        if (element == Type.Rain)
        {
            if (sector.tag == "Water")
            {
                if (sectorType == Sector.Type.Sand)
                {
                    sector.GetComponent<Sector>().ChangeType(Sector.Type.Lake);
                    GameManager.instance.EventLake(sector.transform.position);
                    return true;
                }
            }
        }
        // By default return false
        return false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Play sound when letting go of item
        //AudioManager.instance.InvItemPlaceSound(item.category, amount);
#region UI Object
        /*
        if (eventData.pointerEnter) // Trash can functionality
        {
            //Debug.Log("OnEndDrag eventData.pointerEnter: "+ eventData.pointerEnter.name);
            if (eventData.pointerEnter.name == "TrashCan")
            {
                Destroy(gameObject);
                return;
            }
        }
        */
        // Check if mouse over UI object
        if (MouseController.instance.hoveredElement)
        {
            Debug.Log("Dragging on HoveredUiOBJ");

            // Dragging on other UI object will combine the two if match OK
            var hoveredUIObject = MouseController.instance.hoveredElement;
            var combinedType = ElementCombiner(type, hoveredUIObject.GetComponent<Draggable>().type);
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
            if (ElementSectorCombiner(type, MouseController.instance.hoveredSector)) // Activates matching event
                Destroy(gameObject);

        }
#endregion


        // Check if mouse over 3D object
        /*
        else if (MouseController.instance.mouseOverInventorySlot != null)
        {
            //Debug.Log("OnEndDrag on inventory slot.");

            hostInventory.GetComponent<Inventory>().ColorItemSlots(true, gameObject, MouseController.instance.mouseOverInventorySlot);
            InventorySlot slot = MouseController.instance.mouseOverInventorySlot.GetComponent<InventorySlot>();
            //  Check if held obj can fit in the NEW area
            if (slot.hostInventory.GetComponent<Inventory>().ReserveSlots(gameObject, MouseController.instance.mouseOverInventorySlot))
            {
                // Update inventory item count
                slot.hostInventory.GetComponent<Inventory>().UpdateItemDictionary(gameObject, true);
                MoveToSlot(slot);

                // Check if we are dropping on Crafting Inventory
                if (slot.hostInventory.GetComponent<CraftingInput>())
                {
                    slot.hostInventory.GetComponent<CraftingInput>().CheckIncredientsRecipeCorrelation(); // Check if crafting should spawn item
                }
            }
            // If new inventory area lacks space, return object to ORIGINAL area
            else
                ReturnItem();
        }
        */
        // Player is dropping item return item to original position
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