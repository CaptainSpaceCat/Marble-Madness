using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonTile : Tile
{
	public bool isPressed = false;

	//special bool used to ensure the button is only toggled the first timestep it's entered
	//without this bool, the button would toggle every timestep while an actor is sitting on it
	private bool primed = false;

	[System.Serializable]
	public class OnToggle : UnityEvent { }

	[SerializeField]
	private OnToggle _onToggle = new OnToggle();
	public OnToggle onToggle { get { return _onToggle; } set { _onToggle = value; } }

	public void OnButtonToggled() {
		isPressed = !isPressed;
		onToggle.Invoke();
	}

	public override bool OnActorPresent(ActorController actor) {
		if (primed) {
			primed = false;
			OnButtonToggled();
			return true;
		}
		return false;
	}

	public override bool TryEnter(int fromDir) {
		primed = true;
		return true;
	}
}
