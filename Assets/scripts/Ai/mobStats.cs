
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mobStats : MonoBehaviour {

	public bool hasBeenDestroyed;
    public int uniqueId;//will have to generate this automatically in the future
    public int lifePoints;
	public int damage;
	public int range;
	public int movementPoints;
    public int detectionRange;


	public mobData save(){
		mobData p = new mobData(transform.position, hasBeenDestroyed, lifePoints, damage, range, movementPoints, detectionRange);
		return p;
	}

	public void load(){        
        if (Game.current.sceneSpecificMobs[Game.current.areaName][uniqueId].isDestroyed()) gameObject.GetComponent<mobCombat>().die();
		transform.position = Game.current.sceneSpecificMobs[Game.current.areaName][uniqueId].getLastSavedPos();
		Dictionary<string, int> d = Game.current.sceneSpecificMobs[Game.current.areaName][uniqueId].getStats();
		lifePoints = d["lifePoints"];
		damage = d["damage"];
		range = d["range"];
        detectionRange = d["detectionRange"];
        movementPoints = d["movementPoints"];
	}
	public int getUniqueId(){
		return uniqueId;
	}
}