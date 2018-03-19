using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerController : NetworkBehaviour {
	GameController gameController;

	void Start() {
		this.gameController = GameController.main ();
		this.gameController.SetServer (isServer, this);
	}

	public void OnCellClickFight(int i, int j, bool Server) {
		if (isServer) {
			if (gameController.ServerTurn) {
				RpcClickCellOn (false, i, j);
				if (this.gameController.ClientGrid.cells [i, j].GetComponent<CellController> ().isFree) {
					gameController.ServerTurn = false;
					gameController.ChangeTurns ();
				}
			}
		} else {
			if (!gameController.ServerTurn) {
				this.gameController.ServerGrid.CellClickFight (i, j);
				CmdClickCellOn (true, i, j);
				if (this.gameController.ServerGrid.cells [i, j].GetComponent<CellController> ().isFree) {
					gameController.ServerTurn = true;
					gameController.ChangeTurns ();
				}
			}
		}
	}

	[Command]
	public void CmdClickCellOn(bool Server, int i, int j) {
		if (this.gameController == null)
			this.gameController = GameController.main ();
		this.gameController.shootSound ();
		if (this.gameController.ServerGrid.cells [i, j].GetComponent<CellController> ().isFree) {
			gameController.ServerTurn = true;
			gameController.ChangeTurns ();
		}
		if (Server) {
			this.gameController.ServerGrid.CellClickFight (i, j);
		} else {
			this.gameController.ClientGrid.CellClickFight (i, j);
		}
	}

	[ClientRpc]
	public void RpcClickCellOn(bool Server, int i, int j) {
		if (this.gameController == null)
			this.gameController = GameController.main ();
		this.gameController.shootSound ();
		if (this.gameController.ClientGrid.cells [i, j].GetComponent<CellController> ().isFree) {
			gameController.ServerTurn = false;
			gameController.ChangeTurns ();
		}
		if (Server) {
			this.gameController.ServerGrid.CellClickFight (i, j);
		} else {
			this.gameController.ClientGrid.CellClickFight (i, j);
		}
	}

	[ClientRpc]
	public void RpcCreateServerSideOnClients() {
		if (!isServer) {
			this.gameController.ServerGrid.CreateGrid ();
			this.gameController.ServerGrid.SetOnClickListener ();
		}
	}

	public void SendShipPlacesFromServer (GameObject[,] ServerGridShips) {
		if (isServer && ServerGridShips != null) {
			RpcCreateServerSideOnClients ();
			for (int i = 0; i < ServerGridShips.GetLength (0); i++) {
				for (int j = 0; j < ServerGridShips.GetLength (1); j++) {
					CellController cellC = ServerGridShips [i, j].GetComponent<CellController> ();
					RpcAddInfosOnServerGrid (i, j, cellC.isFree, cellC.isNeighbour, cellC.StartX, cellC.StartY, 
											cellC.ShipSize, cellC.isUp);
				}
			}
		}
	}

	public void SendShipPlacesFromClient(GameObject[,] ClientShips) {
		if (!isServer && ClientShips != null) {
			CmdCreateClientSideOnServer ();
			for (int i = 0; i < ClientShips.GetLength (0); i++) {
				for (int j = 0; j < ClientShips.GetLength (1); j++) {
					CellController cellC = ClientShips [i, j].GetComponent<CellController> ();
					CmdAddInfosOnClientGrid (i, j, cellC.isFree, cellC.isNeighbour, cellC.StartX, cellC.StartY, 
						cellC.ShipSize, cellC.isUp);
				}
			}
		}
	}

	[ClientRpc]
	public void RpcAddInfosOnServerGrid(int i, int j, bool isFree, bool isNeighbour, int startX, int startY, int shipSize, bool isUp) {
		if (isServer)
			return;
		if (this.gameController == null)
			this.gameController = GameController.main ();
		this.gameController.ServerGrid.CellAddInfo (i, j, isFree, isNeighbour, startX, startY, shipSize, isUp);
	}

	[Command]
	public void CmdAddInfosOnClientGrid(int i, int j, bool isFree, bool isNeighbour, int startX, int startY, int shipSize, bool isUp) {
		if (this.gameController == null)
			this.gameController = GameController.main ();
		this.gameController.ClientGrid.CellAddInfo (i, j, isFree, isNeighbour, startX, startY, shipSize, isUp);
	}

	[Command]
	public void CmdCreateClientSideOnServer() {
		if (isServer) {
			this.gameController.ClientGrid.CreateGrid ();
			this.gameController.ClientGrid.SetOnClickListener ();
		}
	}

	public void DoneEditing() {
		if (isServer) {
			RpcDonePreparing(isServer);
		} else {
			this.gameController.DonePrepare (isServer);
			CmdDonePreparing (isServer);
		}
	}

	[ClientRpc]
	public void RpcDonePreparing(bool Server) {
		if (this.gameController == null)
			this.gameController = GameController.main ();
		this.gameController.DonePrepare(Server);
	}

	[Command]
	public void CmdDonePreparing(bool Server) {
		if (this.gameController == null)
			this.gameController = GameController.main ();
		this.gameController.DonePrepare(Server);
	}

	/*
	[ClientRpc]
	public void RpcDrawServerGrid() {
		if (this.gameController == null)
			this.gameController = GameController.main ();
		this.gameController.DrawServerGrid ();
	}

	[Command]
	public void CmdDrawClientGrid() {
		if (this.gameController == null)
			this.gameController = GameController.main ();
		this.gameController.DrawClientGrid ();
	}
	*/
}
