using UnityEngine;
using System.Collections;


public delegate void MouseTargetCallback (GameObject gameObject);


public class InputManager : MonoBehaviour {

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

	/// <summary>
	/// singleton
	/// </summary>
	static InputManager instance = null;

	/// <summary>
	/// maximum mouse reach (in game units)
	/// </summary>
	[SerializeField] float maxDistance;

	/// <summary>
	/// only objects using a layer in this mask will be detected
	/// </summary>
	[SerializeField] LayerMask targetMask;


	/// <summary>
	/// The current mouse target.
	/// </summary>
	GameObject currentTarget = null;

	/// <summary>
	/// The last mouse target.
	/// </summary>
	GameObject lastTarget = null;


	/// <summary>
	/// The current mouse event.
	/// </summary>
	InputEvent currentEvent = null;

	/// <summary>
	/// The last mouse event.
	/// </summary>
	InputEvent lastEvent = null;


	/// <summary>
	/// stores callbacks for when a new target is found
	/// </summary>
	event MouseTargetCallback onTargetEnter;

	/// <summary>
	/// adds callbacks to onTargetEnter
	/// </summary>
	public static event MouseTargetCallback OnTargetEnter { add { instance.onTargetEnter += value; } remove { instance.onTargetEnter -= value; } }


	/// <summary>
	/// stores callbacks for when a new target is found
	/// </summary>
	event MouseTargetCallback onTargetLeave;

	/// <summary>
	/// adds callbacks to onTargetEnter
	/// </summary>
	public static event MouseTargetCallback OnTargetLeave { add { instance.onTargetLeave += value; } remove { instance.onTargetLeave -= value; } }


	/// <summary>
	/// Called by unity when script is loaded
	/// </summary>
	void Awake() {

		// make sure this is the only instance, error if not
		if (instance == null) { instance = this; }
		else { Debug.LogError ("Only 1 instance of MouseBehaviour can exist per scene!"); }
	}
		
	/// <summary>
	/// Updates the current target (if different).
	/// </summary>
	/// <param name="hit">the game object hit from the most recent raycast</param>
	void UpdateTarget(GameObject hit) {

		// if the target has changed
		if (hit != currentTarget) {

			// if no valid object was hit with raycast
			if (hit == null) {

				// leave current target
				currentTarget.SendMessageUpwards ("OnMouseLeave");
				if (onTargetLeave != null) { onTargetLeave (currentTarget); }

				// set new target
				lastTarget = currentTarget;

				// enter new target
				currentTarget = null;
				if (onTargetEnter != null) { onTargetEnter (null); }
			}

			// if a valid object was hit with raycast
			else {
				
				// enter from last
				if (currentTarget != null) {

					// leave current target
					currentTarget.SendMessageUpwards ("OnMouseLeave");
					if (onTargetLeave != null) { onTargetLeave (currentTarget); }

					// set new target
					lastTarget = currentTarget;

					// enter new target
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


	/// <summary>
	/// Called by unity every frame
	/// </summary>
	void Update() {

		// define a raycast hit
		RaycastHit hit;

		// see if the mouse if hovering over a valid object within max distance
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit, maxDistance, targetMask)) { 

			// update target with object
			UpdateTarget (hit.collider.gameObject);
		}

		// no valid object found
		else {

			// update target with null
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

