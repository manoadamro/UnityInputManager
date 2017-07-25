using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	// singleton
	static InputManager instance = null;

	/// <summary>
	/// Called by unity when script is loaded
	/// </summary>
	void Awake() {

		// make sure this is the only instance, error if not
		if (instance == null) { instance = this; }
		else { Debug.LogError ("Only 1 instance of InputManager can exist per scene!"); }
	}

}
