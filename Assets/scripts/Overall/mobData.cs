using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public class mobData {

	float xPosition;
	float yPosition;
	bool hasBeenDestroyed;
	Dictionary<string, int> stats = new Dictionary<string, int>();
	int lifePoints;
	int damage;
	int range;
	int movementPoints;

	public mobData(Vector2 pos, bool destroyed,int lifePoints, int damage, int range, int movementPoints){
		hasBeenDestroyed = destroyed;
		stats.Add("lifePoints", lifePoints);
		stats.Add("damage", damage);
		stats.Add("range", range);
		stats.Add("movementPoints", movementPoints);
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