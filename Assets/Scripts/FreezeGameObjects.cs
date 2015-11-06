using UnityEngine;
using System.Collections;

public class FreezeGameObjects : MonoBehaviour {

	PauseMenuController pauseMenu;
	bool isPaused; //pause state of all game
	//temporary arrays for rigidbody parameters
	Rigidbody[] rigids;
	Vector3[] velocity; 
	Vector3[] angularVelocity;
	bool[] isKinematic;

	void start()
	{
		pauseMenu = GetComponent<PauseMenuController> ();
		isPaused = pauseMenu.getIsPaused ();
	}
	
	public void freezeObjects(bool paused){
		if (paused != isPaused) {
			if (paused) { // save forces
				rigids = gameObject.GetComponentsInChildren<Rigidbody>();//for all childs of this object
				velocity         = new Vector3[rigids.Length];
				angularVelocity = new Vector3[rigids.Length];
				isKinematic    = new bool[rigids.Length];
				for(int i = 0; i < rigids.Length; i ++) {
					if (rigids[i] != null) {
						velocity[i] = rigids[i].velocity;
						angularVelocity[i] = rigids[i].angularVelocity;
						isKinematic[i] = rigids[i].isKinematic;
						rigids[i].isKinematic = true;
					}
				}
			} else { //restore forces
				for(int i = 0; i < rigids.Length; i ++) {
					if (rigids[i] != null) {
						rigids[i].isKinematic = isKinematic[i];
						if (!isKinematic[i]) { //restore velocity only for non kinematic objects
							rigids[i].velocity = velocity[i];
							rigids[i].angularVelocity = angularVelocity[i];
						}
					}
				}
				velocity         = null;
				angularVelocity = null;
				isKinematic    = null;
			}
		}
		isPaused = paused;
	}
}