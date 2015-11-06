using UnityEngine;
using System.Collections;

public class ElevatorWallController : MonoBehaviour, IActivatible, ElevatorComponent {

	EleveatorController elevatorReference;
	public Animator elevatorWallAnimator;
	[SerializeField] private int CurrentFloor;
	public int currentFloor {
		get{
			return CurrentFloor;
		}
		set{
			CurrentFloor = value;
		}
	}
	private bool isOpen;
	public bool IsOpen {
		get {
			return isOpen;
		}
		set {
			isOpen = value;
			if (isOpen)
			elevatorWallAnimator.SetTrigger("Open Elevator");
			else
			elevatorWallAnimator.SetTrigger("Close Elevator");
		}
	}
	
	// Use this for initialization
	void Start () {
		elevatorWallAnimator = GetComponentInParent<Animator>() ;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void getElevatorComponents(EleveatorController returnObject) {
		elevatorReference = returnObject;
		elevatorReference.subscribeAsElevatorWall (this);
	}
	
	public void activate(string condition) {
		elevatorReference.activated (condition, this);
	}
}
