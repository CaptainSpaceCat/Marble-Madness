using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTile : Tile
{
    public override bool OnActorPresent(ActorController actor) {
		actor.OnWin();
		return false;
	}

	public override bool TryEnter(int fromDir) {
		return true;
	}
}
