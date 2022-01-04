using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeTile : Tile
{
	public bool isExtended;
	public Material extendedMat;
	public Material retractedMat;

	private void Awake() {
		SetExtended(isExtended);
	}

	public void ToggleExtended() {
		SetExtended(!isExtended);
	}

	public void SetExtended(bool state) {
		isExtended = state;
		if (isExtended) {
			foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
				rend.material = extendedMat;
			}
		} else {
			foreach (Renderer rend in GetComponentsInChildren<Renderer>()) {
				rend.material = retractedMat;
			}
		}
	}

	public override bool OnActorPresent(ActorController actor) {
		Debug.Log("Processing actor present on bridge");
		if (!isExtended) {
			actor.OnKill();
		}
		return false;
	}

	public override bool TryEnter(int fromDir) {
		return isExtended;
	}
}
