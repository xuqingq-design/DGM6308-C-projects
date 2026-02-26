//9: manage the player turns and check winners
namespace Checkers;

public class Game
{
	private const int PiecesPerColor = 12; //each color piece amount

	public PieceColor Turn { get; private set; }
	public Board Board { get; }
	public PieceColor? Winner { get; private set; }
	public List<Player> Players { get; }
	//NEW: all kinds of pieces has extra move after capturing one piece
	public Piece? PieceWithExtraMove { get; set; }
	public bool UsingExtraMove {get; set; } = false; //3NEW

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
	//3NEW: create a public interface to access the private set: 'Turn' and 'Winner'
	public void ResetGame()
	{
		Board.ResetBoard();
		Turn = Black;
		Winner = null;
		PieceWithExtraMove = null;
		UsingExtraMove = false;
	}

	public void PerformMove(Move move) //perform move
	{
		//update the piece position
		(move.PieceToMove.X, move.PieceToMove.Y) = move.To;
		//check if the piece has arrived the board boundary => promoted King
		if (move.PieceToMove.Type == PieceType.Normal && !move.PieceToMove.Promoted)
		{
			if ((move.PieceToMove.Color is Black && move.To.Y is 7) ||
			(move.PieceToMove.Color is White && move.To.Y is 0))
			{
				move.PieceToMove.Promoted = true;
			}
		}
		//if it's capture move => remove the dead piece
		if (move.PieceToCapture is not null)
		{
			Board.Pieces.Remove(move.PieceToCapture);
		}
		//3NEW: Fusion
		if(move.PieceToFuse is not null)
		{
			//remove the fused pieces, and return the fusion result (which piece type?)
			Board.Pieces.Remove(move.PieceToFuse);
			move.PieceToMove.Type = Board.GetFusionResult(move.PieceToMove.Type, move.PieceToFuse.Type);
			move.PieceToMove.Promoted = true;
		}
		//NEW: rewrite the logic
		//check if the piece can capture other pieces continuously
		//if the piece type is normal one (keep original rules):
		if (move.PieceToCapture is not null && move.PieceToMove.Type == PieceType.Normal &&
			Board.GetPossibleMoves(move.PieceToMove).Any(m => m.PieceToCapture is not null))
		{
			Board.Aggressor = move.PieceToMove;
			PieceWithExtraMove = null; //clear extra move
			Board.PieceWithExtraMove = null; //to board.cs
			UsingExtraMove = false;
		}
		//if the piece type is special one, give this piece one extra move (player can choose attack or withdraw)
		else if (move.PieceToCapture is not null && !UsingExtraMove && (move.PieceToMove.Type == PieceType.King || move.PieceToMove.Type == PieceType.Knight || move.PieceToMove.Type == PieceType.Rock || move.PieceToMove.Type == PieceType.Jet || move.PieceToMove.Type == PieceType.Tank || move.PieceToMove.Type == PieceType.Dragon))//make sure the first condition is precondition
		{
			Board.Aggressor = null; //don't have to capture the aggressor
			PieceWithExtraMove = move.PieceToMove;
			Board.PieceWithExtraMove = move.PieceToMove;
			UsingExtraMove = true; //3NEW: balance fixing: special pieces only have one extra move 
		}
		//the player use the extra move
		else if (PieceWithExtraMove == move.PieceToMove)
		{
			Board.Aggressor = null;
			PieceWithExtraMove = null; //only one extra move
			Board.PieceWithExtraMove = null;
			UsingExtraMove = false;
			Turn = Turn is Black ? White : Black; //switch turn
		}
		else
		{
			Board.Aggressor = null;
			PieceWithExtraMove = null;
			Board.PieceWithExtraMove = null;
			UsingExtraMove = false;
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
