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
    GameObject currentSpell; //points to the spell the player is creating, if the player creates one
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
    
	void Start ()
    {
        combatManager = overallManager.GetComponent<overallManager>().combatManager;
        spellBook = overallManager.GetComponent<overallManager>().spellBook;
        uiManager = overallManager.GetComponent<overallManager>().uiManager;
        spells = new List<GameObject>();
        drawnPaths = new List<GameObject>();

		currentMana = transform.GetComponent<playerStats>().mana;
		currentMovementPoints = transform.GetComponent<playerStats>().movement;
        currentLifePoints = transform.GetComponent<playerStats>().lifePoints;
		currentSpellPoints = transform.GetComponent<playerStats>().spellPoints;
	}
	
	void Update () {
		if(currentLifePoints <= 0){
			die();
			return;
		}
        if (playing)
        {
            if (walkPhase) //in Walkphase user can see possible paths, and move to nearby tiles
            {
                if (!isWalking)
                {
                    drawPath(); //show possible paths with current movement points
                }
                else if (stillDrawn)
                {
                    clearPath();
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    walkToMousePos();
                }
            }
            else if(attackPhase)// In attack phase, the user can instantiate already made spells
            {
                if (currentSpell != null)
                {
                    if (Input.GetButton("Fire1"))
                    {
                        if (Input.GetKey(KeyCode.Escape))
                            cancelSpell();
                        else
                        {
                            if (Input.GetButton("Rotate"))
                                rotateSpell();
                            updateSpellPos();
                        }
                    }
                    else
                    {
                        updateSpellPos();
                        confirmSpell();
                    }
                }
            }
        }
        if(waitingForSpells && spellsStillRunning <= 0){
        	waitingForSpells = false;
			combatManager.GetComponent<CombatManager>().finishedPlaying();
			combatManager.GetComponent<CombatManager>().nextTurn(); // calling next turn from here will be changed later, the player shouldn't always be the last person to play
        }
	}

	public void play(){
		walkPhase = true;
		attackPhase = false;
		playing = true;
        waitingForSpells = false;

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
			if(walkPhase)
            {
                walkPhase = false;
				attackPhase = true;
                clearPath();
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
                uiManager.GetComponent<UiManager>().changeAlphaSpells(0.5f);

                //Update spell durations and move spells
                spellsStillRunning = 0;
				foreach (GameObject s in spells){
					spellsStillRunning++;
	       			s.GetComponent<SpellScript>().nextTurn();
                    waitingForSpells = true;
                }
                playing = false;
                attackPhase = false;
            }
		}
	}

	public void instantiateSpell(int index){
		if(attackPhase){
			if (index < 0 || index >= spellBook.GetComponent<SpellBook>().getSpellCount() || currentSpellPoints <= 0) return;

			GameObject spell = spellBook.GetComponent<SpellBook>().getSpell(index);
			int availableMana = currentMana - spell.GetComponent<SpellScript>().cost;
            if (availableMana >= 0) {
                currentSpell = spellBook.GetComponent<SpellBook>().instantiateSpell(index);
                updateSpellPos();
                currentMana = availableMana;
                currentSpellPoints--;
                updateMana();
                updateSpellPoints();
            }
		}	
	}

    //updates the position of the spell
    void updateSpellPos()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject g = PathFinding.getTileAt(mousePos);
        if(g != null)
            currentSpell.transform.position = g.transform.position;
        else
            currentSpell.transform.position = mousePos;
    }

    //updates the rotation of the spell
    void rotateSpell()
    {
        //not yet implemented
    }
    //returns true if an appropriate tile has been found, so that the user may not create spells in walls.
    bool canPointAppear(Vector2 pos)
    {
        GameObject g = PathFinding.getTileAt(pos);
        if (g != null)
        {
            if (g.GetComponent<tile>().takenBy != null)
            {
                int layer = g.GetComponent<tile>().takenBy.layer;
                if (layer != 11 && layer != 12)
                    return false;
            }
            return true;
        }
        return false;
    }

    void confirmSpell()
    {
        if (EventSystem.current.IsPointerOverGameObject()) //if user cancels spell
        {
            cancelSpell();
        }
        else
        {
            Transform branch = currentSpell.transform.GetChild(0);
            bool isAllDeleted = true;
            foreach (Transform point in branch)
            {
                bool d = confirmSpellPoint(point);
                if (d)
                    isAllDeleted = false;
            }
            if (isAllDeleted)
                cancelSpell();
            else
            {
                foreach (Transform root in branch)
                {
                    activateSpell(root.gameObject, true);
                }
            }
            currentSpell = null;
        }
    }

    void activateSpell(GameObject spell, bool isRoot)
    {
        if (isRoot)
        {
            spell.SetActive(true);
            spell.GetComponent<SpellPoint>().initPoint();
        }
        else
        {
            spell.SetActive(false);
        }
        foreach (Transform child in spell.transform)
        {
            activateSpell(child.gameObject, false);
        }
    }


    bool confirmSpellPoint(Transform point)
    {
        bool shouldDelete = !canPointAppear(point.position);
        if (shouldDelete)
        {
            Destroy(point.gameObject);
            return false;
        }
        List<GameObject> children = new List<GameObject>();
        foreach (Transform p in point)
        {
            shouldDelete = !canPointAppear(p.position);
            if (shouldDelete)
            {
                children.Add(p.gameObject);
            }
            else
            {
                confirmSpellPoint(p);
            }
        }
        foreach(GameObject g in children)
        {
            Destroy(g);
        }
        return true;
    }

    void cancelSpell()
    {
        currentMana += currentSpell.GetComponent<SpellScript>().cost;
        currentSpellPoints++;
        updateMana();
        updateSpellPoints();
        Destroy(currentSpell);
        currentSpell = null;
    }

    void walkToMousePos()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && !isWalking)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject g = PathFinding.getTileAt(mousePos);
            if (g != null)
            {
                Vector3 target = g.transform.position;
                List<GameObject> path = PathFinding.aStarPathFinding(PathFinding.getTileAt(transform.transform.position), g);

                if (path.Count <= currentMovementPoints)
                {
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
  
    void clearPath()
    {
        foreach (GameObject tile in drawnPaths)
        {
            UiManager.changeAlpha(tile, false, 1f);
        }
    }
}
