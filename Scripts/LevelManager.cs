using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public InputHandler input;
	public TextPopup levelTitle;
	public int currentIndex = 0;
	public Level[] allLevels;

	public AnimationCurve levelStartCurve;
	public AnimationCurve levelEndCurve;

	public float delta = 0.01f;
	public float seconds = 1.5f;

	private Level currentLevel;

	// Start is called before the first frame update
	void Start() {
		LoadLevel(currentIndex, true);
	}

	public void IncrementLevel() {
		KillLevel(currentIndex);
		currentIndex++;
		LoadLevel(currentIndex, true);
	}

	public void ResetLevel() {
		KillLevel(currentIndex);
		LoadLevel(currentIndex, false);
	}

	private void KillLevel(int index) {
		currentLevel.Die();
	}

	private void LoadLevel(int index, bool firstTime) {
		input.SetControlsLocked(true);
		currentLevel = Instantiate(allLevels[index], Vector3.up*8, Quaternion.identity);
		currentLevel.AttachLevelManager(this);
		input.AttachActors(currentLevel.allActors, currentLevel.defaultFocus);
		currentLevel.Begin(firstTime);
	}

	public void MarkTransitionComplete(bool firstTime) {
		input.SetControlsLocked(false);
		if (firstTime) {
			input.SetAllTooltipsActive(true);
			StartCoroutine(ShowLevelTitle());
		}
	}

	private IEnumerator ShowLevelTitle() {
		levelTitle.SetText(currentLevel.title);
		levelTitle.SetActive(true);
		yield return new WaitForSeconds(3f);
		levelTitle.SetActive(false);
	}
}
