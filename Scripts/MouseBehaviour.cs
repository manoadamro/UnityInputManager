using UnityEngine;
using System.Collections;

public class MouseBehaviour : MonoBehaviour {


	class MouseTarget {

		public GameObject gameObject;
	}

	class InputEvent {
		
		public KeyCode key;
		public float startTime;
		public float endTime;
		public bool shift;
		public bool alt;
		public bool control;
		public bool doubleClick;
		public bool hold;
		public bool drag;

		public InputEvent (KeyCode key, bool shift = false, bool alt = false, bool control = false, bool doubleClick = false) {
			
			this.key = key;
			this.startTime = Time.time;
			this.endTime = -1;
			this.shift = shift;
			this.alt = alt;
			this.control = control;
			this.doubleClick = doubleClick;
			this.hold = false;
			this.drag = false;
		}
	}

	// singleton
	static MouseBehaviour instance = null;

	[SerializeField] float maxDistance;
	[SerializeField] LayerMask targetMask;

	MouseTarget currentTarget = null;
	MouseTarget lastTarget = null;

	InputEvent currentEvent = null;
	InputEvent lastEvent = null;


	/// <summary>
	/// Called by unity when script is loaded
	/// </summary>
	void Awake() {

		// make sure this is the only instance, error if not
		if (instance == null) { instance = this; }
		else { Debug.LogError ("Only 1 instance of MouseBehaviour can exist per scene!"); }
	}
		

	void UpdateTarget(GameObject hit) {

		if (hit == null) {

			if (currentTarget != null) {
				
				// leave current target
				// target is null
			}

			// else stays null
		}
		else {
			
			if (hit != currentTarget.gameObject) {

				// leave current target
				// set new target as current target
				// enter current target
			}

			// else target stays the same
		}
	}

	void Update() {

		RaycastHit hit;
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, maxDistance, targetMask)) { 
			UpdateTarget (hit.collider.gameObject);
		}
		else {
			UpdateTarget (null);
		}

		if (currentEvent == null) {

			// check for events
		}
		else {

			// check for event end
		}
	}
}

