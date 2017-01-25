using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class playerCombat : MonoBehaviour {

    public GameObject overallManager;
	public GameObject manaUi;
	public GameObject mvmtPointsUi;
	public GameObject lifePointsUi;
	public GameObject spellPointsUi;


    GameObject combatManager; //derived from overallManager
    GameObject spellBook;   //derived from overallManager
    GameObject uiManager;   //derived from overallManager
    GameObject spellCanvasObject;
    List<GameObject> spells;
    List<GameObject> drawnPaths; //List used to show possible paths
    int spellsStillRunning;
    int currentMana;
    int currentMovementPoints;
    int currentLifePoints;
    int currentSpellPoints;
    bool playing;
    bool waitingForSpells;
    bool walkPhase;
    bool attackPhase;
    bool stillDrawn; //are paths still drawn and need to be cleaned up?
    bool isWalking;

    void Awake()
    {
        combatManager = overallManager.GetComponent<overallManager>().combatManager;
        spellBook = overallManager.GetComponent<overallManager>().spellBook;
        uiManager = overallManager.GetComponent<overallManager>().uiManager;
    }
    
	void Start () {
        spells = new List<GameObject>();
        drawnPaths = new List<GameObject>();

		currentMana = transform.GetComponent<playerStats>().mana;
		currentMovementPoints = transform.GetComponent<playerStats>().movement;
        currentLifePoints = transform.GetComponent<playerStats>().lifePoints;
		currentSpellPoints = transform.GetComponent<playerStats>().spellPoints;
	}
	
	
	// Update is called once per frame
	void Update () {
		if(currentLifePoints <= 0){
			die();
			return;
		}
        if (walkPhase && ! isWalking)
        {
            drawPath();
        }
        else if (stillDrawn)
        {
            foreach (GameObject tile in drawnPaths)
            {
                UiManager.changeAlpha(tile, false, 1f);
            }
        }
		if (Input.GetButtonDown("Fire1") && playing && walkPhase){

        	if (!EventSystem.current.IsPointerOverGameObject() && !isWalking)
            {
        		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        		GameObject g = PathFinding.getTileAt(mousePos);
        		if(g != null){
        			Vector3 target = g.transform.position;
                    List<GameObject> path = PathFinding.aStarPathFinding(PathFinding.getTileAt(transform.transform.position), g);

                    if (path.Count <= currentMovementPoints){
                        bool canMove = GetComponent<playerOverall>().takeTile(target);
                        if (canMove)
                        {
                            isWalking = true;
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

        UiManager.changeAlpha(spellPointsUi, true, 0.5f);
        UiManager.changeAlpha(mvmtPointsUi, true, 1f);
        uiManager.GetComponent<UiManager>().changeAlphaSpells(0.5f);
        updateHealth();
        updateMana();
        updateMovementPoints();
        updateSpellPoints();
	}

	public void finishedPhase(){
		if (playing &&  !isWalking){
			if(walkPhase){
				walkPhase = false;
				attackPhase = true;
				currentMovementPoints = transform.GetComponent<playerStats>().movement;
                UiManager.changeAlpha(mvmtPointsUi, true, 0.5f);
                UiManager.changeAlpha(spellPointsUi, true, 1f);
                uiManager.GetComponent<UiManager>().changeAlphaSpells(1f);
                updateMovementPoints();
			}
			else {
				currentSpellPoints = transform.GetComponent<playerStats>().spellPoints;
				updateSpellPoints();
                UiManager.changeAlpha(spellPointsUi, true, 0.5f);
                UiManager.changeAlpha(mvmtPointsUi, true, 1f);

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
    public void finishedWalking()
    {
        isWalking = false;
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
        GetComponent<playerWorld>().enabled = true;
        this.enabled = false;
	}
	public void takeDamage(int damage){
		currentLifePoints -= damage;
		if(currentLifePoints <= 0){
			currentLifePoints = 0;
		}
		updateHealth();
	}

	public void die(){
		print("you died, you little shit, you're lucky I didnt have time to actually program your death yet");
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


    //shows possible paths to the user
	void drawPath(){
        stillDrawn = true;
		GameObject t = PathFinding.getTileAt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if(t != null){
			GameObject currentTile = PathFinding.getTileAt(transform.position);
			if(currentTile != null){
				foreach (GameObject tile in drawnPaths)
                {
				    UiManager.changeAlpha(tile, false, 1f);
			    }
        		if (!EventSystem.current.IsPointerOverGameObject()){
                    drawnPaths = PathFinding.aStarPathFinding(currentTile, t);
					for (int i = 0; i < currentMovementPoints && i < drawnPaths.Count; i++){
                        if (i == drawnPaths.Count - 1 && drawnPaths[i].GetComponent<tile>().taken) {  //if the last item in path is taken then do not draw it
                        }
                        else {
                            UiManager.changeAlpha(drawnPaths[i], false, 0.5f);
                        }
					}
				}
			}
		}
	}
  
}
