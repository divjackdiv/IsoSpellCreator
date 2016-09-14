using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


public class playerCombat : MonoBehaviour {

	public GameObject player;
	public GameObject combatManager;
	public GameObject playerWorld;
	List<GameObject> spells;
	bool playing;
	float step;
    Vector2 target;
    bool shouldWalk;
    bool canWalk;
	// Use this for initialization
	void Start () {
		spells = new List<GameObject>();
		canWalk = true;
        step = playerWorld.GetComponent<playerWorld>().walkSpeed * Time.deltaTime;   
	}
	
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1") && playing && canWalk){
        	if (!EventSystem.current.IsPointerOverGameObject()){
        		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        		GameObject g = StaticFunctions.getTileAt(mousePos);
        		target = g.transform.position;
        		if(moveTo(target)){
        			canWalk = false;
        			shouldWalk = true;
        		}
        	}
        }
        if(shouldWalk){
        	walkTo(target);
        }
        else{
        	canWalk = true;
        }
	}

	public void play(){
		playing = true;
	}
	public void finishedPlaying(){
		if (playing){
			foreach (GameObject s in spells){
       			s.GetComponent<SpellScript>().nextTurn();
   			}
   			playing = false;
			combatManager.GetComponent<CombatManager>().finishedPlaying();
			combatManager.GetComponent<CombatManager>().nextTurn();
		}
	}
	bool moveTo(Vector2 pos){
		GameObject newTile = StaticFunctions.getTileAt(pos);
		bool moved = newTile.GetComponent<tile>().takeTile(player);
		if (moved){
			GameObject currentTile = StaticFunctions.getTileAt(player.transform.position);
			currentTile.GetComponent<tile>().leaveTile();
			return true;
		}
		return false;
	}

	void walkTo(Vector2 position){
        player.transform.position = Vector3.MoveTowards(player.transform.position, position, step);
        Vector2 p = new Vector2(player.transform.position.x, player.transform.position.y);
        if( p == position) shouldWalk = false;
	}
	public void addSpell(GameObject spell){
		spells.Add(spell);
	}
	public void endCombat(){
		foreach(GameObject spell in spells){
			Destroy(spell);
		}
		//reset stats (like hp ?)

	}
}
