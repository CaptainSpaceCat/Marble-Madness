using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MapData : MonoBehaviour {

	private ActorController attachedActor;
	private Tile[,] gridMap;
	private Vector2Int actorPos;

	// Start is called before the first frame update
	void Start() {
		Tile[] allTiles = GetComponentsInChildren<Tile>();
		attachedActor = GetComponentInChildren<ActorController>();
		ConstructGridMap(allTiles, attachedActor);
	}

	private void ConstructGridMap(Tile[] allTiles, ActorController actor) {
		Vector2Int[] bounds = GetBounds(allTiles);
		gridMap = new Tile[Mathf.RoundToInt(bounds[1].x - bounds[0].x)+1,Mathf.RoundToInt(bounds[1].y - bounds[0].y)+1];

		//initialize gridMap to contain all empty (non-enterable) tiles
		for (int x = 0; x < gridMap.GetLength(0); x++) {
			for (int y = 0; y < gridMap.GetLength(1); y++) {
				gridMap[x, y] = new Tile();
			}
		}

		//fill in the actual tiles into the gridMap
		foreach (Tile t in allTiles) {
			int x = Mathf.RoundToInt(t.transform.localPosition.x - bounds[0].x);
			int y = Mathf.RoundToInt(t.transform.localPosition.z - bounds[0].y);
			gridMap[x, y] = t;
		}

		//fill out the actor's location in the gridMap
		int x2 = Mathf.RoundToInt(actor.transform.localPosition.x - bounds[0].x);
		int y2 = Mathf.RoundToInt(actor.transform.localPosition.z - bounds[0].y);
		actorPos = new Vector2Int(x2, y2);
		Debug.Log("Actor at " + actorPos);
		actor.AttachToGridMap(actorPos, this);
	}

	private Vector2Int[] GetBounds(Tile[] allTiles) {
		int minX = 999999, minY = 999999;
		int maxX = -999999, maxY = -999999;
		foreach (Tile t in allTiles) {
			Vector3 pos = t.transform.localPosition;
			if (pos.x < minX) {
				minX = Mathf.RoundToInt(pos.x);
			}
			if (pos.z < minY) {
				minY = Mathf.RoundToInt(pos.z);
			}
			if (pos.x > maxX) {
				maxX = Mathf.RoundToInt(pos.x);
			}
			if (pos.z > maxY) {
				maxY = Mathf.RoundToInt(pos.z);
			}
		}
		return new Vector2Int[] { new Vector2Int(minX, minY), new Vector2Int(maxX, maxY) };
	}

	public Tile GetTile(Vector2Int pos) {
		if (InBounds(pos)) {
			return gridMap[pos.x, pos.y];
		}
		return null;
	}

	private Vector2Int[] refTab = {
		new Vector2Int(0, 1),
		new Vector2Int(0, -1),
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
	};
	public Vector2Int GetMotion(Vector2Int pos, int dir) {
		if (dir == -1) {
			return pos;
		}
		return pos + refTab[dir];
	}

	public Vector2Int GetMotion(int x, int y, int dir) {
		return new Vector2Int(x, y) + refTab[dir];
	}

	public bool InBounds(Vector2Int pos) {
		return pos.x >= 0 && pos.y >= 0 && pos.x < gridMap.GetLength(0) && pos.y < gridMap.GetLength(1);
	}

	public Vector2Int GetClosestKillTile(Vector2Int pos) {
		bool[,] fringe = new bool[gridMap.GetLength(0), gridMap.GetLength(1)];
		bool[,] fill = new bool[gridMap.GetLength(0), gridMap.GetLength(1)];
		bool[,] blacklist = new bool[gridMap.GetLength(0), gridMap.GetLength(1)];
		fringe[pos.x, pos.y] = true;
		while (true) {
			bool doneFlag = true;
			for (int x = 0; x < fringe.GetLength(0); x++) {
				for (int y = 0; y < fringe.GetLength(1); y++) {
					blacklist[x, y] = false;
				}
			}
			for (int x = 0; x < fringe.GetLength(0); x++) {
				for (int y = 0; y < fringe.GetLength(1); y++) {
					if (GetTile(new Vector2Int(x, y)).CanBeEntered() && fringe[x,y] && !fill[x,y] && !blacklist[x,y]) {
						doneFlag = false;
						fill[x, y] = true;
						for (int d = 0; d < 4; d++) {
							Vector2Int newPos = GetMotion(x, y, d);
							if (InBounds(newPos)) {
								blacklist[newPos.x, newPos.y] = true;
								Tile newTile = GetTile(newPos);
								if (newTile.TryEnter(d)) {
									fringe[newPos.x, newPos.y] = true;
								}
								if (newTile.GetType() == typeof(KillTile)) {
									return newPos;
								}
							}
						}
					}
				}
			}
			if (doneFlag) {
				break;
			}
		}
		return Vector2Int.one * -1; //TODO find a better way to return no valid goal
	}

	public int InverseDir(int dir) {
		if (dir == 0) {
			return 1;
		} else if (dir == 1) {
			return 0;
		} else if (dir == 2) {
			return 3;
		} else {
			return 2;
		}
	}

	public int NextStepToGoal(Vector2Int start, Vector2Int goal) {
		int[,] fill = new int[gridMap.GetLength(0), gridMap.GetLength(1)];
		bool[,] blacklist = new bool[gridMap.GetLength(0), gridMap.GetLength(1)];
		for (int x = 0; x < fill.GetLength(0); x++) {
			for (int y = 0; y < fill.GetLength(1); y++) {
				fill[x, y] = -1;
			}
		}
		fill[goal.x, goal.y] = 0;
		while (true) {
			bool doneFlag = true;
			for (int x = 0; x < fill.GetLength(0); x++) {
				for (int y = 0; y < fill.GetLength(1); y++) {
					blacklist[x, y] = false;
				}
			}
			for (int x = 0; x < fill.GetLength(0); x++) {
				for (int y = 0; y < fill.GetLength(1); y++) {
					if (GetTile(new Vector2Int(x, y)).CanBeEntered()) {
						if (fill[x, y] < 0) {
							for (int d = 0; d < 4; d++) {
								Vector2Int adjacent = GetMotion(x, y, d);
								if (InBounds(adjacent) && !blacklist[adjacent.x, adjacent.y] && fill[adjacent.x, adjacent.y] > -1 && GetTile(adjacent).TryEnter(InverseDir(d))) {
									if (fill[x, y] < 0 || fill[x, y] > fill[adjacent.x, adjacent.y] + 1) {
										doneFlag = false;
										fill[x, y] = fill[adjacent.x, adjacent.y] + 1;
										blacklist[x, y] = true;
									}
								}
							}
						}
						if (x == start.x && y == start.y && fill[x, y] >= 0) {
							return NextStep(x, y, fill);
						}
					}
				}
			}
			if (doneFlag) {
				break;
			}
		}
		
		return -1;
	}

	private int NextStep(int x, int y, int[,] fill) {
		int minDir = -1;
		int minVal = 9999;
		for (int n = 0; n < 4; n++) {
			Vector2Int adjacent = GetMotion(x, y, n);
			if (InBounds(adjacent)) {
				if (fill[adjacent.x, adjacent.y] >= 0 && fill[adjacent.x, adjacent.y] < minVal) {
					minVal = fill[adjacent.x, adjacent.y];
					minDir = n;
				}
			}
		}
		return minDir;
	}

}
