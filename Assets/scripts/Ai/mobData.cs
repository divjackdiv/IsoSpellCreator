using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public class mobData {


    Dictionary<string, int> stats = new Dictionary<string, int>();
    float xPosition;
	float yPosition;
	bool hasBeenDestroyed;

	public mobData(Vector2 pos, bool destroyed,int lifePoints, int damage, int range, int movementPoints, int detectionRange)
    {
		hasBeenDestroyed = destroyed;
		stats.Add("lifePoints", lifePoints);
		stats.Add("damage", damage);
		stats.Add("range", range);
		stats.Add("movementPoints", movementPoints);
        stats.Add("detectionRange", detectionRange);
        xPosition = pos.x;
		yPosition = pos.y;
	}

	public bool isDestroyed(){
		return hasBeenDestroyed;
	}
	public Vector2 getLastSavedPos(){
		return new Vector2(xPosition,yPosition);
	}

	public Dictionary<string, int> getStats(){
		return stats;
	}
}