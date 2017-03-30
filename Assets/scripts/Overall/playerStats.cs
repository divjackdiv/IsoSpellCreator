using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class playerStats : MonoBehaviour {

	public static playerStats instance;
    
    public int level;
	public int lifePoints;
	public int mana;
	public int resistance;
	public int reflex;
	public int movement;
	public int spellPoints; //How many different spells can be created per turn;

	public playerData save(){
		playerData p = new playerData(transform.position, level, lifePoints, mana, resistance, reflex, movement, spellPoints);
		return p;
	}
	
	public void load(){
        if(Game.current.areaName == SceneManager.GetActiveScene().name) transform.position = Game.current.playerData.getLastSavedPos();
		Dictionary<string, int> d = Game.current.playerData.getStats();
		level = d["level"];
		lifePoints = d["lifePoints"];
		mana = d["mana"];
		resistance = d["resistance"];
		reflex = d["reflex"];
		movement = d["movement"];
		spellPoints = d["spellPoints"];
	}

}
