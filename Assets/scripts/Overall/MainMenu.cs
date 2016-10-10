using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic; 
using System.IO;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour {


	public static bool isSceneMainMenu;
	public GameObject SaveTemplate; 
	public Vector2 startPos;
	public float displacement;
	public Transform saves;
	public Transform load;

	public void Awake(){
		if(SceneManager.GetActiveScene().buildIndex == 0) isSceneMainMenu = true;
		else isSceneMainMenu = false;
		if(Game.current == null){
			//LOAD ALL SAVES INFO
	    	DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
			FileInfo[] info = dir.GetFiles("*.*");
			foreach (FileInfo f in info){
				string filename = Path.GetFileName(f.Name);
				if(!filename.Contains(".png")){
					SaveManager.Load(f.FullName);
				}
			}
			//LOAD SAVE BUTTONS
			if(!MainMenu.isSceneMainMenu) Game.current = new Game();
		}
		if(!MainMenu.isSceneMainMenu) loadAllSaveButtons(saves, false);
		loadAllSaveButtons(load, true);
	}
	// Use this for initialization
	
	// Update is called once per frame
	public void loadScene(int scene){
    	SceneManager.LoadScene(scene);
    }
    public void Continue(){
    	string name = getLastSaveName();
    	if (name == null) return;
    	SaveManager.Load(name);
    	Game.current = SaveManager.savedGames[SaveManager.savedGames.Count - 1];
		Game.current.mobCount = 1; 
    	SceneManager.LoadScene(Game.current.sceneIndex); 
    }
    public void Quit(){
    	if (Application.isEditor) UnityEditor.EditorApplication.isPlaying = false;
    	else Application.Quit();
    }

    public void loadAllSaveButtons(Transform parent, bool interactive){
    	int i = 0;
		foreach (Game g in SaveManager.savedGames){
			GameObject s = (GameObject) Instantiate(SaveTemplate);
			s.transform.parent = parent;
			s.transform.position = new Vector2(s.transform.parent.position.x,s.transform.parent.position.y)  + startPos;
			startPos.y -= displacement;
			s.transform.localScale = new Vector3(1,1,1);	
			s.transform.GetChild(0).GetComponent<Text>().text = g.fileName;
			s.transform.GetChild(1).GetComponent<Text>().text = g.dayDate + "/" + g.monthDate + "/" + g.yearDate;
			s.transform.GetChild(2).GetComponent<Text>().text = g.savedHour + ":" + g.savedMinute;
			s.transform.GetChild(3).GetComponent<Text>().text = "" + g.playerLevel;
			char[] delimiters = new char[] { '.', '/' };
		    string[] parsed = g.areaName.Split(delimiters);
			s.transform.GetChild(4).GetComponent<Text>().text = "" + parsed[parsed.Length - 2];
			StartCoroutine(loadImage("file:///"+ Application.persistentDataPath + "/" + g.fileName + ".png", s));
			int index = i;
			if(interactive) {
				s.GetComponent<Button>().onClick.AddListener(() => Load(index));
			}
			else s.GetComponent<Button>().enabled = false;
			i++;
		}
    }

    public void Load(int i){
    	Game.current = SaveManager.savedGames[i];
		Game.current.mobCount = 1; 
    	SceneManager.LoadScene(Game.current.sceneIndex); 
    }

    public string getLastSaveName(){
    	DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
		FileInfo[] info = dir.GetFiles("*.*");
		if (info.Length == 0) return null;
		FileInfo lastSave = info[0];
		foreach (FileInfo f in info){
			string filename = Path.GetFileName(f.Name);
			if(!filename.Contains(".png")){
				if (lastSave != null){
					if (lastSave.LastWriteTime < f.LastWriteTime){
						lastSave = f;
					}
				}
				else lastSave = f;
			}
		}
		return lastSave.FullName;
    }

    IEnumerator loadImage(string name, GameObject s){
    	WWW www = new WWW(name);
    	yield return www;
		Texture2D texture = new Texture2D(480, 300, TextureFormat.DXT1, false);  
		www.LoadImageIntoTexture(texture);
		Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 480, 300), new Vector2(0, 0), 100.0f);
		s.transform.GetChild(5).GetComponent<Image>().sprite = sprite;
    }
}
