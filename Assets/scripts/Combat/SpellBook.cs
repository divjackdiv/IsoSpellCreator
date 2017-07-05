using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpellBook : MonoBehaviour {

    public List<GameObject> spellGameObjects;
    public List<Sprite> spellsSprites;
    public GameObject overallManager;
    public GameObject defaultSpell;
    public GameObject defaultBranch;

    List<GameObject> Spells;
    GameObject player;
    GameObject combatManager;
    GameObject uiManager;

    public void Awake()
    {
        combatManager = overallManager.GetComponent<overallManager>().combatManager;
        player = overallManager.GetComponent<overallManager>().player;
        uiManager = overallManager.GetComponent<overallManager>().uiManager;
    }

	public GameObject instantiateSpell(int index)
    {
        if (index < 0 || Spells == null || index > Spells.Count) return null;
		GameObject s = Spells[index];
		s = (GameObject) Instantiate(s);
		s.SetActive(true);
        UiManager.recursiveActive(s.transform.GetChild(0), true);
		player.GetComponent<playerCombat>().addSpell(s);
		s.transform.parent = combatManager.transform;
        return s;
	}

    public List<GameObject> getAllSpells()
    {
        if (Spells == null) return new List<GameObject>();
        return Spells;
    }

	public GameObject getSpell(int index){
		if (index < 0 || index >= Spells.Count) return null;
		return Spells[index];
	}

	public int getSpellCount(){
        if (Spells == null) return 0;
        return Spells.Count;
	}

	

    public void addSpell(GameObject spell){
        if (Spells == null) Spells = new List<GameObject>();
        Spells.Add(spell);
	}

    static public GameObject loadSpell(SpellData s, bool editing, List<Sprite> spellsSprites, List<GameObject> spellsGameObjects, GameObject defaultSpell, GameObject defaultBranch, GameObject player)
    {
        int cost = s.getCost();
        Sprite sprite = spellsSprites[s.getUiSpriteIndex()];
        GameObject spell = (GameObject)Instantiate(defaultSpell, s.getPos(), Quaternion.identity);

        spell.GetComponent<SpellScript>().player = player;
        spell.GetComponent<SpellScript>().cost = cost;
        spell.GetComponent<SpellScript>().setSpellData(s);
        foreach (BranchData b in s.getBranches())
        {
            GameObject branch = (GameObject)Instantiate(defaultBranch);
            List<PointData> points = b.getPoints();
            List<Transform> pointsGO = new List<Transform>();
            foreach (PointData p in points)
            {
                //first Create the right default spell point
                GameObject point = (GameObject)Instantiate(spellsGameObjects[p.getGameObjectIndex()], p.getPosition(), Quaternion.identity);
                //then update the value
                point.GetComponent<SpellPoint>().duration = p.getDuration();
                point.GetComponent<SpellPoint>().damage = p.getDamage();
                point.GetComponent<SpellPoint>().cost = p.getCost();
                point.GetComponent<SpellPoint>().movementSpeed = p.getMovementSpeed();
                point.GetComponent<SpellPoint>().spriteIndex = p.getSpriteIndex();
                //actually change the sprite
                point.GetComponent<SpriteRenderer>().sprite = spellsSprites[p.getSpriteIndex()];
                if (editing)
                {
                    GameObject tile = PathFinding.getTileAt(p.getPosition());
                    tile.GetComponent<tile>().takeTile(point);
                }
                if (p.getParentIndex() == -1)
                {
                    point.transform.parent = branch.transform;
                }
                else
                {
                    point.transform.parent = pointsGO[p.getParentIndex()];
                    if (editing)
                    {
                        point.GetComponent<LineRenderer>().positionCount = 2;
                        point.GetComponent<LineRenderer>().SetPosition(0, pointsGO[p.getParentIndex()].transform.position);
                        point.GetComponent<LineRenderer>().SetPosition(1, p.getPosition());
                        point.GetComponent<LineRenderer>().sortingLayerName = point.GetComponent<SpriteRenderer>().sortingLayerName;
                    }
                }
                pointsGO.Add(point.transform);
            }
            branch.transform.parent = spell.transform;
        }
        return spell;
    }

    public void deleteSpell(int i)
    {
        uiManager.GetComponent<UiManager>().deleteSpell(i);
        Game.current.spells.RemoveAt(i);
        Destroy(transform.GetChild(i).gameObject);
    }
}
