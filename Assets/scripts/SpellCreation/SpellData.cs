using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//spell
//position
//branches
//spellPoints
//duration
//damage
//movementSpeed
//cost
//position

[System.Serializable]
public class SpellData {
    float xPos;
    float yPos;
    int cost;
    int spriteIndex;
    List<BranchData> branches;
    string spellName;
    public SpellData(Vector2 pos, List<BranchData> b, int c, int sIndex, string name)
    {
        xPos = pos.x;
        yPos = pos.y;
        branches = b;
        cost = c;
        spriteIndex = sIndex;
        spellName = name;
    }
    public Vector2 getPos()
    {
        return new Vector2(xPos,yPos);
    }
    public List<BranchData> getBranches()
    {
        return branches;
    }
    public int getCost()
    {
        return cost;
    }
    public int getUiSpriteIndex() //use to get which sprite to use on the ui
    {
        return spriteIndex;
    }
    public string getSpellName()
    {
        return spellName;
    }
}
[System.Serializable]
public class BranchData
{
    List<PointData> points; //an list of spellpoints which need to be reconstructed into the right structure through the use of "parent index"
    public BranchData(List<PointData> p)
    {
        points = p;
    }
    public List<PointData> getPoints()
    {
        return points;
    }
}
[System.Serializable]
public class PointData
{
    int duration;
    int damage;
    float movementSpeed;
    int cost;
    float xPos;
    float yPos;
    int parentIndex;
    int gameObjectIndex;
    int spriteIndex;
    public PointData(int dur, int dam, float mvmSpeed, int c, Vector2 pos, int pIndex, int GOIndex, int sIndex)
    {
        duration = dur;
        damage = dam;
        movementSpeed = mvmSpeed;
        cost = c;
        xPos = pos.x;
        yPos = pos.y;
        parentIndex = pIndex;
        gameObjectIndex = GOIndex;
        spriteIndex = sIndex;
    }
    public int getDuration()
    {
        return duration;
    }
    public int getDamage()
    {
        return damage;
    }
    public int getCost()
    {
        return cost;
    }
    public float getMovementSpeed()
    {
        return movementSpeed;
    }
    public Vector2 getPosition()
    {
        return new Vector2(xPos, yPos);
    }
    public int getParentIndex() //use to get the parent within the list of Spell Points
    {
        return parentIndex;
    }
    public int getGameObjectIndex() //use to get which base gameObject to use when re creating the spell 
    {
        return gameObjectIndex;
    }
    public int getSpriteIndex() //use to get the sprite of the point
    {
        return spriteIndex;
    }
}
