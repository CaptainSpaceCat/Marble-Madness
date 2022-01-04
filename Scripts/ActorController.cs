using System.Collections;
using UnityEngine;

public class ActorController : MonoBehaviour {
	public int focusNum;
	public bool inFocus = false;
	public bool isAlive = true;
	public bool isWin = false;

	public TextPopup focusLabel;

	public Material playerMat;
	public Material enemyMat;
	public Material winMat;
	public Material killMat;

	public MapNode currentMapNode;
	public MapData mapData;
	public Tile currentTile;

	private Vector2Int currentPos;

	private InputHandler inputHandler;
	 
	private void Awake() {
		if (focusNum == 0) {
			Debug.LogError("Warning: focusNum not set");
		}
	}

	public void Move(int num) {
		if (isAlive) {
			if (inFocus) {
				PlayerMove(num);
			} else {
				EnemyMove();
			}
		}
	}

	private void PlayerMove(int num) {
		int dir = currentTile.TryExit(num);
		if (dir >= 0) { //TODO leaves option for no-op, i oughta add that
			Vector2Int newPos = mapData.GetMotion(currentPos, dir);
			Tile newTile = mapData.GetTile(newPos);

			if (newTile != null && newTile.TryEnter(mapData.InverseDir(dir))) {
				currentPos = newPos;
				currentTile = newTile;
				ExecutePhysicalMove(dir);
			}
		}
		MovementCompleteCallback();
	}

	//TODO: refactor these two ^ v so that there's no repeated code

	private void EnemyMove() {
		//TODO implement more advanced scoring system for choosing the most evil move >;)
		Vector2Int goalPos = mapData.GetClosestKillTile(currentPos);
		if (goalPos != Vector2Int.one * -1) { //TODO figure out a less annoying way to return no valid goal
			int dir = currentTile.TryExit(mapData.NextStepToGoal(currentPos, goalPos));
			Vector2Int newPos = mapData.GetMotion(currentPos, dir);
			Tile newTile = mapData.GetTile(newPos);
			currentPos = newPos;
			currentTile = newTile;
			ExecutePhysicalMove(dir);
		}
		MovementCompleteCallback();
	}

	private void ExecutePhysicalMove(int num) {
		if (num == 0) {
			transform.Translate(Vector3.forward);
			Debug.Log("up");
		}
		if (num == 1) {
			transform.Translate(-Vector3.forward);
			Debug.Log("down");
		}
		if (num == 2) {
			transform.Translate(Vector3.left);
			Debug.Log("left");
		}
		if (num == 3) {
			transform.Translate(-Vector3.left);
			Debug.Log("right");
		}
	}


	public void OnWin() {
		isAlive = false;
		isWin = true;
		GetComponent<Renderer>().material = winMat;
	}

	public void OnKill() {
		isAlive = false;
		GetComponent<Renderer>().material = killMat;
	}

	public void AttachToGridMap(Vector2Int pos, MapData map) {
		currentPos = pos;
		currentTile = map.GetTile(pos);
		mapData = map;
	}

	public void AttachInputHandler(InputHandler h) {
		inputHandler = h;
	}

	public bool ShiftFocus(int num) {
		if (isAlive) {
			if (num == focusNum) {
				inFocus = true;
				GetComponent<Renderer>().material = playerMat;
				return true;
			} else {
				inFocus = false;
				GetComponent<Renderer>().material = enemyMat;
			}
		}
		return false;
	}

	public void SetTooltipActive(bool state) {
		focusLabel.SetText(focusNum.ToString());
		focusLabel.SetActive(state);
	}

	private void MovementCompleteCallback() {
		inputHandler.ImportCompletedMotion();
	}

	public bool ProcessMapState() {
		return currentTile.OnActorPresent(this);
	}
}
