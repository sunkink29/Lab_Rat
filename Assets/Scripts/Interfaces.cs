using UnityEngine;
using System.Collections;

public interface Interactible {

	void interact();

}

public interface IActivatible {

	void activate(string condition);

}

public interface ElevatorComponent {
	
	int currentFloor {
		get;
		set;
	}
}

public interface Moveable {

}