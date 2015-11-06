using UnityEngine;
using System.Collections;

public class EleveatorController: MonoBehaviour {

	public EleveatorRoomController elevatorRoomController;
	public ElevatorWallController[] elevatorWalls = new ElevatorWallController[10];
	public int amountOfFloors;

	void Awake () {
		BroadcastMessage ("getElevatorComponents", this);
		//elevatorRoomController.getElevatorReference (animator);
	}

	public void Start() {
		for (int i = 1; elevatorWalls[i] != null; i++) {
			if (elevatorWalls[i].currentFloor == elevatorRoomController.currentFloor) {
				elevatorWalls[i].IsOpen = elevatorRoomController.startOpen;
			} else {
				elevatorWalls[i].IsOpen = !elevatorRoomController.startOpen;
			}
		}
	}

	public void subscribeAsElevatorWall(ElevatorWallController elevatorWall) {

		int i = 1;
		int j;
		bool foundPlace = false;

		amountOfFloors++;

		while (!foundPlace) {
			if (elevatorWalls [i] == null) {
				elevatorWalls [i] = elevatorWall;
				foundPlace = true;
			} else 
				i++;
		}

		j = i;

		if (i != 1) {
			elevatorWalls [0] = elevatorWalls [i];
			i = 0;
		}

		while (j != 0 && elevatorWalls[j-1] != null) {
			if (elevatorWalls [j - 1].transform.position.y > elevatorWalls [i].transform.position.y) {
				elevatorWalls [j] = elevatorWalls [j - 1];
				j--;
			} else {
				elevatorWalls [j] = elevatorWalls [i];
				j = 0;
			}
		}

		j = 0;

		while (elevatorWalls[j] != null) {
			elevatorWalls [j].currentFloor = j;
			j++;
		}

		ElevatorWallController closestToRoom = elevatorWalls [1];
		for (int k = 1; elevatorWalls[k]!=null; k++) {
			if (Mathf.Abs(elevatorRoomController.transform.position.y - elevatorWalls [k].transform.position.y) <= Mathf.Abs(elevatorRoomController.transform.position.y - closestToRoom.transform.position.y)) {
				closestToRoom = elevatorWalls [k];
			}


		}
		elevatorRoomController.currentFloor = closestToRoom.currentFloor;
	}

	 public void subscribeAsElevatorRoom(EleveatorRoomController elevatorRoom){
		elevatorRoomController = elevatorRoom;
	}
	
	public void activated(string condition,ElevatorComponent activatedObject) {
		int num;
		if (activatedObject.GetType () == typeof(ElevatorWallController)) {
			goToFloor(activatedObject.currentFloor);
		}			
		if (activatedObject.GetType () == typeof(EleveatorRoomController)) {
			if (condition != null) {
				if (int.TryParse(condition,out num)){
					goToFloor (num);
				} else {
					if (condition == "up"){
						goToFloor(elevatorRoomController.currentFloor + 1);
					}else if (condition == "down") {
						goToFloor(elevatorRoomController.currentFloor - 1);
					}
				}
			} else if (!elevatorRoomController.isMoving) {

			}
		}
	}

	void goToFloor(int floor) {
		if (floor < 1 || floor > amountOfFloors)
			toggleCurrentDoor ();
		else
		elevatorRoomController.StartCoroutine (elevatorRoomController.goToFloor(floor));
	}

	public void toggleCurrentDoor(){
		elevatorWalls [elevatorRoomController.currentFloor].IsOpen = !elevatorRoomController.isOpen;
		elevatorRoomController.isOpen = !elevatorRoomController.isOpen;
	}
}
