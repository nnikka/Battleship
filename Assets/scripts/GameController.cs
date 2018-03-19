using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	private static GameController singlton = null;
	private bool isServer = false;
	public GridController ServerGrid;
	public GridController ClientGrid;

	public PlayerController PlayerC;

	public GameObject ServerPanel;
	public GameObject ServerShips;
	public GameObject ClientPanel;
	public GameObject ClientShips;
	public GameObject Disconnected;
	public GameObject MiddleLine;
	public GameObject ServerTurnText;
	public GameObject ClientTurnText;
	public GameObject ServerWinText;
	public GameObject ClientWinText;

	public AudioClip Shoot;
	public AudioClip Sinking;
	public AudioClip Hit;


	private bool serverDone = false;
	private bool clientDone = false;
	private GameObject Player = null;
	private bool gameStarted = false;

	private GameObject[,] ServerGridShips;
	private GameObject[,] ClientGridShips;

	public bool ServerTurn = true;

	public static GameController main () {
		if (singlton == null)
			singlton = FindObjectOfType<GameController> ();
		return singlton;
	}

	public void shootSound() {
		GetComponent<AudioSource> ().PlayOneShot (this.Shoot);
	}

	public void sinkSound() {
		GetComponent<AudioSource> ().PlayOneShot (this.Sinking);
	}

	public void hitSound() {
		GetComponent<AudioSource> ().PlayOneShot (this.Hit);
	}

	public void Start() {
		this.ServerGrid.isServer = true;
		this.ClientGrid.isServer = false;
	}

	private void SetClientPanelAnchors(Vector2 min, Vector2 max) {
		RectTransform trans = this.ClientPanel.GetComponent<RectTransform> ();
		trans.anchorMin  = min;
		trans.anchorMax = max;
	}

	private void SetServerPanelAnchors(Vector2 min, Vector2 max) {
		RectTransform trans = this.ServerPanel.GetComponent<RectTransform> ();
		trans.anchorMin  = min;
		trans.anchorMax = max;
	}

	private void RestartAllValues() {
		RemoveWins ();
		RemoveTurns ();
		this.ServerTurn = true;
		this.isServer = false;
		this.serverDone = false;
		this.clientDone = false;
		this.Player = null;
		this.gameStarted = false;
		this.ServerPanel.SetActive (false);
		this.ClientPanel.SetActive (false);
		this.ServerShips.SetActive (false);
		this.ClientShips.SetActive (false);
		this.MiddleLine.SetActive (false);
		SetServerPanelAnchors (new Vector2(0.0f, 0.25f), new Vector2(0.5f, 1.0f));
		SetClientPanelAnchors (new Vector2(0.5f, 0.25f), new Vector2(1.0f, 1.0f));
		RestartShipValues ();
	}

	private void RestartShipValues() {
		ServerShips.gameObject.transform.FindChild ("Sh1/Quantity").GetComponent<Text> ().text = "4";
		ServerShips.gameObject.transform.FindChild ("Sh2/Quantity").GetComponent<Text> ().text = "3";
		ServerShips.gameObject.transform.FindChild ("Sh3/Quantity").GetComponent<Text> ().text = "2";
		ServerShips.gameObject.transform.FindChild ("Sh4/Quantity").GetComponent<Text> ().text = "1";
		ClientShips.gameObject.transform.FindChild ("Sh1/Quantity").GetComponent<Text> ().text = "4";
		ClientShips.gameObject.transform.FindChild ("Sh2/Quantity").GetComponent<Text> ().text = "3";
		ClientShips.gameObject.transform.FindChild ("Sh3/Quantity").GetComponent<Text> ().text = "2";
		ClientShips.gameObject.transform.FindChild ("Sh4/Quantity").GetComponent<Text> ().text = "1";
		//
		ServerShips.gameObject.transform.FindChild ("Sh1/Button").GetComponent<Button> ().interactable = true;
		ServerShips.gameObject.transform.FindChild ("Sh2/Button").GetComponent<Button> ().interactable = true;
		ServerShips.gameObject.transform.FindChild ("Sh3/Button").GetComponent<Button> ().interactable = true;
		ServerShips.gameObject.transform.FindChild ("Sh4/Button").GetComponent<Button> ().interactable = true;
		ClientShips.gameObject.transform.FindChild ("Sh1/Button").GetComponent<Button> ().interactable = true;
		ClientShips.gameObject.transform.FindChild ("Sh2/Button").GetComponent<Button> ().interactable = true;
		ClientShips.gameObject.transform.FindChild ("Sh3/Button").GetComponent<Button> ().interactable = true;
		ClientShips.gameObject.transform.FindChild ("Sh4/Button").GetComponent<Button> ().interactable = true;

	}

	private void OnDisconnect() {
		this.Disconnected.SetActive (true);
		RestartAllValues ();
	}

	void Update() {
		if (this.Player == null && !this.Disconnected.gameObject.activeSelf)
			OnDisconnect ();
		if (!this.gameStarted && this.serverDone && this.clientDone) {
			BothIsDone ();
			ChangeTurns ();
		}
	}

	private void BothIsDone() {
		this.gameStarted = true;
		this.ClientPanel.SetActive (true);
		this.ServerPanel.SetActive (true);
		this.ClientShips.SetActive (false);
		this.ServerShips.SetActive (false);
		this.MiddleLine.SetActive (true);
		if (this.isServer) {
			//this.ClientGrid.CreateGrid ();
			//this.ClientGrid.SetOnClickListener ();
			this.ServerGridShips = this.ServerGrid.GetCells ();
			PlayerC.SendShipPlacesFromServer (this.ServerGridShips);
		} else {
			//this.ServerGrid.CreateGrid ();
			//this.ServerGrid.SetOnClickListener ();
			this.ClientGridShips = this.ClientGrid.GetCells ();
			PlayerC.SendShipPlacesFromClient (this.ClientGridShips);
		}
	}

	public void SetServer(bool Server, PlayerController PController) {
		this.isServer = Server;
		this.PlayerC = PController;
		if (Server) {
			ConnectedServer ();
		} else {
			ConnectedClient ();
		}
		this.Player = PlayerC.gameObject;
		this.Disconnected.SetActive (false);
	}

	private void ConnectedServer() {
		this.ServerPanel.SetActive (true);
		this.ServerShips.SetActive (true);
		this.ServerGrid.CreateGrid ();
	}

	private void ConnectedClient() {
		this.ClientPanel.SetActive (true);
		this.ClientShips.SetActive (true);
		this.ClientGrid.CreateGrid ();
	}

	public void StartButton() {
		//
	}

	public void ChangeTurns() {
		RemoveWins ();
		if (this.ServerTurn) {
			this.ServerTurnText.SetActive (true);
			this.ClientTurnText.SetActive (false);
		} else {
			this.ServerTurnText.SetActive (false);
			this.ClientTurnText.SetActive (true);
		}
	}

	public void DisplayWinner(bool server) {
		RemoveTurns ();
		if (server) {
			this.ServerWinText.SetActive (true);
			this.ClientWinText.SetActive (false);
		} else {
			this.ServerWinText.SetActive (false);
			this.ClientWinText.SetActive (true);
		}
	}

	public void RemoveTurns() {
		this.ServerTurnText.SetActive (false);
		this.ClientTurnText.SetActive (false);
	}

	public void RemoveWins() {
		this.ServerWinText.SetActive (false);
		this.ClientWinText.SetActive (false);
	}

	public void SettingDone() {
		DonePreparing();
		if (this.PlayerC != null)
			this.PlayerC.DoneEditing ();
	}

	private void PrepareServerForFight() {
		this.ServerGridShips = this.ServerGrid.GetCells ();
		this.ServerGrid.RemoveCellClick ();
		this.ServerGrid.SetNeighboursAsFree ();
		this.ServerShips.SetActive (false);
		SetServerPanelAnchors (new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.9f));
		SetClientPanelAnchors (new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.9f));
	}

	private void PrepareClientForFight() {
		this.ClientGridShips = this.ClientGrid.GetCells ();
		this.ClientGrid.SetNeighboursAsFree ();
		this.ClientGrid.RemoveCellClick ();
		this.ClientShips.SetActive (false);
		SetServerPanelAnchors (new Vector2(0.0f, 0.0f), new Vector2(0.5f, 0.9f));
		SetClientPanelAnchors (new Vector2(0.5f, 0.0f), new Vector2(1.0f, 0.9f));
	}



	public void DonePrepare(bool Server) {
		if (Server)
			this.serverDone = true;
		else
			this.clientDone = true;
	}

	public void DonePreparing() {
		if (this.isServer)
			PrepareServerForFight ();
		else
			PrepareClientForFight ();
	}
	/*
	public void DrawServerGrid() {
		this.ServerGrid.isServer = true;
		this.ServerGrid.CreateGrid ();
	}

	public void DrawClientGrid() {
		this.ServerGrid.isServer = false;
		this.ClientGrid.CreateGrid ();
	}
	*/
}
