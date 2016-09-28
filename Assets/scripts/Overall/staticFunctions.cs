using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StaticFunctions {

 	public static Vector2 getMousePosition(int groundLayer){ 
 		int layerMask = 1<<groundLayer;
        Vector2 mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, layerMask)){
            mousePosition  = ray.GetPoint(hit.distance);
        }
        else{
            Debug.DrawRay (ray.origin, ray.direction * 10, Color.red);
            mousePosition = new Vector2(-5,-5);
        }
        return mousePosition;
    }

    public static GameObject getObjectAtMousePos(int layerMask){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, layerMask);
        if (hit){
            if (hit.collider != null){
                GameObject obj = hit.collider.gameObject;
                return obj;
            }
        }
        return null;
    }

    public static GameObject getObjectAt(Vector2 pos, int layerMask){
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, layerMask);
        if (hit){
            if (hit.collider != null){
                GameObject obj = hit.collider.gameObject;
                return obj;
            }
        }
        return null;
    }
    public static GameObject getTileAt(Vector2 pos){
        int groundLayerMask = 1<<9;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, groundLayerMask);
        if (hit){
            if (hit.collider != null){
                GameObject tile = hit.collider.gameObject;
                return tile;
            }
        }
        return null;
    }


    public static int movementCost(GameObject currentTile, GameObject target){
        float xCost = Mathf.Abs(currentTile.transform.position.x - target.transform.position.x); 
        float yCost = Mathf.Abs(currentTile.transform.position.y - target.transform.position.y); 
        xCost *= 2;
        yCost *= 4;
        int totalCost = (int) xCost;
        if (totalCost < yCost) totalCost = (int) yCost;
        return totalCost;
    }
    public static List<GameObject> getPath(GameObject currentTile, GameObject target){
        List<GameObject> path = new List<GameObject>();
        Vector2 currentPos = currentTile.transform.position;
        Vector2 targetPos = target.transform.position;
        float xDifference = currentPos.x - targetPos.x;
        float yDifference = currentPos.y - targetPos.y;
        int max = 100;
        while (xDifference != 0 || yDifference != 0){
            max--;
            if(max <= 0 ) return new List<GameObject>(); //If anything goes wrong this returns an empty list
            if(xDifference == 0){
                if(yDifference > 0){
                    currentPos.x -= 0.5f;
                    currentPos.y -= 0.25f;
                }
                //if y is negative and x does not matter
                else{
                    currentPos.x -= 0.5f;
                    currentPos.y += 0.25f;
                }
            }
            else if(yDifference == 0){
                //if x is positive and y does not matter
                if(xDifference > 0){
                    currentPos.x -= 0.5f;
                    currentPos.y -= 0.25f;
                }
                //if x is negative and y does not matter
                else{
                    currentPos.x += 0.5f;
                    currentPos.y -= 0.25f;
                }
            }
            else{
                if(xDifference > 0){
                    //if both x and y are positive
                    if(yDifference > 0){
                        currentPos.x -= 0.5f;
                        currentPos.y -= 0.25f;
                    }
                    //if x is positive and y is negative
                    else{
                        currentPos.x -= 0.5f;
                        currentPos.y += 0.25f;
                    }
                }
                else {
                    //if x is negative but y is positive
                    if(yDifference > 0){
                        currentPos.x += 0.5f;
                        currentPos.y -= 0.25f;
                    }

                    //if x and y are negative
                    else{
                        currentPos.x += 0.5f;
                        currentPos.y += 0.25f;
                    }
                }
            }
            xDifference = currentPos.x - targetPos.x;
            yDifference = currentPos.y - targetPos.y;
            path.Add(StaticFunctions.getTileAt(currentPos));
        }
        return path;
    }
}
