using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateTile : MonoBehaviour {

    public GameObject startTile; // start and end pole
    public GameObject tilePrefab; // to be able to instantiate game tile from script 

    bool tileSnapping;

    List<GameObject> tiles;

    // Use this for initialization
    void Start() {
        startTile = (GameObject)Instantiate(tilePrefab, Vector3.zero, Quaternion.identity);
        tiles = new List<GameObject>();
        tiles.Add(startTile);
    }

    // Update is called once per frame
    void Update() {
        getInput();
    }

    void getInput()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is the left-click
        {
            placeTile();
        } 
    }

    void placeTile()
    {
        Vector3 tapPosition = getWorldPoint();
        Vector3 newTilePos = newTilePosition(tapPosition);

        GameObject tile = (GameObject) Instantiate(tilePrefab, newTilePos, Quaternion.identity); // Quaternion.identity (no rotation), 
        tiles.Add(tile);
    }

    Vector3 newTilePosition(Vector3 tapPoint)
    {
        GameObject closestTile = null;
        float minDistance = Mathf.Infinity;
        float currentDistance = Mathf.Infinity;
        foreach(GameObject p in tiles) {
            currentDistance = Vector3.Distance(tapPoint, p.transform.position);
            if(currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestTile = p;
            }
        }

        bool aboveZPosX = isAboveZPosX(closestTile.transform.position, tapPoint);
        bool aboveZNegX = isAboveZNegX(closestTile.transform.position, tapPoint);
        
        return findProperAdjacency(closestTile, aboveZPosX, aboveZNegX);
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

    bool isAboveZPosX(Vector3 originTile, Vector3 tapPoint)
    {
        return tapPoint.z - originTile.z > tapPoint.x - originTile.x;
    }

    bool isAboveZNegX(Vector3 originTile, Vector3 tapPoint)
    {
        return tapPoint.z - originTile.z > -1 * (tapPoint.x - originTile.x);
    }

    Vector3 findProperAdjacency(GameObject closestTile, bool aboveZPosX, bool aboveZNegX)
    {
        Vector3 returnPosition = Vector3.zero;

        if (aboveZPosX && aboveZNegX) // top
        {
            returnPosition = new Vector3(closestTile.transform.position.x, 0, closestTile.transform.position.z + 1);
        }
        else if (aboveZPosX && !aboveZNegX) // left
        {
            returnPosition = new Vector3(closestTile.transform.position.x - 1, 0, closestTile.transform.position.z);
        }
        else if (!aboveZPosX && aboveZNegX) //right
        {
            returnPosition = new Vector3(closestTile.transform.position.x + 1, 0, closestTile.transform.position.z);
        }
        else if (!aboveZPosX && !aboveZNegX) // bottom
        {
            returnPosition = new Vector3(closestTile.transform.position.x, 0, closestTile.transform.position.z - 1);
        }

        return returnPosition;
    }
}
