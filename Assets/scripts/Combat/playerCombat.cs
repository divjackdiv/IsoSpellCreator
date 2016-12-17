using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class playerCombat : MonoBehaviour {

	public GameObject combatManager;
	public GameObject spellBook;
	public GameObject overallManager;

	public GameObject manaUi;
	public GameObject mvmtPointsUi;
	public GameObject lifePointsUi;
	public GameObject spellPointsUi;


	List<GameObject> spells;
	int spellsStillRunning;
	bool waitingForSpells;

    GameObject spellCanvasObject;
	bool playing;
   	List<GameObject> drawnPaths; //List used to show possible paths
    bool walkPhase;
    bool attackPhase;
    bool stillDrawn; //are paths still drawn and need to be cleaned up?

    int currentMana;
    int currentMovementPoints;
    int currentLifePoints;
    int currentSpellPoints;
    

	void Start () {
		spells = new List<GameObject>();
        drawnPaths = new List<GameObject>();

		currentMana = transform.GetComponent<playerStats>().mana;
		currentMovementPoints = transform.GetComponent<playerStats>().movement;
        currentLifePoints = transform.GetComponent<playerStats>().lifePoints;
		currentSpellPoints = transform.GetComponent<playerStats>().spellPoints;
		spellCanvasObject = overallManager.GetComponent<overallManager>().spellCanvasObject;
	}
	
	
	// Update is called once per frame
	void Update () {
		if(currentLifePoints <= 0){
			die();
			return;
		}
		if(walkPhase) drawPath();
        else if (stillDrawn)
        {
            foreach (GameObject tile in drawnPaths)
            {
                whiteOut(tile, false);
            }
        }
		if (Input.GetButtonDown("Fire1") && playing && walkPhase){
        	if (!EventSystem.current.IsPointerOverGameObject() && !GetComponent<playerOverall>().isMoving()){
        		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        		GameObject g = StaticFunctions.getTileAt(mousePos);
        		if(g != null){
        			Vector3 target = g.transform.position;
                    List<GameObject> path = StaticFunctions.aStarPathFinding(StaticFunctions.getTileAt(transform.transform.position), g);

                    if (path.Count <= currentMovementPoints){
                        bool canMove = GetComponent<playerOverall>().takeTile(target);
                        if (canMove)
                        {
                            GetComponent<playerOverall>().updatePath(path);
                            currentMovementPoints -= path.Count;
                            updateMovementPoints();
                        }
	        		}
        		}
        	}
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

		greyOut(spellPointsUi, true);
		foreach(Transform child in spellCanvasObject.transform){
			greyOut(child.gameObject, true);
		}
		whiteOut(mvmtPointsUi, true);
        updateHealth();
        updateMana();
        updateMovementPoints();
        updateSpellPoints();
	}

	public void finishedPhase(){
		if (playing &&  !GetComponent<playerOverall>().isWalking()){
			if(walkPhase){
				walkPhase = false;
				attackPhase = true;
				currentMovementPoints = transform.GetComponent<playerStats>().movement;
				greyOut(mvmtPointsUi, true);
				whiteOut(spellPointsUi, true);
				foreach(Transform child in spellCanvasObject.transform){
					whiteOut(child.gameObject, true);
				}
				updateMovementPoints();
			}
			else {
				currentSpellPoints = transform.GetComponent<playerStats>().spellPoints;
				updateSpellPoints();
				greyOut(spellPointsUi, true);
				whiteOut(mvmtPointsUi, true);

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
	public void takeDamage(int damage){
		currentLifePoints -= damage;
		if(currentLifePoints <= 0){
			currentLifePoints = 0;
		}
		updateHealth();
	}

	public void die(){
		print("you died, you little shit, you're lucky I didnt have time to actually programme your death yet");
		//play death animation;
		//menu appears;
	}
	
	public void startCombat(){
        GetComponent<playerOverall>().moveToNearestTile();
	}
	// ui stuff
	void updateMana(){
		manaUi.transform.GetChild(0).GetComponent<Text>().text = currentMana + "";
	}
	void updateMovementPoints(){
		mvmtPointsUi.transform.GetChild(0).GetComponent<Text>().text = currentMovementPoints + "";
	}
	void updateHealth(){
		lifePointsUi.transform.GetChild(0).GetComponent<Text>().text = currentLifePoints + "";
	}
	void updateSpellPoints(){
		spellPointsUi.transform.GetChild(0).GetComponent<Text>().text = currentSpellPoints + "";
	}

	//Greys out a ui element
	void greyOut(GameObject obj, bool isUiElement){
		if(isUiElement){
			Color c = obj.GetComponent<Image>().color;
			c.a = 0.5f;
			obj.GetComponent<Image>().color = c;
		}
		else{
			Color c = obj.GetComponent<SpriteRenderer>().color;
			c.a = 0.5f;
			obj.GetComponent<SpriteRenderer>().color = c;
		}
	}

	void whiteOut(GameObject obj, bool isUiElement){
		if(isUiElement){
			Color c = obj.GetComponent<Image>().color;
			c.a = 1f;
			obj.GetComponent<Image>().color = c;
		}
		else{
			Color c = obj.GetComponent<SpriteRenderer>().color;
			c.a = 1f;
			obj.GetComponent<SpriteRenderer>().color = c;
		}
	}

    //shows possible paths to the user
	void drawPath(){
        stillDrawn = true;
		GameObject t = StaticFunctions.getTileAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if(t != null){
			GameObject currentTile = StaticFunctions.getTileAt(transform.position);
			if(currentTile != null){
				foreach (GameObject tile in drawnPaths)
                {
				    whiteOut(tile, false);
			    }
        		if (!EventSystem.current.IsPointerOverGameObject()){
                    drawnPaths = StaticFunctions.aStarPathFinding(currentTile, t);
					for (int i = 0; i < currentMovementPoints && i < drawnPaths.Count; i++){
                        if (i == drawnPaths.Count - 1 && drawnPaths[i].GetComponent<tile>().taken) {  //if the last item in path is taken then do not draw it
                        }
                        else {
                            greyOut(drawnPaths[i], false);
                        }
					}
				}
			}
		}
	}
  
}
