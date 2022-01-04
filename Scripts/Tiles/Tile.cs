using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //parent Tile class
    //all empty grid slots will contain these
    //should be impossible to enter
    //actually useful tiles will derive from this

    /*
     * Method - runs after all actors have completed their moves
     * Params - actor: ActorController present on the tile
     * Returns - bool: whether or not a reprocess is required. example, if one of the actors is on a button, 
     * the bridge tiles need to be preprocessed in case one of them just got deactivated and happens to have an actor on it
     */
    public virtual bool OnActorPresent(ActorController actor) {
        Debug.LogError("Warning: actor has somehow entered an empty tile");
        actor.OnKill();
        return false;
	}
    
    public virtual bool TryEnter(int fromDir) {
        return false;
	}

    public virtual int TryExit(int toDir) {
        return toDir;
	}

    public bool CanBeEntered() {
        for (int i = 0; i < 4; i++) {
            if (this.TryEnter(i)) {
                return true;
			}
		}
        return false;
	}

}
