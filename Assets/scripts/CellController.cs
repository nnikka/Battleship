using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class CellController : MonoBehaviour {
	public int i;
	public int j;
	public Sprite free;
	public Sprite yes;
	public Sprite x;
	public Sprite one;
	public Sprite start;
	public Sprite middle;
	public Sprite end;
	public bool isNeighbour = false;
	public bool isFree = true;
	public bool isBombed = false;
	public bool isServer;

	public bool isUp;
	public int StartX;
	public int StartY;
	public int ShipSize;

	public void OnCellClick() {
		GridController grid = transform.parent.GetComponent<GridController> ();
		grid.CellClick (i, j);
	}

	private void onFightClick() {
		GameController gameC = GameController.main ();
		gameC.PlayerC.OnCellClickFight (this.i, this.j, this.isServer);
	}

	public void fightClickResult() {
		this.isBombed = true;
		if (!this.isFree) {
			SetX ();
			GridController grid = transform.parent.GetComponent<GridController> ();
			grid.CheckAfterBombed (this.StartX, this.StartY, this.isUp, this.ShipSize, this.i, this.j);
			GameController gameC = GameController.main ();
			gameC.hitSound ();
		} else {
			SetYes ();
		}
		this.GetComponent<Button> ().interactable = false;
	}

	public void SetOnClickListener() {
		this.transform.GetComponent<Button> ().onClick.AddListener (onFightClick);
	}

	private void noFunc() {
	}

	public void RemoveOnClick() {
		this.transform.GetComponent<Button> ().onClick.AddListener (noFunc);
	}

	public void SetUsed() {
		Set (this.yes);
	}

	public void SetRotation(float rot) {
		RectTransform trans = GetComponent<RectTransform> ();
		Vector3 v = trans.localEulerAngles;
		v.z = rot;
		trans.localEulerAngles = v;
	}

	private void Set(Sprite spr) {
		GetComponent<Image> ().sprite = spr;
	}

	public void SetFree() {
		Set (this.free);
	}

	public void SetX() {
		Set (this.x);
	}

	public void SetStart() {
		Set (this.start);
	}

	public void SetMiddle() {
		Set (this.middle);
	}

	public void SetEnd() {
		Set (this.end);
	}

	public void SetYes() {
		Set (this.yes);
	}

	public void SetOne() {
		Set (this.one);
	}
}
