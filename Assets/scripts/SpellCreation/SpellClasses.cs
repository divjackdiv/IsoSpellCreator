using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
/*
    Spells : 
        first Spell:
            first branch:
                first point:
                    duration;
                    position;
                    specs;
*/
public class Spell {   
	public GameObject spellGameObject;
    public List<Branch> branches;

    public Spell(GameObject s){
        spellGameObject = s;
        branches = new List<Branch>();
    }
    public GameObject getGameObject(){
        return spellGameObject;
    }
}


public class Branch {
	public GameObject branchGameObject;
    public Point root;  
    public Spell spell;
    public List<Point> currentPoints;
    public Branch(Spell s, GameObject b){
    	currentPoints = new List<Point>();
        branchGameObject = b;
        spell = s;
    }
    public Spell getSpell(){
        return spell;
    }
    public GameObject getGameObject(){
        return branchGameObject;
    }
}

public class Point {
    public int duration;
    Vector2 position;
    GameObject pointGameObject;
    public Point parent;
    public List<Point> children;  
    public Branch branch;

    public Point(Branch b, int d,Vector2 pos, GameObject s, Point p){  
        children = new List<Point>();  
        branch = b;
        duration = d;
        position = pos;
        parent = p; // !!!! parent is not a game object parent, just a parent in the data structure
        pointGameObject = s;
    }
    public void changeDuration(int newDuration){
        duration = newDuration;
    }
    public void changePos(Vector2 newPos){
        position = newPos;
    }
    public GameObject getGameObject(){
        return pointGameObject;
    }
    public Branch getBranch(){
        return branch;
    }
    public List<Point> getChildren(){
        return children;
    }
    public Vector2 getPosition(){
        return position;
    }
}