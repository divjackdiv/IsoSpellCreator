using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class playerStats : MonoBehaviour {

    public static playerStats instance;

    public int level;
    public int movement;

    public int mana;
    public int manaRegen;
    public int lifePoints;
    public int spellSlots;
    public int range;
    public int spellPoints; //How many different spells can be created per turn;
    public int resistance;
    public int reflex;

    public int startMana;
    public int startManaRegen;
    public int startLifePoints;
    public int startSpellSlots;
    public int startRange;
    public int startSpellPoints;

    //public int startResistance;
    //public int startReflex;
    //public int startMovement;


    public playerData save() {
        playerData p = new playerData(transform.position, level, lifePoints, mana, resistance, reflex, movement, spellPoints);
        return p;
    }

    public void load() {
        if (Game.current.areaName == SceneManager.GetActiveScene().name) transform.position = Game.current.playerData.getLastSavedPos();
        Dictionary<string, int> d = Game.current.playerData.getStats();
        level = d["level"];
        lifePoints = d["lifePoints"];
        mana = d["mana"];
        //resistance = d["resistance"];
        //reflex = d["reflex"];
        movement = d["movement"];
        spellPoints = d["spellPoints"];
    }

    public void levelUp()
    {
        setLevelTo(level + 1);
    }

    public void setLevelTo(int lvl)
    {
        level = lvl;
        lifePoints = startLifePoints;
        mana = startMana;
        manaRegen = startManaRegen;
        range = startRange;
        spellPoints = startSpellPoints;
        spellSlots = startSpellSlots;
        float manaGrowthRate = 2 / 3;
        for (int i = 2; i < lvl; i++)
        {
            mana += (int) (manaGrowthRate * mana);
            manaGrowthRate += 1 / 14;
            manaRegen += 1; 
            lifePoints += (int) ((startLifePoints) * (Mathf.Pow(0.9f, i-2)));
            spellPoints += (int)((startSpellPoints) * (Mathf.Pow(1.18f, i - 2)));
            spellSlots += 1;
        }
    }
}
