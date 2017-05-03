using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class tileCreatorWindow : EditorWindow
{
    public static Object objToSpawn;
    static List<bool> foldouts;
    static Dictionary<string, Dictionary<Object, bool>> objects;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/tileCreatorWindow")]
    void OnEnable()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(tileCreatorWindow));
        ShowWindow();
    }

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(tileCreatorWindow));
        objects = new Dictionary<string, Dictionary<Object, bool>>();
        foldouts = new List<bool>();
        string[] directories = Directory.GetDirectories("Assets/prefabs/World/");
        foreach (string name in directories)
        {
            foldouts.Add(false);
            objects.Add(name, new Dictionary<Object, bool>());
            System.IO.DirectoryInfo info = new DirectoryInfo(name + "/");
            FileInfo[] fileInfo = info.GetFiles("*.prefab");
            foreach (FileInfo file in fileInfo)
            {
                Object t = AssetDatabase.LoadAssetAtPath(name + "/" + file.Name, typeof(GameObject));
                objects[name].Add(t, false);
            }
        }
    }

    void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= tileCreationEditor.OnScene;
        SceneView.onSceneGUIDelegate += tileCreationEditor.OnScene;
    }

    void OnGUI()
    {
        int l = 0;
        if (objects != null)
        {
            bool anyToggled = false;   //is any object toggled?
            foreach (string name in objects.Keys)
            {
                GUILayout.BeginVertical();
                foldouts[l] = EditorGUILayout.Foldout(foldouts[l], name);
                if (foldouts[l])
                {
                    int i = 0;
                    GUILayout.BeginHorizontal();
                    List<Object> objs = new List<Object>(objects[name].Keys);
                    foreach (Object t in objs)
                    {
                        GameObject tObj = (GameObject)t;
                        Texture image = tObj.GetComponent<SpriteRenderer>().sprite.texture;
                        objects[name][t] = GUILayout.Toggle(objects[name][t], image, "Button", GUILayout.MaxHeight(64), GUILayout.MaxWidth(64));
                        if (objects[name][t])
                        {
                            foreach (string n in objects.Keys)
                            {
                                List<Object> objs2 = new List<Object>(objects[n].Keys);
                                foreach (Object t2 in objs2)
                                {
                                    if (t != t2)
                                        objects[n][t2] = false;
                                }
                            }
                            objToSpawn = t;
                            anyToggled = true;
                        }
                        i++;
                        if (i % 4 == 0)
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                l++;
                GUILayout.EndVertical();
            }
            if (!anyToggled)
            {
                objToSpawn = null;
            }
        }
    }
}
