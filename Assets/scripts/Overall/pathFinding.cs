using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding {

    public static GameObject getObjectAtMousePos(){
        int layerMask = 1 << gridManager.groundLayerS;
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

    //different to Vector3.Distance, takes into account the isometric view
    //to be used when comparing two objects within the scene
    public static float IsometricDistance(Vector3 first, Vector3 second)
    {
        float dist = Mathf.Sqrt(Mathf.Pow((first.x - second.x),2) + Mathf.Pow((first.y - second.y)*2, 2)); 
        return dist;
    }

    public static GameObject getObjectAt(Vector2 pos){
        int layerMask = 1 << gridManager.groundLayerS;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, layerMask);
        if (hit){
            if (hit.collider != null){
                GameObject obj = hit.collider.gameObject;
                return obj;
            }
        }
        return null;
    }
    public static GameObject getTileAt(Vector2 pos)
    {
        int groundLayerMask = 1 << gridManager.groundLayerS;
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, groundLayerMask);
        if (hit){
            if (hit.collider != null){
                GameObject tile = hit.collider.gameObject;
                return tile;
            }
        }
        return null;
    }

    public static List<GameObject> getNeighbours(GameObject current)
    {
        List<GameObject> neighbours = new List<GameObject>();
        List<Vector2> directions = new List<Vector2>();
        directions.Add(new Vector2(0.5f, -0.25f));
        directions.Add(new Vector2(-0.5f, 0.25f));
        directions.Add(new Vector2(-0.5f, -0.25f));
        directions.Add(new Vector2(0.5f, 0.25f));
        foreach (Vector3 neighbour in directions)
        {
            Vector2 pos = new Vector2();
            pos = current.transform.position + neighbour;
            GameObject neighbourTile = PathFinding.getTileAt(pos);
            if (neighbourTile != null)
                neighbours.Add(neighbourTile);
        }
        return neighbours;
    }
    public static float heuristic_cost_estimate(Transform start, Transform goal)
    {
        return IsometricDistance(start.position, goal.position);
    }

    public static List<GameObject> aStarPathFinding(GameObject currentTile, GameObject targetTile)
    {
        if (currentTile == targetTile)
        {
            List<GameObject> l = new List<GameObject>();
            l.Add(targetTile);
            return l;
        }
        List<GameObject> visited = new List<GameObject>();
        List<GameObject> unvisited = new List<GameObject>();
        unvisited.Add(currentTile);
        Dictionary<GameObject, GameObject> path = new Dictionary<GameObject, GameObject>();
        Dictionary<GameObject, int> graphScores = new Dictionary<GameObject, int>();
        graphScores.Add(currentTile, 0);
        Dictionary<GameObject, float> heuristicScores = new Dictionary<GameObject, float>();
        heuristicScores.Add(currentTile, PathFinding.heuristic_cost_estimate(currentTile.transform, targetTile.transform));

        while (unvisited.Count > 0)
        {
            float currentScore = 9999f;
            GameObject current = currentTile;
            foreach (GameObject i in unvisited)
            {
                if (heuristicScores[i] < currentScore)
                {
                    current = i;
                    currentScore = heuristicScores[i];
                }
            }
            if (current == targetTile)
            {
                List<GameObject> actualPath = new List<GameObject>();
                while (path.ContainsKey(current))
                {
                    current = path[current];
                    actualPath.Insert(0, current);
                }
                actualPath.RemoveAt(0); //take off the current tile 
                actualPath.Add(targetTile); //add the goal tile
                return actualPath;
            }
            unvisited.Remove(current);
            visited.Add(current);
            foreach (GameObject neighbour in PathFinding.getNeighbours(current))
            {
                if (visited.Contains(neighbour) || (neighbour.GetInstanceID() != targetTile.GetInstanceID() && neighbour.GetComponent<tile>().taken))
                {
                    continue;
                }

                int gScore = graphScores[current] + 1;
                if (!unvisited.Contains(neighbour))
                {
                    unvisited.Add(neighbour);
                }
                else if (gScore >= graphScores[neighbour])
                {
                    continue;
                }
                path[neighbour] = current;
                graphScores[neighbour] = gScore;
                heuristicScores[neighbour] = gScore + PathFinding.heuristic_cost_estimate(neighbour.transform, targetTile.transform);
            }
        }
        return new List<GameObject>();
    }

    //should maybe add a limiter in case the nearest free tile is very far away
    public static GameObject findNearestFreeTile(GameObject tile)
    {
        if (! tile.GetComponent<tile>().taken) return tile;
        foreach (GameObject neighbourTile in PathFinding.getNeighbours(tile))
        {
            if (! neighbourTile.GetComponent<tile>().taken)
            {
                return neighbourTile;
            }
        }
        return null;
    }
}
