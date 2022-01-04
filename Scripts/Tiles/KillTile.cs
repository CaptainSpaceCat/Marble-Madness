using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTile : Tile
{
	public override bool OnActorPresent(ActorController actor) {
		actor.OnKill();
		return false;
	}

	public override bool TryEnter(int fromDir) {
		return true;
	}
}
