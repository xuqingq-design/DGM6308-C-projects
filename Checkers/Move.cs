//6: once move/capture
namespace Checkers;

public class Move
{
	public Piece PieceToMove { get; set; } //the moving piece

	public (int X, int Y) To { get; set; } //end moving position

	public Piece? PieceToCapture { get; set; } //the piece which is going to be captured(can be null)

	public Move(Piece pieceToMove, (int X, int Y) to, Piece? pieceToCapture = null)
	{
		PieceToMove = pieceToMove;
		To = to;
		PieceToCapture = pieceToCapture;
	}
}
