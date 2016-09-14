using UnityEngine;
using System.Collections;

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

}
