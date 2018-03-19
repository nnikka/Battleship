using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DoneButtonController : MonoBehaviour {
	private GameController gameController;
	private Button but;

	void Start() {
		this.but = GetComponent<Button> ();
		this.but.onClick.AddListener (OnDoneClick);
		this.gameController = GameController.main();
	}

	public void OnDoneClick() {
		this.gameController.SettingDone ();
	}
}
