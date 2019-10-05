using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interacter : MonoBehaviour
{
    public GameObject hoveredObject;
    public Material hoveredMaterial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit, 999f))
        {
            Debug.Log("Raycast hit on interactable: " + hit.point);



            else
            {
                clearHover();
            }
        }
        */
        // Take a position from the camera and convert it to a point in world space
        // In this case, right in front of the player

        
        //Vector3 useRay = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
             
        if (Physics.Raycast(ray, out hit, 99f))
        {
            //Debug.Log("mouseover hit: " + hit.transform.gameObject);
            // Calculate closest point from objects bounds
            //Vector3 closestPoint = interactable.coll.ClosestPointOnBounds(transform.position);
            //Vector3.Distance(closestPoint, transform.position) <1f

            if (hit.collider.GetComponent<Sector>())
            {
                Sector interactable = hit.collider.GetComponent<Sector>();
                // Debug.Log("Raycast hit on interactable: " + hit.collider.name);
                // Add outline material on hover
                HoverObject(interactable.gameObject);
                // INTERACTION
          
            }

            if (Input.GetMouseButtonDown(0))
            {
                // If there is interactable in the traced object, damage it

                // Add force to wiggle it a bit
                if (hit.rigidbody)
                {
                    Debug.Log("Click on: "+ hit.collider.name);
                }
            }
        }
        else
        {
            clearHover();
        }
        
    }
    void HoverObject(GameObject obj)
    {
        Debug.Log("HoverObject() on: " + obj.name);
        obj.GetComponent<Sector>().PlayHoverSound();
        obj.GetComponent<Sector>().DebugLog("MOUSEOVER");

        if (hoveredObject)
        {
            if (obj == hoveredObject)
                return;
            clearHover();
        }

        hoveredObject = obj;
        hoveredObject.GetComponent<Sector>().ChangeMaterialToHover();
        Debug.Log(hoveredObject.GetComponent<Renderer>().material);
      
    }
    void clearHover()
    {
        if (!hoveredObject)
        {
            return;
        }
        hoveredObject.GetComponent<Sector>().audioPlayed = false;
        hoveredObject.GetComponent<Sector>().ResetMaterial();
        hoveredObject = null;
        
    }
}

