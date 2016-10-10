using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public class playerData {

	float xPosition;
	float yPosition;

	Dictionary<string, int> stats = new Dictionary<string, int>();

	public playerData(Vector2 pos, int level,int lifePoints, int mana, int resistance, int reflex, int movement, int spellPoints){
		stats.Add("level", level);
		stats.Add("lifePoints", lifePoints);
		stats.Add("mana", mana);
		stats.Add("resistance", resistance);
		stats.Add("reflex", reflex);
		stats.Add("movement", movement);
		stats.Add("spellPoints", spellPoints);
		xPosition = pos.x;
		yPosition = pos.y;
	}
	public Vector2 getLastSavedPos(){
		return new Vector2(xPosition,yPosition);
	}

	public Dictionary<string, int> getStats(){
		return stats;
	}
}