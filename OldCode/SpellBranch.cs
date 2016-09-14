using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellBranch : MonoBehaviour {

	public Branch branch;
	List<Point> nextPoints;

	public void Start(){
		nextPoints = new List<Point>();
	}

	public void updateBranch(){
		for(int i = 0; i < branch.currentPoints.Count; i++){
			Point p = branch.currentPoints[i];
			print("		branch");
			bool shouldMove = p.getGameObject().GetComponent<SpellPoint>().updatePoint();
			if(shouldMove){
				nextPoints.Remove(p);
				foreach(Point child in p.children){
					nextPoints.Add(child);
				}
			}
		}
		branch.currentPoints = nextPoints;
	}
}
