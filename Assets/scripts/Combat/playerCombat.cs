using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class playerCombat : MonoBehaviour {

	public GameObject player;
	public GameObject gridManager;
	public GameObject combatManager;
	public GameObject playerWorld;
	public GameObject spellBook;
	public GameObject spellCreator;

	public GameObject manaUi;
	public GameObject mvmtPointsUi;
	public GameObject lifePointsUi;
	public GameObject spellPointsUi;

	List<GameObject> spells;
	int spellsStillRunning;
	bool waitingForSpells;

	GameObject spellCanvasObject;
	bool playing;
	float step;
    Vector2 target;
    bool shouldWalk;
    bool canWalk;
    bool walkPhase;
    bool attackPhase;

    int currentMana;
    int currentMovementPoints;
    int currentLifePoints;
    int currentSpellPoints;

    List<Vector2> path;
	// Use this for initialization
	void Start () {
		path = new List<Vector2>();
		spells = new List<GameObject>();
		canWalk = true;
        step = playerWorld.GetComponent<playerWorld>().walkSpeed * Time.deltaTime;

		currentMana = player.GetComponent<playerStats>().mana;
		currentMovementPoints = player.GetComponent<playerStats>().mouvement;
        currentLifePoints = player.GetComponent<playerStats>().lifePoints;
		currentSpellPoints = player.GetComponent<playerStats>().spellPoints;
		spellCanvasObject = spellCreator.GetComponent<SpellCreator>().spellCanvasObject;
	}
	
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1") && playing && walkPhase){
        	if (!EventSystem.current.IsPointerOverGameObject()){
        		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        		GameObject g = StaticFunctions.getTileAt(mousePos);
        		target = g.transform.position;
        		int movementCost = gridManager.GetComponent<gridManager>().movementCost(StaticFunctions.getTileAt(player.transform.position),g);
        		if(movementCost <= currentMovementPoints && moveTo(target)){
        			currentMovementPoints -= movementCost;
        			canWalk = false;
        			shouldWalk = true;
        			updateMouvementPoints();
        		}
        	}
        }
        if(shouldWalk){
        	walkTo(target);
        }
        else{
        	canWalk = true;
        }

        if(waitingForSpells && spellsStillRunning <= 0){
        	waitingForSpells = false;
			combatManager.GetComponent<CombatManager>().finishedPlaying();
			combatManager.GetComponent<CombatManager>().nextTurn();
        }
	}

	public void play(){
		walkPhase = true;
		attackPhase = false;
		playing = true;

		greyOut(spellPointsUi);
		foreach(Transform child in spellCanvasObject.transform){
			greyOut(child.gameObject);
		}
		whiteOut(mvmtPointsUi);
        updateHealth();
        updateMana();
        updateMouvementPoints();
        updateSpellPoints();
	}

	public void finishedPhase(){
		if (playing){
			if(walkPhase){
				walkPhase = false;
				attackPhase = true;
				currentMovementPoints = player.GetComponent<playerStats>().mouvement;
				greyOut(mvmtPointsUi);
				whiteOut(spellPointsUi);
				foreach(Transform child in spellCanvasObject.transform){
					whiteOut(child.gameObject);
				}
				updateMouvementPoints();
			}
			else {
				currentSpellPoints = player.GetComponent<playerStats>().spellPoints;
				updateSpellPoints();
				greyOut(spellPointsUi);
				whiteOut(mvmtPointsUi);

				//Update spell durations and move spells
				spellsStillRunning = 0;
				foreach (GameObject s in spells){
					spellsStillRunning++;
	       			s.GetComponent<SpellScript>().nextTurn();
	   			}
	   			waitingForSpells = true;
	   			playing = false;
			}
		}
	}
	public void instantiateSpell(int index){
		if(playing && attackPhase){
			if (index < 0 || index >= spellBook.GetComponent<SpellBook>().getSpellCount() || currentSpellPoints <= 0) return;
			GameObject spell = spellBook.GetComponent<SpellBook>().getSpell(index);
			int availableMana = currentMana - spell.GetComponent<SpellScript>().cost;
			if (availableMana >= 0) {
				currentMana = availableMana;
				currentSpellPoints--;
				spellBook.GetComponent<SpellBook>().instantiateSpell(index);
				updateMana();
				updateSpellPoints();
			}
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
	public void removeSpell(GameObject spell){
		spells.Remove(spell);
	}
	public void spellFinished(){
		spellsStillRunning--;
	}
	public void endCombat(){
		foreach(GameObject spell in spells){
			Destroy(spell);
		}
	}

	public void startCombat(){
		GameObject tile = StaticFunctions.getTileAt(player.transform.position);
		tile.GetComponent<tile>().takeTile(player);
		target = tile.transform.position;
		shouldWalk = true;
	}

	// ui stuff
	void updateMana(){
		manaUi.GetComponent<Text>().text = currentMana + "";
	}
	void updateMouvementPoints(){
		mvmtPointsUi.transform.GetChild(0).GetComponent<Text>().text = currentMovementPoints + "";
	}
	void updateHealth(){
		lifePointsUi.GetComponent<Text>().text = currentLifePoints + "";
	}
	void updateSpellPoints(){
		spellPointsUi.transform.GetChild(0).GetComponent<Text>().text = currentSpellPoints + "";
	}

	//Greys out a ui element
	void greyOut(GameObject uiElem){
		Color c = uiElem.GetComponent<Image>().color;
		c.a = 0.5f;
		uiElem.GetComponent<Image>().color = c;
	}

	void whiteOut(GameObject uiElem){
		Color c = uiElem.GetComponent<Image>().color;
		c.a = 1f;
		uiElem.GetComponent<Image>().color = c;
	}
}
