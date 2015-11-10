using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class EleveatorRoomController : MonoBehaviour , IActivatible, ElevatorComponent {

	public EleveatorController elevatorReference;
	public MovableObject[] objectsInElevator = new MovableObject[10];
	Animator elevatorAnimator;
	public float elevatorSpeed = 1;
	[SerializeField] private int CurrentFloor;
	public int currentFloor {
		get{
			return CurrentFloor;
		}
		set{
			CurrentFloor = value;
		}
	}
	public bool isMoving;
	public bool startOpen = true;
	private bool IsOpen;
	public bool isOpen {
		get {
			return IsOpen;
		}
		set {
			IsOpen = value;
			if (IsOpen)
				elevatorAnimator.SetTrigger("Open Elevator");
			else
				elevatorAnimator.SetTrigger("Close Elevator");
		}
	}



	// Use this for initialization
	void Start () {
		elevatorAnimator = GetComponentInParent<Animator> ();
		isOpen = startOpen;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		ElevatorWallController currentElevatorFloor = other.transform.gameObject.GetComponent<ElevatorWallController>();
		if (currentElevatorFloor != null && elevatorSpeed > 0) {
			currentFloor = currentElevatorFloor.currentFloor;
		} else {
			MovableObject moveableObject = other.transform.gameObject.GetComponent<MovableObject> ();
			if (moveableObject != null && !objectsInElevator.Contains(moveableObject)) {
				int num = 0;
				bool foundEmtyspot = false;
				while (!foundEmtyspot) {
					if (objectsInElevator [num] == null) {
						foundEmtyspot = true;
						objectsInElevator [num] = moveableObject;
					} else if (num >= objectsInElevator.Length) {
						foundEmtyspot = true;
					} else {
						num++;
					}
				}
			}
		}

	}
	void OnTriggerExit(Collider other) {
		ElevatorWallController currentElevatorFloor = other.transform.gameObject.GetComponent<ElevatorWallController> ();
		if (currentElevatorFloor != null && elevatorSpeed < 0) {
			currentFloor = currentElevatorFloor.currentFloor;
		} else {
			MovableObject movableOject = other.transform.gameObject.GetComponent<MovableObject> ();
			if (movableOject != null) {
				objectsInElevator[Array.IndexOf(objectsInElevator,movableOject)] = null;
			}
		}
	}

	public void getElevatorReference(Animator reference) {
		elevatorAnimator = reference;
	}
	
	public void activate(string condition) {
		elevatorReference.activated (condition, this);
	}

	public IEnumerator goToFloor(int floor) {
		AnimatorStateInfo currentState;
		elevatorReference.toggleCurrentDoor ();
		isMoving = true;
		if (currentFloor > floor)
			elevatorSpeed = -1;
		else
			elevatorSpeed = 1;
		while (currentFloor != floor) {
			currentState = elevatorAnimator.GetCurrentAnimatorStateInfo(elevatorAnimator.GetLayerIndex("Base Layer"));
			if (currentState.IsName("closedWallAnimation")){
				transform.parent.Translate (new Vector3 (0, (elevatorSpeed * Time.deltaTime), 0));
				for (int elevatorObject = 0; elevatorObject < objectsInElevator.Length; elevatorObject++) {
					if (objectsInElevator[elevatorObject] != null){
						objectsInElevator[elevatorObject].transform.Translate(new Vector3(0, 0, elevatorSpeed * Time.deltaTime));
					}
				}
			}
				yield return null;
		}
		elevatorReference.toggleCurrentDoor ();
	}

	void getElevatorComponents(EleveatorController elevator) {
		elevatorReference = elevator;
		elevatorReference.subscribeAsElevatorRoom (this);
	}







}