using UnityEngine;
using System.Collections;


public delegate void MouseTargetCallback (GameObject gameObject);


public class MouseBehaviour : MonoBehaviour {

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

	GameObject currentTarget = null;
	GameObject lastTarget = null;

	InputEvent currentEvent = null;
	InputEvent lastEvent = null;


	// stores callbacks for when a new target is found
	event MouseTargetCallback onTargetEnter;

	// adds callbacks to onTargetEnter
	public static event MouseTargetCallback OnTargetEnter { add { instance.onTargetEnter += value; } remove { instance.onTargetEnter -= value; } }


	// stores callbacks for when a new target is found
	event MouseTargetCallback onTargetLeave;

	// adds callbacks to onTargetEnter
	public static event MouseTargetCallback OnTargetLeave { add { instance.onTargetLeave += value; } remove { instance.onTargetLeave -= value; } }


	/// <summary>
	/// Called by unity when script is loaded
	/// </summary>
	void Awake() {

		// make sure this is the only instance, error if not
		if (instance == null) { instance = this; }
		else { Debug.LogError ("Only 1 instance of MouseBehaviour can exist per scene!"); }
	}
		

	void UpdateTarget(GameObject hit) {

		if (hit != currentTarget) {
			
			if (hit == null) {

				// from last to null
				if (currentTarget != null) {

					currentTarget.SendMessageUpwards ("OnMouseLeave");
					if (onTargetLeave != null) { onTargetLeave (currentTarget); }

					lastTarget = currentTarget;
					currentTarget = null;
					if (onTargetEnter != null) { onTargetEnter (null); }
				}
			}
			else {
				
				if (hit != currentTarget) {

					// enter from last
					if (currentTarget != null) {
						
						currentTarget.SendMessageUpwards ("OnMouseLeave");
						if (onTargetLeave != null) { onTargetLeave (currentTarget); }

						lastTarget = currentTarget;
						currentTarget.SendMessageUpwards ("OnMouseEnter");
						if (onTargetEnter != null) { onTargetEnter (currentTarget); }
					}

					// enter from null
					else {
						
						lastTarget = null;
						currentTarget.SendMessageUpwards ("OnMouseEnter");
						if (onTargetEnter != null) { onTargetEnter (currentTarget); }
					}
				}
			}
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


		// not 'if/else' because currentEvent might be set in above 'if'
		if (currentEvent == null) {

			// check for events
		}
		if (currentEvent != null) {
			
			// check for event end
		}

	}
}

