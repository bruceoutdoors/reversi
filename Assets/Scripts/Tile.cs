public class Tile {

	public enum State {
		Empty,
		White,
		Black
	};

	public int C { get; set; }
	public int R { get; set; }

	public State CurrentState { get; set; }

	// default x and y values signifies an invalid tile
	public Tile (int x = -1, int y = -1) {
		CurrentState = State.Empty;
		C = x;
		R = y;
	}
}


