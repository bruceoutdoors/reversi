using UnityEngine;
using System.Collections.Generic;

public class Board {

	public Tile[,] Grid { get; set; }
	public Tile.State winner { get; private set; }
	public Tile.State activePlayer { get; private set; }
	public int whiteCount { get; private set; }
	public int blackCount { get; private set; }

	private int rows, cols;
	private bool hasNoPossibleMoves = false;

	// all possible directions, clockwise from top
	private int[,] directions = {
		{ 0, -1}, // TOP
		{+1, -1}, // TOP RIGHT
		{+1,  0}, // RIGHT
		{+1, +1}, // BOTTOM RIGHT
		{ 0, +1}, // BOTTOM
		{-1, +1}, // BOTTOM LEFT
		{-1,  0}, // LEFT
		{-1, -1}  // TOP LEFT
	};

	public Board (int rows = 8, int cols = 8) {
		activePlayer = Tile.State.White;
		winner       = Tile.State.Empty;

		this.rows = rows;
		this.cols = cols;

		Grid = new Tile[rows, cols];

		for (int j = 0; j < cols; j++) {
			for (int i = 0; i < rows; i++) {
				Grid[i, j] = new Tile(j, i);
			}
		}
	}

	public void ClickTile(Tile tile) {
		if (tile.CurrentState != Tile.State.Empty) {
			Debug.Log("Spot is taken. Please place on empty tile.");
			return;
		}

		List<Tile> flippedTiles = new List<Tile>();
		for (int i = 0; i < 8; i++) {
			List<Tile> l = GetFlippedTiles(tile, directions[i,0], directions[i,1]);
			flippedTiles.AddRange(l);
        }

		if (flippedTiles.Count <= 0) {
			Debug.Log("You must kill something.");
			return;
		}

		foreach (Tile t in flippedTiles) {
			t.CurrentState = activePlayer;
		}

		tile.CurrentState = activePlayer;
//		Debug.Log(string.Format("Placed {0} piece at [{1}, {2}]", tile.CurrentState, tile.C, tile.R));

		SwitchPlayer();
		if (PossibleMoves().Count <= 0) {
			// player looses a turn if there are no possible moves:
			Debug.Log(activePlayer + " has no possible moves. Turn is lost!");
			SwitchPlayer();
			// if no possible moves for both players
			if (PossibleMoves().Count <= 0) {
				hasNoPossibleMoves = true;
				Debug.Log("There are no possible moves for both players. Game end!");
			}
		} 

		UpdateCount();
	}

	public Tile GetTile(int x, int y) {
		// boundary checking
		if (y < 0 || y >= rows || x < 0 || x >= cols) {
			return new Tile();
		}
		return Grid[y, x];
	}
	
	public Tile GetNeighborTile(Tile tile, int col, int row) {
		return GetTile(tile.C + col, tile.R + row);
	}

	public int CountTile(Tile.State state) {
		int count = 0;
		for (int j = 0; j < cols; j++) {
			for (int i = 0; i < rows; i++) {
				if (Grid[j,i].CurrentState == state) {
					count++;
				}
            }
        }

		return count;
	}

	public bool IsGameOver() {
		if (CountTile(Tile.State.Empty) == 0
		    || blackCount == 0
		    || whiteCount == 0
		    || hasNoPossibleMoves) {
			if (whiteCount == blackCount) {
				winner = Tile.State.Empty;
			} else if (whiteCount > blackCount) {
				winner = Tile.State.White;
			} else {
				winner = Tile.State.Black;
			} 
        	return true;
		}

		return false;
	}

	public void UpdateCount() {
		whiteCount = CountTile(Tile.State.White);
		blackCount = CountTile(Tile.State.Black);
	}

	// uses the active player

	// given a tile (and therefore getting having X and Y), 
	// and a direction (X, Y) 
	// returns a list of tiles that need to be flipped based on that direction
	// iterates through if the tile is different from given tile
	// if it stops reaching a same tile, the list is returned


	public List<Tile> GetFlippedTiles(Tile tile, int col, int row) {
		List<Tile> list = new List<Tile>();

		Tile nextTile = GetNeighborTile(tile, col, row);
		// if it stops reaching an empty tile, out of bounds or 
		// same tile as active, returns an empty list
		if (nextTile.C == -1 || !isOpposite(nextTile)) return list; 

		//assuming, only the opposite tile will be added
		BGAssert.Assert(isOpposite(nextTile), "Needs to be opposite!");
		list.Add(nextTile);

		// here on founded opposite tile, now see if it extends further
        while (true) {
			nextTile = GetNeighborTile(nextTile, col, row);
			 if (nextTile.CurrentState == activePlayer) {
				return list;
			} else if (isOpposite(nextTile)) {
				list.Add(nextTile);
			} else if (nextTile.CurrentState == Tile.State.Empty) {
                // here it shout hit an empty tile/out of bounds, in which an empty list is returned:
				return new List<Tile>();
			} else {
				BGAssert.Assert(nextTile.CurrentState == Tile.State.Empty, "It should not reach here!!");
			}
		}
    }

	public List<Tile> PossibleMoves()
	{
		List<Tile> moves = new List<Tile>();
		for (int j = 0; j < cols; j++) {
			for (int i = 0; i < rows; i++) {
				Tile currentTile = Grid[j,i];
				if (currentTile.CurrentState == Tile.State.Empty) {
					List<Tile> flippedTiles = new List<Tile>();
					for (int k = 0; k < 8; k++) {
						List<Tile> l = GetFlippedTiles(currentTile, directions[k,0], directions[k,1]);
						flippedTiles.AddRange(l);
					}
					if (flippedTiles.Count > 0) {
						moves.Add(currentTile);
					}
				}
			}
		}
		return moves;
	}

	public void SwitchPlayer() {
		if (activePlayer == Tile.State.White) {
			activePlayer = Tile.State.Black;
		} else {
			activePlayer = Tile.State.White;
		}
	}

	private bool isOpposite(Tile tile) {
		if (tile.CurrentState == Tile.State.Empty) return false;
		if (tile.CurrentState == activePlayer)     return false;
		return true;
	} 

}

