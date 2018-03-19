using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShipsButtonController : MonoBehaviour {
	private Button but;
	public bool isServer;
	public int shipSize;

	private GameController gameController;

	void Start() {
		this.but = GetComponent<Button> ();
		this.but.onClick.AddListener (OnShipClick);
		this.gameController = GameController.main();
	}

	public void OnShipClick() {
		this.but.interactable = false;
		if (isServer) {
			this.gameController.ServerGrid.ChoosenShip (this.shipSize, this);
		} else {
			this.gameController.ClientGrid.ChoosenShip (this.shipSize, this);
		}
	}

	public void MinusQuantityAndEnable(int count) {
		if (count == 0) {
			this.but.interactable = true;
			return;
		}
		foreach (Transform child in transform.parent) {
			if (child.name == "Quantity"){
				Text Quantity = child.gameObject.GetComponent<Text>();
				int qty = int.Parse (Quantity.text);
				qty--;
				if (qty > 0)
					this.but.interactable = true;
				Quantity.text = "" + qty;
			}
		}
	}
}
