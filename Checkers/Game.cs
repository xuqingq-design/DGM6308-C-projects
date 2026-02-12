//9: manage the player turns and check winners
namespace Checkers;

public class Game
{
	private const int PiecesPerColor = 12; //each color piece amount

	public PieceColor Turn { get; private set; }
	public Board Board { get; }
	public PieceColor? Winner { get; private set; }
	public List<Player> Players { get; }

	public Game(int humanPlayerCount)
	{
		if (humanPlayerCount < 0 || 2 < humanPlayerCount) throw new ArgumentOutOfRangeException(nameof(humanPlayerCount));
		Board = new Board();
		Players = new()
		{
			new Player(humanPlayerCount >= 1, Black), //if PVE, Black = huaman
			new Player(humanPlayerCount >= 2, White), //if PVP(human amount>=2), White = human
		};
		Turn = Black; //Black goes first
		Winner = null;
	}

	public void PerformMove(Move move) //perform move
	{
		//update the piece position
		(move.PieceToMove.X, move.PieceToMove.Y) = move.To;
		//check if the piece has arrived the board boundary => promoted King
		if ((move.PieceToMove.Color is Black && move.To.Y is 7) ||
			(move.PieceToMove.Color is White && move.To.Y is 0))
		{
			move.PieceToMove.Promoted = true;
		}
		//if it's capture move => remove the dead piece
		if (move.PieceToCapture is not null)
		{
			Board.Pieces.Remove(move.PieceToCapture);
		}
		//check if the piece can capture other pieces continuously
		if (move.PieceToCapture is not null &&
			Board.GetPossibleMoves(move.PieceToMove).Any(m => m.PieceToCapture is not null))
		{
			Board.Aggressor = move.PieceToMove;
		}
		else
		{
			Board.Aggressor = null;
			Turn = Turn is Black ? White : Black; //switch turn
		}
		CheckForWinner();
	}

	public void CheckForWinner()
	{
		if (!Board.Pieces.Any(piece => piece.Color is Black))
		{
			Winner = White;
		}
		if (!Board.Pieces.Any(piece => piece.Color is White))
		{
			Winner = Black;
		}
		if (Winner is null && Board.GetPossibleMoves(Turn).Count is 0)
		{
			Winner = Turn is Black ? White : Black;
		}
	}

	//calculate how many pieces of one color have been captured
	public int TakenCount(PieceColor colour) =>
		PiecesPerColor - Board.Pieces.Count(piece => piece.Color == colour);
}
