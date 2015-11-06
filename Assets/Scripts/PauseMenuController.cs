using UnityEngine;
using System.Collections;

public class PauseMenuController : MonoBehaviour
{

	bool isPaused = false;
	GameObject[] PauseMenuGameObjects;
	GameObject player;
	FirstPersonController playerController;
	GameObject dynamicObjects;
	FreezeGameObjects freezeGameObjects;

	void Start () 
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		playerController = player.GetComponent<FirstPersonController> ();
		dynamicObjects = GameObject.FindWithTag ("Dynamic");
		freezeGameObjects = dynamicObjects.GetComponent<FreezeGameObjects> ();
		PauseMenuGameObjects = GameObject.FindGameObjectsWithTag ("Pause Menu");
		setIsPaused (false);
	}
	
	void Update () 
	{
		if (Input.GetButtonDown("Pause")) 
		{
			setIsPaused(!isPaused);
		}
	}

	public void setIsPaused(bool pauseState)
	{
		isPaused = pauseState;
		playerController.setIsPaused (isPaused);
		freezeGameObjects.freezeObjects (isPaused);
		Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = isPaused;
		setVisibility (isPaused);
		
		//		set move to set the velocity
	}

	void ReturnToGame()
	{
		setIsPaused (false);
	}

	public bool getIsPaused()
	{
		return isPaused;
	}

	void quit ()
	{
	//	Debug.Log ("quit");
		Application.Quit ();
	}

	void setVisibility(bool visibility)
	{
		foreach (GameObject element in PauseMenuGameObjects) 
		{
			element.SetActive(false);
		}
	}
}
