//5: create Piece
namespace Checkers;

public class Piece
{
	public int X { get; set; }

	public int Y { get; set; } //the piece's X and Y position

	public string NotationPosition
	{
		get => Board.ToPositionNotationString(X, Y); //transform: (0,2) => "A3"
		set => (X, Y) = Board.ParsePositionNotation(value);//"A3" => (0,2)
	}

	public PieceColor Color { get; init; }

	//NEW：
	public PieceType Type {get; set;}

	public bool Promoted { get; set; } //default: false
}
