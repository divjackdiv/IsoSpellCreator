using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;

///OLD GRID MANAGER not used anymore
public class oldGridManager : MonoBehaviour {

    //World Creation
    Vector2 origin;
    public float widthOfWorld;
    public float heightOfWorld;
    public int widthOfGrid;
    public int heightOfGrid;
    public List<Dictionary<int, GameObject>> worldGrid;
    float Xstep;
    float Ystep;

    List<GameObject> createdSpells;

    public void Start(){
        origin = transform.position;
        createdSpells = new List<GameObject>();
        Application.targetFrameRate = 60;
        Xstep = widthOfWorld /widthOfGrid;
        Ystep = heightOfWorld / heightOfGrid;
        worldGrid = new List<Dictionary<int, GameObject>>();
        createGrid(widthOfGrid,heightOfGrid);
    }

    public void createGrid(int x, int y){
        for (int i = 0; i < x; i++){
            worldGrid.Add(new Dictionary<int, GameObject>());
            for (int j = 0; j < y; j++){
                worldGrid[i].Add(j,null);
            }
        }
    }
    public bool addToGrid(GameObject g, Vector2 mousePosition){
        int x = (int) mousePosition.x;
        int y = (int) mousePosition.y;
        if(x < 0 || y < 0  || x >= worldGrid.Count || y >= worldGrid[x].Count) return false;
        if(worldGrid[x][y] != null) return false;
        worldGrid[x][y] = g;
        return true;
    }
    public bool removeFromGrid(Vector2 mousePosition){
        int x = (int) mousePosition.x;
        int y = (int) mousePosition.y;
        if(x < 0 || y < 0  || x >= worldGrid.Count || y >= worldGrid[x].Count) return false;
        worldGrid[x][y] = null;
        return true;
    }
    public bool isTaken(Vector2 mousePosition){
        int x = (int)(Mathf.Round(mousePosition.x/Xstep));
        int y = (int)(Mathf.Round(mousePosition.y/Ystep));
        if( x < 0 || y < 0 || x >= worldGrid.Count || y >= worldGrid[x].Count || worldGrid[x][y] != null) return true; 
        return false;
    }
    public bool isOutOfBounds(Vector2 pos){  
        int x = (int)(Mathf.Round(pos.x/Xstep));
        int y = (int)(Mathf.Round(pos.y/Ystep));  
        if( x < 0 || y < 0 || x >= worldGrid.Count || y >= worldGrid[x].Count) return true; 
        return false;
    }

    public Vector2 nearestPoint(Vector2 v){
        float x = (Mathf.Round((origin.x + v.x)/Xstep))*Xstep;
        float y = (Mathf.Round((origin.y + v.y)/Ystep))*Ystep;
        return new Vector2(x,y);
    }

    public Vector2 convertPosToGrid(Vector2 v){
        float x = Mathf.Round(v.x/Xstep);
        float y = Mathf.Round(v.y/Ystep);
        return new Vector2(x,y);
    }

}
