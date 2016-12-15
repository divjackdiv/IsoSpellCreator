using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
[System.Serializable] 
public class Game {
	public static Game current;
	public string fileName;
	public string yearDate;
	public string monthDate;
	public string dayDate;
	public string savedHour;
	public string savedMinute;
	public int sceneIndex;
	public int playerLevel;
	public string areaName;
	public playerData playerData;
    public List<SpellData> spells = new List<SpellData>();
	public int mobCount = 1;
	public Dictionary<int, mobData> mobs = new Dictionary<int, mobData>();

	public Game(){
	}
}