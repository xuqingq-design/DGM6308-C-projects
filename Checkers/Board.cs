//8:manage the board state and all pieces
namespace Checkers;

public class Board
{
	public List<Piece> Pieces { get; } //store all 24 pieces on the board. If one is captured, remove it from List.

	public Piece? Aggressor { get; set; }
	//NEW: all kinds of pieces has extra move after capturing one piece
	public Piece? PieceWithExtraMove { get; set; }

	public Piece? this[int x, int y] =>
		Pieces.FirstOrDefault(piece => piece.X == x && piece.Y == y); //borad[3,4] return piece[3,4]; if no piece here, return null

	public Board()
	{
		Aggressor = null;
		Pieces = new List<Piece>
			{
				//NEW:
				//black pieces
				//first row (3) Normal
				new() { NotationPosition ="A3", Color = Black, Type = PieceType.Normal},
				new() { NotationPosition ="C3", Color = Black, Type = PieceType.Normal},
				new() { NotationPosition ="E3", Color = Black, Type = PieceType.Normal},
				new() { NotationPosition ="G3", Color = Black, Type = PieceType.Normal},
				//second row (2) Rock
				new() { NotationPosition ="B2", Color = Black, Type = PieceType.Rock},
				new() { NotationPosition ="D2", Color = Black, Type = PieceType.Rock},
				new() { NotationPosition ="F2", Color = Black, Type = PieceType.Rock},
				new() { NotationPosition ="H2", Color = Black, Type = PieceType.Rock},
				//third row (3) Normal+Kight
				new() { NotationPosition ="A1", Color = Black, Type = PieceType.Normal},
				new() { NotationPosition ="C1", Color = Black, Type = PieceType.Knight},
				new() { NotationPosition ="E1", Color = Black, Type = PieceType.Knight},
				new() { NotationPosition ="G1", Color = Black, Type = PieceType.Normal},
				//white pieces
				//first row (6) Normal
				new() { NotationPosition ="B6", Color = White, Type = PieceType.Normal},
				new() { NotationPosition ="D6", Color = White, Type = PieceType.Normal},
				new() { NotationPosition ="F6", Color = White, Type = PieceType.Normal},
				new() { NotationPosition ="H6", Color = White, Type = PieceType.Normal},
				//second row (7) Rock
				new() { NotationPosition ="A7", Color = White, Type = PieceType.Rock},
				new() { NotationPosition ="C7", Color = White, Type = PieceType.Rock},
				new() { NotationPosition ="E7", Color = White, Type = PieceType.Rock},
				new() { NotationPosition ="G7", Color = White, Type = PieceType.Rock},
				//third row (8) Normal+Kight
				new() { NotationPosition ="B8", Color = White, Type = PieceType.Normal},
				new() { NotationPosition ="D8", Color = White, Type = PieceType.Knight},
				new() { NotationPosition ="F8", Color = White, Type = PieceType.Knight},
				new() { NotationPosition ="H8", Color = White, Type = PieceType.Normal}

			};
	}

	public static string ToPositionNotationString(int x, int y) //transform coodinate position to string
	{
		if (!IsValidPosition(x, y)) throw new ArgumentException("Not a valid position!");
		return $"{(char)('A' + x)}{y + 1}"; //return string like "A3": x=0, y=2 => 'A' + 0 = 'A', 2+1=3 => "{'A'}{3}"="A3"
	}

	public static (int X, int Y) ParsePositionNotation(string notation)//transform string to coodinate position
	{
		if (notation is null) throw new ArgumentNullException(nameof(notation));
		notation = notation.Trim().ToUpper();
		if (notation.Length is not 2 ||
			notation[0] < 'A' || 'H' < notation[0] ||
			notation[1] < '1' || '8' < notation[1])
			throw new FormatException($@"{nameof(notation)} ""{notation}"" is not valid");
		return (notation[0] - 'A', notation[1] - '1'); //"A3"=>(0,2)
	}

	public static bool IsValidPosition(int x, int y) =>
		0 <= x && x < 8 &&
		0 <= y && y < 8; //check if the position is on the board

	public (Piece A, Piece B) GetClosestRivalPieces(PieceColor priorityColor) //find the piece and its clostest opponent's piece
	{
		double minDistanceSquared = double.MaxValue;
		(Piece A, Piece B) closestRivals = (null!, null!);
		foreach (Piece a in Pieces.Where(piece => piece.Color == priorityColor))
		{
			foreach (Piece b in Pieces.Where(piece => piece.Color != priorityColor)) //find all pieces on the board
			{
				(int X, int Y) vector = (a.X - b.X, a.Y - b.Y); //calculate the vector between two pieces
				double distanceSquared = vector.X * vector.X + vector.Y * vector.Y;
				if (distanceSquared < minDistanceSquared) //update the smallest distance
				{
					minDistanceSquared = distanceSquared;
					closestRivals = (a, b);
				}
			}
		}
		return closestRivals;
	}

	public List<Move> GetPossibleMoves(PieceColor color) //get all possible moves of one piece color
	{
		List<Move> moves = new();
		if (Aggressor is not null)
		{
			if (Aggressor.Color != color)
			{
				throw new Exception($"{nameof(Aggressor)} is not null && {nameof(Aggressor)}.{nameof(Aggressor.Color)} != {nameof(color)}");
			}
			moves.AddRange(GetPossibleMoves(Aggressor).Where(move => move.PieceToCapture is not null));
		}
		//NEW: special pieces' extra moves
		else if(PieceWithExtraMove is not null && PieceWithExtraMove.Color == color)
		{
			moves.AddRange(GetPossibleMoves(PieceWithExtraMove));
			return moves; //only return the possible moves, not mandatory
		}
		else
		{
			foreach (Piece piece in Pieces.Where(piece => piece.Color == color))
			{
				moves.AddRange(GetPossibleMoves(piece));
			}
		}
		//if a piece can be capture, must do this move
		return moves.Any(move => move.PieceToCapture is not null)
			? moves.Where(move => move.PieceToCapture is not null).ToList()
			: moves;
	}

	public List<Move> GetPossibleMoves(Piece piece) //get one piece's all possible moves
	{
		List<Move> moves = new();
		//NEW：
		switch (piece.Type)
		{
			case PieceType.Normal:
				//four diagonal moves:
				ValidateDiagonalMove(-1, -1); //left down
				ValidateDiagonalMove(-1, 1); //left up
				ValidateDiagonalMove(1, -1); //right down
				ValidateDiagonalMove(1, 1); //right up
				break;
			case PieceType.Rock:
				ValidateRockMove();
				break;
			case PieceType.Knight:
				ValidateKnightMove();
				break;
			case PieceType.King:
				ValidateKingMove();
				break;
		}

		//if a piece can be capture, must do this move
		return moves.Any(move => move.PieceToCapture is not null)
			? moves.Where(move => move.PieceToCapture is not null).ToList()
			: moves;

		void ValidateDiagonalMove(int dx, int dy) //check all four diagonal moves
		{
			//if the piece has been promoted and located at the boundary of the board
			if (!piece.Promoted && piece.Color is Black && dy is -1) return;
			if (!piece.Promoted && piece.Color is White && dy is 1) return;

			(int X, int Y) target = (piece.X + dx, piece.Y + dy); //calculate the end moving(target) position
			if (!IsValidPosition(target.X, target.Y)) return;
			PieceColor? targetColor = this[target.X, target.Y]?.Color;
			if (targetColor is null) //target position is empty => normal moving
			{
				if (!IsValidPosition(target.X, target.Y)) return;
				Move newMove = new(piece, target);
				moves.Add(newMove);
			}
			else if (targetColor != piece.Color) //capture moving
			{
				(int X, int Y) jump = (piece.X + 2 * dx, piece.Y + 2 * dy);
				if (!IsValidPosition(jump.X, jump.Y)) return;
				PieceColor? jumpColor = this[jump.X, jump.Y]?.Color;
				if (jumpColor is not null) return;
				Move attack = new(piece, jump, this[target.X, target.Y]);
				moves.Add(attack);
			}
		}
		//NEW:
		void ValidateRockMove()
		{
			CheckRockDirection(0, 1); //up
			CheckRockDirection(0, -1); //down
			CheckRockDirection(-1, 0); //left
			CheckRockDirection(1, 0); //right
			void CheckRockDirection(int moveX, int moveY)
			{
				//calculate the moving target position
				int targetX = piece.X + moveX;
				int targetY = piece.Y + moveY;
				//check if the position is in the board
				if (!IsValidPosition(targetX, targetY))
				{
					return;
				}
				//check if the target piece is empty
				Piece? targetPiece = this[targetX, targetY];
				if (targetPiece == null)
				{
					Move newMove = new Move(piece, (targetX, targetY));
					moves.Add(newMove);
				}
				else if (targetPiece.Color != piece.Color) //if there has an opponent piece
				{
					Move captureMove = new Move(piece, (targetX, targetY), targetPiece);
					moves.Add(captureMove);
				}//if there has a same color piece, do nothing
			}
		}
		void ValidateKnightMove()
		{
			CheckKnightDirection(0, 1, 1, 2); //piece check: up; self move: up-right
			CheckKnightDirection(0, 1, -1, 2); //piece check: up; self move: up-left
			CheckKnightDirection(0, -1, 1, -2); //piece check: down; self move: down-right
			CheckKnightDirection(0, -1, -1, -2); //piece check: down; self move: down-right
			CheckKnightDirection(-1, 0, -2, 1); //piece check: left; self move: left-up
			CheckKnightDirection(-1, 0, -2, -1); //piece check: left; self move: left-down
			CheckKnightDirection(1, 0, 2, 1); //piece check: right; self move: right-up
			CheckKnightDirection(1, 0, 2, -1); //piece check: right; self move: right-down
			void CheckKnightDirection(int blockX, int blockY, int moveX, int moveY)
			{
				int targetX = piece.X + moveX;
				int targetY = piece.Y + moveY;
				int blockPieceX = piece.X + blockX;
				int blockPieceY = piece.Y + blockY;
				Piece? blockPiece = this[blockPieceX, blockPieceY];
				//check if the target position is valid or there is a block piece of moving Knight
				if (!IsValidPosition(targetX, targetY) || blockPiece != null)
				{
					return;
				}
				//check if the target piece is empty
				Piece? targetPiece = this[targetX, targetY];
				if (targetPiece == null)
				{
					Move newMove = new Move(piece, (targetX, targetY));
					moves.Add(newMove);
				}
				else if (targetPiece.Color != piece.Color)
				{
					Move captureMove = new Move(piece, (targetX, targetY), targetPiece);
					moves.Add(captureMove);
				}
			}
		}
		void ValidateKingMove()
		{
			CheckKingDirection(-1, -1); // left-down
			CheckKingDirection(-1, 0); // left
			CheckKingDirection(-1, 1); // left-up
			CheckKingDirection(0, -1); // down
			CheckKingDirection(0, 1); // up
			CheckKingDirection(1, -1); // right-down
			CheckKingDirection(1, 0); // right
			CheckKingDirection(1, 1); // right-up
			void CheckKingDirection(int moveX, int moveY)
			{
				int targetX = piece.X + moveX;
				int targetY = piece.Y + moveY;
				if (!IsValidPosition(targetX, targetY))
				{
					return;
				}
				Piece? targetPiece = this[targetX, targetY];
				if (targetPiece == null)
				{
					Move newMove = new Move(piece, (targetX, targetY));
					moves.Add(newMove);
				}
				else if (targetPiece.Color != piece.Color)
				{
					Move captureMove = new Move(piece, (targetX, targetY), targetPiece);
					moves.Add(captureMove);
				}
			}
		}
	}

	/// <summary>Returns a <see cref="Move"/> if <paramref name="from"/>-&gt;<paramref name="to"/> is valid or null if not.</summary>
	//check if the moving is vaild
	public Move? ValidateMove(PieceColor color, (int X, int Y) from, (int X, int Y) to)
	{
		Piece? piece = this[from.X, from.Y]; //check if there is a piece at the start moving position
		if (piece is null)
		{
			return null;
		}
		foreach (Move move in GetPossibleMoves(color)) //find all possiblity of this piece
		{
			if ((move.PieceToMove.X, move.PieceToMove.Y) == from && move.To == to)
			{
				return move; //find vaild move and return
			}
		}
		return null;
	}

	//check if the moving is towards to a opponent's piece
	public static bool IsTowards(Move move, Piece piece)
	{
		//calculate the distance before moving
		(int Dx, int Dy) a = (move.PieceToMove.X - piece.X, move.PieceToMove.Y - piece.Y);
		int a_distanceSquared = a.Dx * a.Dx + a.Dy * a.Dy;
		//calculate the distance after moving
		(int Dx, int Dy) b = (move.To.X - piece.X, move.To.Y - piece.Y);
		int b_distanceSquared = b.Dx * b.Dx + b.Dy * b.Dy;
		//if the distance after moving is smaller, return
		return b_distanceSquared < a_distanceSquared;
	}
}
