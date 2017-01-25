using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
[System.Serializable] 
public class Game {
	public static Game current;

    public playerData playerData;
    public Dictionary<string, Dictionary<int, mobData>> sceneSpecificMobs = new Dictionary<string, Dictionary<int, mobData>>();//area name, mobs
    public List<SpellData> spells = new List<SpellData>();
    public string fileName;
	public string yearDate;
	public string monthDate;
	public string dayDate;
	public string savedHour;
	public string savedMinute;
    public string areaName;
    public int sceneIndex;
	public int playerLevel;
    public int editingSpell;

	public Game(){
	}
}