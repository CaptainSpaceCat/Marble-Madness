using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class TextPopup : MonoBehaviour
{
    private bool dismissedFlag;

	void Awake() {
        SetActive(false);
        transform.rotation = Camera.main.transform.rotation;

    }

	public void SetActive(bool state) {
        if (state) {
            dismissedFlag = false;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.zero;
            StartCoroutine(TextFloat());
        } else {
            dismissedFlag = true;
		}
	}

    public void SetText(string text) {
        GetComponent<TextMeshPro>().text = text;
	}
    
    void LateUpdate()
    {
        //transform.LookAt(GameObject.FindObjectOfType<Camera>().transform);
    }

    private IEnumerator TextFloat() {
        //activate
        float delta = 0.01f;
        float seconds = 0.5f;
        for (float i = 0; i < 1; i += delta) {
            transform.localPosition = Vector3.Lerp(Vector3.zero, Vector3.up*1.5f, i);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, i);
            yield return new WaitForSeconds(delta * seconds);
        }
        transform.localPosition = Vector3.up * 1.5f;
        transform.localScale = Vector3.one;
        yield return new WaitForSeconds(delta * seconds);

        //hover
        float n = 0;
        float hoverDelta = 0.03f;
        while (!dismissedFlag) {
            transform.localPosition = Vector3.Lerp(Vector3.up*1.5f, Vector3.up*1.7f, Mathf.Sin(n)*.5f + .5f);
            n += hoverDelta;
            yield return new WaitForSeconds(.01f);
		}

        //deactivate
        for (float i = 0; i < 1; i += delta) {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, i);
            yield return new WaitForSeconds(delta * seconds);
        }
    }
}
