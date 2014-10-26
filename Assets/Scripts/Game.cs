using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

    public GUISkin ReversiSkin;
    public GUISkin MainSkin;
    public int rows = 8;
    public int cols = 8;
	public int tileSize = 70;
	public GUIText blackCount;
	public GUIText whiteCount;
	public SpriteRenderer activeGlow;

	private Board board;

	// AI related functionality
	public static bool HasDumbAI;
	private float aiWaitTime = 0.7f;
	private bool dumbAIhasMoved = true;
       
    // Use this for initialization
    void Start () {
		if (!HasDumbAI) { 
			GameObject aitext = GameObject.Find("AIText"); 
			aitext.guiText.enabled = false;
		}
		board = new Board(rows, cols);
		board.Grid[3,4].CurrentState = Tile.State.Black;
		board.Grid[3,3].CurrentState = Tile.State.White;
		board.Grid[4,4].CurrentState = Tile.State.White;
		board.Grid[4,3].CurrentState = Tile.State.Black;

		board.UpdateCount();
		UpdatePieceCount();

		BGAssert.Assert(board.GetTile(45,99).C == -1, "GetTile out of bounds not working!");
		BGAssert.Assert(board.CountTile(Tile.State.White) == 2, "CountTile() method not working!");
		BGAssert.Assert(!board.IsGameOver(), "Game should not be over yet!!");
	}
    
    // draws the GUI every frame:
    void OnGUI () {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
		if (board.IsGameOver()) {
			GUI.enabled = false;
			DrawBoard();
			GUI.enabled = true;
			BuildWinPrompt();
		} else {
			DrawBoard();
		}
		BuildBackBtn();
        GUILayout.EndArea();
    }

    private void DrawBoard() {
		GUI.skin = ReversiSkin;
		Tile[,] grid = board.Grid;

        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        for (int i = 0; i < rows; i++) {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            for (int j = 0; j < cols; j++) {
				Tile tile = grid[i, j];
				string img = "";
				switch (tile.CurrentState) {
					case Tile.State.White:
						img = "white";
						break;
					case Tile.State.Black:
						img = "black";
						break;
				}
				if (GUILayout.Button(Resources.Load<Texture>(img), 
				                     GUILayout.Width(tileSize),
				                     GUILayout.Height(tileSize))) {
					// condition prevents user from moving while AI is "thinking"
					if (dumbAIhasMoved) {
						board.ClickTile(tile);
					}
					if (HasDumbAI && dumbAIhasMoved && board.activePlayer == Tile.State.Black) {
						StartCoroutine("DumbAIMove");
					} 
				}
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

		updateActivePlayerDisplay();
		UpdatePieceCount();
    }

	private void BuildBackBtn() {
		GUI.skin = MainSkin;

		if (GUI.Button(new Rect(690, 532, 90, 45), "Back")) {
			Application.LoadLevel("title");
		}
	}

	private void BuildWinPrompt() {
		GUI.skin = null;
		GUI.skin.button.fontSize = 20;
		GUI.skin.box.fontSize = 16;
		
		int winPromptW = 300;
		int winPromptH = 100;
		
		float halfScreenW = Screen.width / 2;
		float halfScreenH = Screen.height / 2;
		
		int halfPromptW = (int)(winPromptW / 2);
		int halfPromptH = (int)(winPromptH / 2);
		
		GUI.BeginGroup(new Rect(halfScreenW - halfPromptW,
		                        halfScreenH - halfPromptH,
		                        winPromptW, winPromptH));
		string winner;
		if (board.winner == Tile.State.Empty) {
			winner = "Count is even. It's a TIE!";
		} else if (board.winner == Tile.State.Black) {
			winner = "Darkness once more reign supreme!";
		} else {
			winner = "The just and pure has prevailed!";
		}
		GUI.Box(new Rect(0, 0, winPromptW, winPromptH), winner);
		
		int buttonW = 120;
		int buttonH = 40;
		
		if (GUI.Button(new Rect(halfPromptW - (buttonW/2),
		                        halfPromptH - (buttonH/2)+10,
		                        120, 40),
		               "Play Again?")) {
			Debug.Log("Restart!!");
			Application.LoadLevel(Application.loadedLevel);
		}
		GUI.EndGroup();
	}

	private void UpdatePieceCount() {
		blackCount.text = board.blackCount.ToString("D2");   
		whiteCount.text = board.whiteCount.ToString("D2");  
    }

	private void updateActivePlayerDisplay() {
		if (board.activePlayer == Tile.State.White) {
			activeGlow.transform.position = new Vector3(-5.7f, -1.02f, 0);
		} else {
			activeGlow.transform.position = new Vector3(-5.7f, +1.61f, 0);
		}
	}

	IEnumerator DumbAIMove() {
		dumbAIhasMoved = false;
		yield return new WaitForSeconds(aiWaitTime);
		List<Tile> moves = board.PossibleMoves();
		int randNum = Random.Range(0, moves.Count);
		board.ClickTile(moves[randNum]);
		dumbAIhasMoved = true;
	}
}
