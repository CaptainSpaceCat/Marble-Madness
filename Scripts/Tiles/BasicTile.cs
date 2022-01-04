using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTile : Tile
{
	public override bool OnActorPresent(ActorController actor) { return false; }

	public override bool TryEnter(int fromDir) { return true; }

}
