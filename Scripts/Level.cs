using System.Collections;
using UnityEngine;

public class Level : MonoBehaviour {
	public string title;
	private LevelManager manager;
	public ActorController[] allActors; //TODO: automate the collection of the actors so it's not a public variable
	public int defaultFocus;

	public void AttachLevelManager(LevelManager lm) {
		manager = lm;
	}

	public void Begin(bool firstTime) {
		StartCoroutine(BeginRoutine(firstTime));
	}

	private IEnumerator BeginRoutine(bool firstTime) {
		Vector3 startPos = transform.position;
		for (float i = 0; i < 1; i += manager.delta) {
			transform.position = Vector3.Lerp(startPos, Vector3.zero, manager.levelStartCurve.Evaluate(i));
			yield return new WaitForSeconds(manager.delta * manager.seconds);
		}
		transform.position = Vector3.zero;
		manager.MarkTransitionComplete(firstTime);
	}

	public void Die() {
		StartCoroutine(DieRoutine());
	}


	private IEnumerator DieRoutine() {
		Vector3 startPos = transform.position;
		for (float i = 0; i < 1; i += manager.delta) {
			transform.position = Vector3.Lerp(startPos, Vector3.down*8, manager.levelEndCurve.Evaluate(i));
			yield return new WaitForSeconds(manager.delta * manager.seconds);
		}
		Destroy(gameObject);
	}

}
