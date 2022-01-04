using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalTile : Tile
{
	public int direction; //TODO globally convert this to an enum

	public override bool OnActorPresent(ActorController actor) {
		//weirdly enough, I don't think I actually need to do anything here
		//the actors will always try to move and when they do they'll get punted off this tile as intended
		return false;
	}

	public override bool TryEnter(int fromDir) {
		return fromDir != direction;
	}

	public override int TryExit(int toDir) {
		//actor can ONLY leave in this tile's direction
		return direction;
	}
}
