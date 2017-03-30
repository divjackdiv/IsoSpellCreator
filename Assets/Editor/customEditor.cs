using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameObject))]
public class tileCreationEditor : Editor
{
    public static Vector3 mousePosition;
    private void OnScene(SceneView sceneview)
    {
        Event e = Event.current;
        if (e.type == EventType.keyDown)
        {
            if(e.keyCode == (KeyCode.T))
            {
                if (tileCreatorWindow.objToSpawn != null)
                {
                    mousePosition = e.mousePosition;
                    mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
                    mousePosition = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
                    Object objToSpawn = tileCreatorWindow.objToSpawn;
                    if (((GameObject) objToSpawn).layer == 9)
                    {
                        gridManager.createTileSceneView(objToSpawn, mousePosition);
                    }
                    else if (((GameObject)objToSpawn).layer > 9 && ((GameObject)objToSpawn).layer < 12)
                    {
                        gridManager.createLargeObjSceneView(objToSpawn, mousePosition);
                    }
                    else
                    {
                        gridManager.createSmallObjSceneView(objToSpawn, mousePosition);
                    }
                }
            }
        }
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate -= OnScene;
        SceneView.onSceneGUIDelegate += OnScene;
    }
}
