using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartBGController : MonoBehaviour {

	private GameController gameController;
	private Button but;

	void Start() {
		this.but = GetComponent<Button> ();
		this.gameController = GameController.main();
		this.but.onClick.AddListener (OnStartClick);
	}

	public void OnStartClick() {
		this.but.gameObject.SetActive (false);
		this.gameController.StartButton ();
	}
}
