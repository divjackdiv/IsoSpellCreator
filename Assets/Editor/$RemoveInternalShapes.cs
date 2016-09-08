/**
 * Author: Engidia SCP - Eduard Bosch (eduardbosch@engidia.com)
 * File: RemoveInternalShapes.cs
 */

using UnityEngine;
using UnityEditor;
using System.Collections;

public class RemoveInternalShapes : MonoBehaviour {

    [MenuItem ("GameObject/Remove Internal Shapes", true)]
    static bool Validate ()
    {
        // Check if there are any selected GameObject with more than one path
        foreach (GameObject lObj in Selection.gameObjects)
        {
            PolygonCollider2D lCollider = lObj.GetComponent<PolygonCollider2D>();
            if (lCollider!=null && lCollider.pathCount > 1)
            {
                return true;
            }
        }

        return false;
    }

 

    [MenuItem ("GameObject/Remove Internal Shapes")]
    static void RemoveShapes ()
    {
        foreach (GameObject lObj in Selection.gameObjects)
        {
            PolygonCollider2D lCollider = lObj.GetComponent<PolygonCollider2D>();
            if (lCollider==null)
            {
                continue;
            }

            // Allow undo action
            Undo.RecordObject (lCollider, "Remove Interior Shapes");


            // Get the shape that are more to the left than the others to take it as the exterior path
            int   lExteriorShape = 0;
            float lLeftmostPoint = Mathf.Infinity;

            Vector2[] lPath;

            for (int i=0, length=lCollider.pathCount; i<length ; ++i)
            {
                lPath = lCollider.GetPath(i);

                foreach (Vector2 lPoint in lPath)
                {
                    if (lPoint.x < lLeftmostPoint)
                    {
                        lExteriorShape = i;
                        lLeftmostPoint = lPoint.x;
                    }
                }
            }

            // Initialize collider with exterior path
            lPath = lCollider.GetPath (lExteriorShape);

            // Set only the exterior path
            lCollider.pathCount = 1;
            lCollider.SetPath (0, lPath);
        }
    }
}
