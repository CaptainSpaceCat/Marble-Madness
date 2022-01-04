using UnityEngine;

public class MapNode {
	public MapNode up;
	public MapNode down;
	public MapNode left;
	public MapNode right;

	public bool isKillTile;
	public bool isWinTile;

	public Vector3 position;

	public int distanceToKill = 99999;

	public MapNode(Vector3 pos, bool kill, bool win) {
		position = pos;
		isKillTile = kill;
		isWinTile = win;
	}

	public MapNode TryMove(int num) {
		if (num == 0) {
			return up;
		}
		if (num == 1) {
			return down;
		}
		if (num == 2) {
			return left;
		}
		if (num == 3) {
			return right;
		}
		return null;
	}
}
