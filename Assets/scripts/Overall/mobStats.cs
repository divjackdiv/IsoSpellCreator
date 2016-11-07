
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mobStats : MonoBehaviour {

	public bool hasBeenDestroyed;
	int index;
	public int lifePoints;
	public int damage;
	public int range;
	public int movementPoints;
	void Awake(){}
	void Start(){
		if(index == 0) index = Game.current.mobCount++;
	}
	public mobData save(){
		mobData p = new mobData(transform.position, hasBeenDestroyed, lifePoints, damage, range, movementPoints);
		return p;
	}

	public void load(){
		index = Game.current.mobCount++;
		if(Game.current.mobs[index].isDestroyed()) gameObject.GetComponent<mobCombat>().die();
		transform.position = Game.current.mobs[index].getLastSavedPos();
		Dictionary<string, int> d = Game.current.mobs[index].getStats();
		lifePoints = d["lifePoints"];
		damage = d["damage"];
		range = d["range"];
		movementPoints = d["movementPoints"];
	}
	public int getIndex(){
		return index;
	}
}