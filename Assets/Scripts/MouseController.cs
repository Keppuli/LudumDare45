using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseController : MonoBehaviour
{
    public static MouseController instance;

    public GameObject mouseOverInfo;
    public TextMeshProUGUI mouseOverInfoText;

    public GameObject heldItem;
    public Texture2D cursorTexture;

    public GameObject cursorObj;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public GameObject hoveredSector;
    public GameObject hoveredElement;

    public bool draggingElement;

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
        mouseOverInfo.transform.position = Input.mousePosition;
        UpdateMouseOverInfo();
    }
    public void UpdateMouseOverInfo()
    {
        //mouseOverInfo.transform.SetSiblingIndex(999); // Place at bottom of Canvas hiererchy to maintain on top position

        if (hoveredElement)
        {
            mouseOverInfoText.text = hoveredElement.GetComponent<Element>().description;
        }
        else if (hoveredSector)
        {
            mouseOverInfoText.text = hoveredSector.GetComponent<Sector>().description;
        }
        else
            mouseOverInfoText.text = "";


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
        //Debug.Log("HoverObject() on: " + obj.name);
        obj.GetComponent<Sector>().PlayHoverSound();
        //obj.GetComponent<Sector>().DebugLog("MOUSEOVER");

        if (hoveredSector)
        {
            if (obj == hoveredSector)
                return;
            ClearHover();
        }

        hoveredSector = obj;
        if (!draggingElement)
            hoveredSector.GetComponent<Sector>().ChangeMaterialToHover(0);
        //Debug.Log(hoveredSector.GetComponent<Renderer>().material);

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