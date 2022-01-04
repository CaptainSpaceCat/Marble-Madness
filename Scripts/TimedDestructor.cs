using System.Collections;
using UnityEngine;

public class TimedDestructor : MonoBehaviour {

	void Start() {
		StartCoroutine(Activate(5));
	}

	public IEnumerator Activate(int time) {
		for (int i = 0; i < time; i++) {
			yield return new WaitForSeconds(1f);
		}
		Destroy(gameObject);
	}
}
