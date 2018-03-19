using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GridController : MonoBehaviour {
	public bool isServer = false;
	public int width, height;
	public GameObject cell;
	public GameObject[,] cells = null;
	private int chosenShip = -1;
	private bool isUpArrow = false;
	ShipsButtonController selectedShip = null;
	ArrowButtonController selectedArrow = null;

	public GameObject[,] GetCells() {
		return this.cells;
	}

	public void SetUpArrow(bool isUpArrow, ArrowButtonController selectedArrow) {
		if (this.selectedArrow != null)
			this.selectedArrow.EnableButton ();
		this.selectedArrow = selectedArrow;
		this.isUpArrow = isUpArrow;
	}

	public void ChoosenShip(int chosenSize, ShipsButtonController selectedShip) {
		if (this.selectedShip != null)
			this.selectedShip.MinusQuantityAndEnable (0);
		this.chosenShip = chosenSize;
		this.selectedShip = selectedShip;
	}

	private bool IfAvaileble(int i, int j, int shipSize, bool isUpArrow) {
		if (i >= this.width || i < 0 || j >= this.height || j < 0)
			return false;
		int realWidth = isUpArrow ? 1 : shipSize;
		int realHeight = isUpArrow ? shipSize : 1;
		if (i + realWidth > this.width || j + realHeight > this.height)
			return false;
		int xStart = i - 1 < 0 ? i : i - 1;
		int xEnd = i + realWidth + 1 > this.width ? i + realWidth : i + realWidth + 1;
		int yStart = j - 1 < 0 ? j : j - 1;
		int yEnd = j + realHeight + 1 > this.height ? j + realHeight : j + realHeight + 1;
		for (int x = xStart; x < xEnd; x++) {
			for (int y = yStart; y < yEnd; y++) {
				CellController cellC = cells[x,y].GetComponent<CellController>();
				if (!cellC.isFree)
					return false;
			}
		}
		return true;
	}

	private void ChangeCellPrefab (int i, int j, int shipSize, bool isUpArrow, int x, int y) {
		int diff = (isUpArrow ? y - j : x - i) + 1;
		CellController cellC = this.cells [x, y].GetComponent<CellController> ();
		if (shipSize == 1)
			cellC.SetOne ();
		else if (shipSize == 2) {
			if (diff == 1)
				cellC.SetStart ();
			else
				cellC.SetEnd ();
		} else if (shipSize == 3) {
			if (diff == 1)
				cellC.SetStart ();
			else if (diff == 2)
				cellC.SetMiddle ();
			else
				cellC.SetEnd ();
		} else if (shipSize == 4) {
			if (diff == 1)
				cellC.SetStart ();
			else if (diff == 2)
				cellC.SetMiddle ();
			else if (diff == 3)
				cellC.SetMiddle ();
			else
				cellC.SetEnd ();
		}
		cellC.isFree = false;
		if (isUpArrow)
			cellC.SetRotation (90f);
		cellC.StartX = i;
		cellC.StartY = j;
		cellC.isUp = isUpArrow;
		cellC.ShipSize = shipSize;
	}

	private void AddShip(int i, int j, int shipSize, bool isUpArrow) {
		int realWidth = isUpArrow ? 1 : shipSize;
		int realHeight = isUpArrow ? shipSize : 1;
		for (int x = i - 1; x < i + realWidth + 1; x++) {
			for (int y = j - 1; y < j + realHeight + 1; y++) {
				if (x < 0 || x >= this.width || y < 0 || y >= this.height)
					continue;
				if (x == i - 1 || x == i + realWidth || y == j - 1 || y == j + realHeight) {
					this.cells [x, y].GetComponent<CellController> ().isNeighbour = true;
					this.cells [x, y].GetComponent<CellController> ().SetUsed ();
					continue;
				}
				ChangeCellPrefab (i, j, shipSize, isUpArrow, x, y);
			}
		}
	}

	private bool isValid(int i, int j) {
		return i >= 0 && i < this.width && j >= 0 && j < this.height;
	}

	public void CheckAfterBombed(int startX, int startY, bool isUp, int shipSize, int i, int j) {
		if (isUp) {
			for (int y = startY; y < startY + shipSize; y++) {
				CellController cellC = this.cells [startX, y].GetComponent<CellController> ();
				if (!cellC.isFree && !cellC.isBombed)
					return;
			}
		} else {
			for (int x = startX; x < startX + shipSize; x++) {
				CellController cellC = this.cells [x, startY].GetComponent<CellController> ();
				if (!cellC.isFree && !cellC.isBombed)
					return;
			}
		}
		for (int x = startX - 1; x < startX + 1 + (isUp ? 1 : shipSize); x++) {
			for (int y = startY - 1; y < startY + 1 + (isUp ? shipSize : 1); y++) {
				if (!isValid(x, y))
					continue;
				CellController cellC = this.cells [x, y].GetComponent<CellController> ();
				if (cellC.isNeighbour) {
					cellC.SetYes ();
					cellC.isBombed = true;
					cellC.GetComponent<Button> ().interactable = false;
				}
			}
		}
		bool allIsBombed = true;
		for (int x = 0; x < this.width; x++) {
			if (!allIsBombed)
				break;
			for (int y = 0; y < this.height; y++) {
				CellController cellC = this.cells [x, y].GetComponent<CellController> ();
				if (!cellC.isFree && !cellC.isBombed) {
					allIsBombed = false;
					break;
				}
			}
		}
		if (allIsBombed) {
			for (int x = 0; x < this.width; x++) {
				for (int y = 0; y < this.height; y++) {
					CellController cellC = this.cells [x, y].GetComponent<CellController> ();
					if (cellC.isFree) {
						cellC.SetYes ();
					}
				}
			}
			GameController gameC = GameController.main ();
			if (this.isServer) {
				gameC.DisplayWinner (false);
				//done game
			} else {
				gameC.DisplayWinner (true);
				//done game
			}
			gameC.sinkSound ();
		}
	}

	public void CellClickFight(int i, int j) {
		this.cells [i, j].GetComponent<CellController> ().fightClickResult ();
	}

	public void CellClick(int i, int j) {
		if (this.chosenShip < 0)
			return;
		if (!IfAvaileble (i, j, this.chosenShip, this.isUpArrow))
			return;
		AddShip (i, j, this.chosenShip, this.isUpArrow);
		if (this.selectedShip != null)
			this.selectedShip.MinusQuantityAndEnable (1);
		if (this.selectedArrow != null)
			this.selectedArrow.EnableButton ();
		this.selectedArrow = null;
		this.chosenShip = -1;
		this.selectedShip = null;
		this.isUpArrow = false;
	}

	public void CellAddInfo(int i, int j, bool isFree, bool isNeighbour, int startX, int startY, int shipSize, bool isUp) {
		CellController cellC = this.cells [i, j].GetComponent<CellController> ();
		cellC.isFree = isFree;
		cellC.isNeighbour = isNeighbour;
		cellC.StartX = startX;
		cellC.StartY = startY;
		cellC.ShipSize = shipSize;
		cellC.isUp = isUp;
	}

	public void CellIsNeighbour (int i, int j) {
		CellController cellC = this.cells [i, j].GetComponent<CellController> ();
		cellC.isNeighbour = true;
	}

	public void CreateGrid() {
		CleanGrid ();
		this.cells = new GameObject[width, height];
		for (int i = 0; i < this.cells.GetLength (0); i++) {
			for (int j = 0; j < this.cells.GetLength (1); j++) {
				this.cells [i, j] = Instantiate (this.cell);
				this.cells [i, j].transform.SetParent (transform);
				PlaceCell (i, j, this.cells [i, j]);
			}
		}
	}

	private void PlaceCell (int i, int j, GameObject cell) {
		cell.GetComponent<CellController> ().i = i;
		cell.GetComponent<CellController> ().j = j;
		cell.GetComponent<CellController> ().isServer = this.isServer;
		RectTransform trans = cell.GetComponent<RectTransform> ();
		float cellW = 1.0f / this.width;
		float cellH = 1.0f / this.height;
		trans.anchorMin = new Vector2 (cellW * i, cellH * j);
		trans.anchorMax = new Vector2 (cellW * (i + 1), cellH * (j + 1));
		trans.anchoredPosition = Vector2.zero;
		trans.sizeDelta = Vector2.zero;
	}

	public void SetNeighboursAsFree() {
		if (this.cells != null) {
			for (int i = 0; i < this.cells.GetLength (0); i++) {
				for (int j = 0; j < this.cells.GetLength (1); j++) {
					CellController cellC = cells [i, j].GetComponent<CellController> ();
					if (cellC != null) {
						if (cellC.isNeighbour)
							cellC.SetFree ();
					}
				}
			}
		}
	}

	public void RemoveCellClick() {
		if (this.cells != null) {
			for (int i = 0; i < this.cells.GetLength (0); i++) {
				for (int j = 0; j < this.cells.GetLength (1); j++) {
					CellController cellC = cells [i, j].GetComponent<CellController> ();
					if (cellC != null)
						cellC.RemoveOnClick ();
				}
			}
		}
	}

	public void GetCellInfo(int x, int y) {

	}

	public void CleanGrid() {
		if (this.cells != null) {
			for (int i = 0; i < this.cells.GetLength (0); i++) {
				for (int j = 0; j < this.cells.GetLength (1); j++) {
					Destroy (this.cells [i, j]);
					this.cells [i, j] = null;
				}
			}
		}
		this.cells = null;
	}

	public void CreateGridForFight() {

	}

	public void SetOnClickListener() {
		if (this.cells != null) {
			for (int i = 0; i < this.cells.GetLength (0); i++) {
				for (int j = 0; j < this.cells.GetLength (1); j++) {
					CellController cellC = cells [i, j].GetComponent<CellController> ();
					if (cellC != null)
						cellC.SetOnClickListener ();
				}
			}
		} else
			print ("SetOnClickListener cell is null");
	}
}
