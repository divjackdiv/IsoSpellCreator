﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour {

	public static SaveManager instance = null;
	public static List<Game> savedGames = new List<Game>();

	void Awake()
    {
	    if (instance == null) instance = this;
	    else if (instance != this) Destroy(gameObject);   

	    DontDestroyOnLoad(gameObject);
	}

	public static void Save(Game g) {
	    SaveManager.savedGames.Add(g);
	    BinaryFormatter bf = new BinaryFormatter();
	    FileStream file = File.Create(Application.persistentDataPath + "/" + g.fileName);
        Application.CaptureScreenshot(Application.persistentDataPath + "/" + g.fileName + ".png");
	    bf.Serialize(file, g);
	    file.Close();
	}

	public static void Load(string filename) {
	    if(File.Exists(filename)) {
	        BinaryFormatter bf = new BinaryFormatter();
	        FileStream file = File.Open(filename, FileMode.Open);
	        Game s = (Game) bf.Deserialize(file);
	        SaveManager.savedGames.Add(s);
	        file.Close();
	    }
	}
}
