using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateTile : MonoBehaviour {

    bool creating;
    public GameObject start; // start and end pole
    public GameObject end;

    public GameObject wallPrefab; // to be able to instantiate game wall from script 
    GameObject wall;

    bool xSnapping;
    bool zSnapping;

    bool poleSnapping;

    List<GameObject> poles;

    // Use this for initialization
    void Start() {
        poles = new List<GameObject>();
    }

    // Update is called once per frame
    void Update() {
        getInput();
    }

    void getInput()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is the left-click
        {
            setStart();
        } else if (Input.GetMouseButtonUp(0))
        {
            setEnd();
        } else
        {
            if(creating)
            {
                adjust();
            }
        }

        if(Input.GetKey (KeyCode.X))
        {
            xSnapping = true;
        } else
        {
            xSnapping = false;
        }

        if(Input.GetKey (KeyCode.Z))
        {
            zSnapping = true;
        } else
        {
            zSnapping = false;
        }

        if (Input.GetKey (KeyCode.P))
        {
            poleSnapping = !poleSnapping;
            if (GameObject.FindGameObjectsWithTag("Pole").Length == 0)
            {
                poleSnapping = false;
                Debug.Log("Set some walls before activating pole snapping.");
            }
        }
    }

    Vector3 gridSnap(Vector3 originalPosition)
    {
        int granularity = 1;
        // Example of how function works:
        // granularity = 2
        // 15.8 / 2 = 7.9
        // clamp: 7.9 => 7 (floor func)
        // 7 * 2 = 14
        Vector3 snappedPosition = new Vector3(Mathf.Floor(originalPosition.x / granularity) * granularity, originalPosition.y, Mathf.Floor(originalPosition.z / granularity) * granularity);
        return snappedPosition;
    }

    void setStart()
    {
        creating = true;
        // start.transform.position = getWorldPoint(); // no grid snapping
        start.transform.position = gridSnap(getWorldPoint());
        wall = (GameObject) Instantiate(wallPrefab, start.transform.position, Quaternion.identity); // Quaternion.identity (no rotation), 

        // Snap to Closest Pole
        if (poleSnapping)
        {
            start.transform.position = closestPoleTo(getWorldPoint()).transform.position;
        }
    }

    GameObject closestPoleTo(Vector3 worldPoint)
    {
        GameObject closest = null;
        float distance = Mathf.Infinity;
        float currentDistance = Mathf.Infinity;
        foreach(GameObject p in poles)
        {
            currentDistance = Vector3.Distance(worldPoint, p.transform.position);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = p;
            }
        }
        return closest;
    }

    void setEnd()
    {
        creating = false;
        // end.transform.position = getWorldPoint();
        end.transform.position = gridSnap(getWorldPoint());

        if (xSnapping)
        {
            end.transform.position = new Vector3(start.transform.position.x, end.transform.position.y, end.transform.position.z);
        }

        if (zSnapping)
        {
            end.transform.position = new Vector3(end.transform.position.x, end.transform.position.y, start.transform.position.z);
        }

        setEndPoles();

    }

    void setEndPoles()
    {
        GameObject p1 = (GameObject)Instantiate(wallPrefab, start.transform.position, start.transform.rotation);
        GameObject p2 = (GameObject)Instantiate(wallPrefab, end.transform.position, end.transform.rotation);
        // To add a tag: Main Camera -> Tag: Add Tag... -> click '+' 
        // Todo: learn more about tags
        p1.tag = "Pole";
        p2.tag = "Pole";
        poles.Add(p1);
        poles.Add(p2);
    }

    void adjust()
    {
        // end.transform.position = getWorldPoint();
        end.transform.position = gridSnap(getWorldPoint());

        // whichever axis is being snapped, set the value of end.transform ot the x or z value of the starting pole
        if (xSnapping)
        {
            end.transform.position = new Vector3(start.transform.position.x, end.transform.position.y, end.transform.position.z);
        }

        if (zSnapping)
        {
            end.transform.position = new Vector3(end.transform.position.x, end.transform.position.y, start.transform.position.z);
        }

        // adjust wall size and rotation
        adjustWall();
    }

    void adjustWall()
    {
        start.transform.LookAt(end.transform.position); // start should look at end position, LookAt function of transform
        end.transform.LookAt(start.transform.position);
        float distance = Vector3.Distance(start.transform.position, end.transform.position); // distance between walls
        wall.transform.position = start.transform.position + distance / 2 * start.transform.forward; // start.transform.forward -> direction
        wall.transform.rotation = start.transform.rotation;
        wall.transform.localScale = new Vector3(wall.transform.localScale.x, wall.transform.localScale.y, distance); // x and y parameter should stay the same
    }

    Vector3 getWorldPoint()
    {
        // return the world position of where I clicked on screen
        // can directly address camera component since script is tied to main camera
        // Tutorial guy thinks ray is very accurate -> research other options
       
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
