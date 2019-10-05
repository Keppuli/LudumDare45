using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public static MouseController instance;

    public GameObject heldItem;
    public Texture2D cursorTexture;

    public GameObject cursorObj;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public GameObject hoveredSector;
    public GameObject hoveredElement;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        //Cursor.visible = false;
    }
    void Update()
    {
        CheckMouseOver();
    }
    public bool CheckMouseBounds() // Checks that mouse is within screen bounds
    {
        if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1) return false;
        return true;
    }

    void CheckMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 99f))
        {
      
            if (hit.collider.GetComponent<Sector>())
            {
                Sector interactable = hit.collider.GetComponent<Sector>();
                // Debug.Log("Raycast hit on interactable: " + hit.collider.name);
                // Add outline material on hover
                HoverObject(interactable.gameObject);
            }

            if (Input.GetMouseButtonDown(0))
            {
                // Add force to wiggle stuff randomly
                if (hit.rigidbody)
                {
                    Debug.Log("Click on: " + hit.collider.name);
                }
            }
        }
        else
        {
            ClearHover();
        }
    }
    void HoverObject(GameObject obj)
    {
        Debug.Log("HoverObject() on: " + obj.name);
        obj.GetComponent<Sector>().PlayHoverSound();
        obj.GetComponent<Sector>().DebugLog("MOUSEOVER");

        if (hoveredSector)
        {
            if (obj == hoveredSector)
                return;
            ClearHover();
        }

        hoveredSector = obj;
        hoveredSector.GetComponent<Sector>().ChangeMaterialToHover();
        Debug.Log(hoveredSector.GetComponent<Renderer>().material);

    }
    void ClearHover()
    {
        if (!hoveredSector)
        {
            return;
        }
        hoveredSector.GetComponent<Sector>().audioPlayed = false;
        hoveredSector.GetComponent<Sector>().ResetMaterial();
        hoveredSector = null;
    
    }
}