using UnityEngine;
using System.Collections;

public class MouseBehaviour : MonoBehaviour {

	// singleton
	static MouseBehaviour instance = null;

	/// <summary>
	/// Called by unity when script is loaded
	/// </summary>
	void Awake() {

		// make sure this is the only instance, error if not
		if (instance == null) { instance = this; }
		else { Debug.LogError ("Only 1 instance of MouseBehaviour can exist per scene!"); }
	}

	void Update() {

	}
}

