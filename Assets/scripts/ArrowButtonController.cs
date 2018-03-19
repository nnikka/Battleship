using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArrowButtonController : MonoBehaviour {
	public bool isServer;
	public bool isUpArrow;
	private GameController gameController;
	private Button but;

	void Start() {
		this.but = GetComponent<Button> ();
		this.but.onClick.AddListener (OnArrowClick);
		this.gameController = GameController.main();
	}

	public void OnArrowClick() {
		this.but.interactable = false;
		if (isServer) {
			this.gameController.ServerGrid.SetUpArrow (this.isUpArrow == true, this);
		} else {
			this.gameController.ClientGrid.SetUpArrow (this.isUpArrow == true, this);
		}
	}

	public void EnableButton() {
		this.but.interactable = true;
	}
}
