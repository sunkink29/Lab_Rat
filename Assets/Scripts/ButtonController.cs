using UnityEngine;
using System;
using System.Collections;


public class ButtonController : MonoBehaviour , Interactible {
	Animator buttonAnimator;
	[SerializeField] string condition;
	GameObject attachedObject;
	
	void Start () {
		RaycastHit hitInfo;
		buttonAnimator = GetComponent<Animator> ();
		Physics.Raycast (gameObject.transform.position, gameObject.transform.up * -1, out hitInfo);
		attachedObject = hitInfo.transform.gameObject;
	}
	
	void Update () {
	
	}

	public void interact() {
		attachedObject.SendMessage ("activate", condition);
		buttonAnimator.SetTrigger ("Button pressed");
	}

}
