using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour {
	public List<ActorController> allActors;
	private ActorController focusedActor;
	public int defaultFocus;
	public LevelManager levelManager;

	private bool controlsLocked = true;
	private int callbackCounter;

	public void Start() {
		levelManager = GameObject.FindObjectOfType<LevelManager>();
	}

	// Update is called once per frame
	void Update() {
		if (!controlsLocked) {
			if (Input.GetButtonDown("Up")) {
				ExportMotion(0);
			}
			if (Input.GetButtonDown("Down")) {
				ExportMotion(1);
			}
			if (Input.GetButtonDown("Left")) {
				ExportMotion(2);
			}
			if (Input.GetButtonDown("Right")) {
				ExportMotion(3);
			}

			if (Input.GetButtonDown("Slot1")) {
				ExportFocus(1);
			}
			if (Input.GetButtonDown("Slot2")) {
				ExportFocus(2);
			}
			if (Input.GetButtonDown("Slot3")) {
				ExportFocus(3);
			}

			if (Input.GetButtonDown("Reset")) {
				OnReset();
			}
			if (Input.GetButtonDown("Undo")) {
				OnUndo();
			}
		}
	}

	public void SetControlsLocked(bool state) {
		controlsLocked = state;
	}

	public void SetAllTooltipsActive(bool state) {
		foreach (ActorController actor in allActors) {
			actor.SetTooltipActive(state);
		}
	}

	public void AttachActors(ActorController[] actors, int focus) {
		allActors.Clear();
		//fill the allActors list with all of the actors in the level IN ORDER of focusNum
		for (int i = 0; i < actors.Length; i++) {
			foreach (ActorController act in actors) {
				if (act.focusNum == i + 1) {
					allActors.Add(act);
					act.AttachInputHandler(this);
				}
			}
		}
		ExportFocus(focus);
	}

	private void OnReset() {
		//SceneManager.LoadScene(0);
		levelManager.ResetLevel();
	}

	private void OnUndo() {

	}

	public void ExportFocus(int num) {
		if (num <= allActors.Count && allActors[num-1].isAlive) {
			foreach (ActorController actor in allActors) {
				if (actor.ShiftFocus(num)) {
					focusedActor = actor;
				}
			}
		}
	}

	public void ExportMotion(int num) {
		//some movement key has been pressed
		SetAllTooltipsActive(false);
		callbackCounter = 0;
		foreach (ActorController actor in allActors) {
			if (actor.isAlive) {
				callbackCounter++;
			}
		}

		foreach (ActorController actor in allActors) {
			actor.Move(num);
		}
	}

	public void ImportCompletedMotion() {
		callbackCounter--;
		Debug.Log(callbackCounter);
		if (callbackCounter == 0) {
			ProcessMapState();
		}
	}

	private void ProcessMapState() {
		Debug.Log("Processing map state");
		bool reprocess = true;
		while (reprocess) {
			reprocess = false;
			foreach (ActorController actor in allActors) {
				if (actor.ProcessMapState()) {
					reprocess = true;
				}
			}
		}
		CheckVictoryState();
	}

	private void CheckVictoryState() {
		bool winFlag = true;
		foreach (ActorController actor in allActors) {
			if (!actor.isAlive) {
				if (!actor.isWin) {
					//if actor isn't alive and isn't win, it must have lost
					levelManager.ResetLevel();
				}
			} else {
				//actor is still alive, win hasn't happened yet
				winFlag = false;
			}
		}
		if (winFlag) {
			levelManager.IncrementLevel();
		}
	}

}
