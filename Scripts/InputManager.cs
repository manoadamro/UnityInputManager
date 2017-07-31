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
		public Vector2 startPoint;

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


	/* <--- call messages --->

	onCLick
	onDoubleClick

	onHoldStart
	onHold
	onHoldEnd

	onDragStart
	onDrag
	onDragEnd
	*/


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
	/// Checks for mouse event.
	/// </summary>
	bool CheckForMouseEvent() {
		// check for mouse button events
		for (int i = 0; i < 3; i++) {

			// see if mouse button has been clicked this frame
			if (Input.GetMouseButtonDown (i)) {

				// get the keycode from mouse button index
				KeyCode code = (KeyCode)((int)KeyCode.Mouse0 + i);

				// create an event
				currentEvent =
					new InputEvent (
						code,
						Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift),
						Input.GetKeyDown (KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt),
						Input.GetKeyDown (KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl),
						lastEvent != null && lastEvent.key == code && Time.time - lastEvent.endTime < 0.2f
					);

				// last event is not needed now
				lastEvent = null;

				// no more looking!
				return true;
			}
		}

		// no events
		return false;
	}


	/// <summary>
	///
	/// </summary>
	void CheckWatchKeys() {

		// To Do

	}


	/// <summary>
	///
	/// </summary>
	bool CheckForEventEnd() {

		if (Input.GetKeyUp(currentEvent.key)) {


			// Call event end
			if (!currentEvent.hold && !currentEvent.drag) {

				if (currentEvent.doubleClick) {

					// <--- end double click --->
				}
				else {

					// <--- end click --->
				}
			}
			else {

				if (currentEvent.hold) {

					// <--- end hold --->
				}

				// this could just be else, but... 'else if' is pretty
				else if (currentEvent.drag) {

					// <--- end drag --->
				}
			}

			// set last event
			lastEvent = currentEvent;

			// clear current event
			currentEvent = null;

			// event was ended
			return true;
		}

		// event was not ended
		return false;
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

		// if there is no current event
		if (currentEvent == null) {

			// check for mouse events
			if (CheckForMouseEvent ()) { return; }

			// Check watch keys
			CheckWatchKeys();
		}

		// if there is a current event
		else {

			// check for event end
			if (!CheckForEventEnd()) {

				// if there 'hold' is false but the key has been held longer than threshold
				if (!currentEvent.hold && Time.time - currentEvent.startTime > 0.2f) {

					// hold is not true
					currentEvent.hold = true;

					// <--- begin hold --->
				}

				// if there 'drag' is false but the cursor has moved more than the threshold
				if (!currentEvent.drag && Vector2.Distance (currentEvent.startPoint, Input.MousePosition) > 2f) {

					// drag is not true
					currentEvent.drag = true;

					// <--- begin drag --->
				}

			}
			else {

				if (currentEvent.hold) {

					// <--- hold stay --->
				}

				if (currentEvent.drag) {

					// <--- drag stay --->
				}
			}
		}
	}
}
