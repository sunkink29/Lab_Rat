using UnityEngine;
using System.Collections;

public class EleveatorRoomController : MonoBehaviour , IActivatible, ElevatorComponent {

	public EleveatorController elevatorReference;
	public Collider[] objectsInElevator = new Collider[10];
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
		} else{
			int num = 1;
			bool foundEmtyspot = false;
			while (!foundEmtyspot) {
				if (objectsInElevator [num] == null) {
					foundEmtyspot = true;
					objectsInElevator [num] = other;
				} else {
					num++;
				}
			}
		}

	}
	void OnTriggerExit(Collider other) {
		ElevatorWallController currentElevatorFloor = other.transform.gameObject.GetComponent<ElevatorWallController> ();
		if (currentElevatorFloor != null && elevatorSpeed < 0) {
			currentFloor = currentElevatorFloor.currentFloor;
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
			elevatorSpeed = elevatorSpeed * -1;
		while (currentFloor != floor) {
			currentState = elevatorAnimator.GetCurrentAnimatorStateInfo(elevatorAnimator.GetLayerIndex("Base Layer"));
			if (currentState.IsName("closedWallAnimation")){
				transform.parent.Translate (new Vector3 (0, (elevatorSpeed * Time.deltaTime), 0));
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