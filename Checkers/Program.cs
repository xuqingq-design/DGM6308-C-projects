//4
Exception? exception = null; //store abnormal information

Encoding encoding = Console.OutputEncoding;

try
{
	Console.OutputEncoding = Encoding.UTF8; //to display special characters as the pieces
	Game game = ShowIntroScreenAndGetOption();
	Console.Clear();
	RunGameLoop(game); //loop until the game ends
	RenderGameState(game, promptPressKey: true); //display the final board and winner
	Console.ReadKey(true);
}
catch (Exception e)
{
	exception = e;
	throw;
}
finally
{
	Console.OutputEncoding = encoding;//restore original encoding
	Console.CursorVisible = true;
	Console.Clear();
	Console.WriteLine(exception?.ToString() ?? "Checkers was closed.");
}

Game ShowIntroScreenAndGetOption()
{
	Console.Clear();
	Console.WriteLine();
	Console.WriteLine("  Checkers");
	Console.WriteLine();
	Console.WriteLine("  Checkers is played on an 8x8 board between two sides commonly known as black");
	Console.WriteLine("  and white. The objective is simple - capture all your opponent's pieces. An");
	Console.WriteLine("  alternative way to win is to trap your opponent so that they have no valid");
	Console.WriteLine("  moves left.");
	Console.WriteLine();
	Console.WriteLine("  Black starts first and players take it in turns to move their pieces forward");
	Console.WriteLine("  across the board diagonally. Should a piece reach the other side of the board");
	Console.WriteLine("  the piece becomes a king and can then move diagonally backwards as well as");
	Console.WriteLine("  forwards.");
	Console.WriteLine();
	Console.WriteLine("  Pieces are captured by jumping over them diagonally. More than one enemy piece");
	Console.WriteLine("  can be captured in the same turn by the same piece. If you can capture a piece");
	Console.WriteLine("  you must capture a piece.");
	Console.WriteLine();
	Console.WriteLine("  Moves are selected with the arrow keys. Use the [enter] button to select the");
	Console.WriteLine("  from and to squares. Invalid moves are ignored.");
	Console.WriteLine();
	Console.WriteLine("  Press a number key to choose number of human players:");
	Console.WriteLine("    [0] Black (computer) vs White (computer)");
	Console.WriteLine("    [1] Black (human) vs White (computer)");
	Console.Write("    [2] Black (human) vs White (human)");

	int? humanPlayerCount = null;
	while (humanPlayerCount is null)
	{
		Console.CursorVisible = false;
		switch (Console.ReadKey(true).Key) //input the  amount the human players
		{
			case ConsoleKey.D0 or ConsoleKey.NumPad0: humanPlayerCount = 0; break;
			case ConsoleKey.D1 or ConsoleKey.NumPad1: humanPlayerCount = 1; break;
			case ConsoleKey.D2 or ConsoleKey.NumPad2: humanPlayerCount = 2; break;
		}
	}
	return new Game(humanPlayerCount.Value);
}

void RunGameLoop(Game game)
{
	while (game.Winner is null)
	{
		Player currentPlayer = game.Players.First(player => player.Color == game.Turn); //find current turn's player
		if (currentPlayer.IsHuman)
		{
			while (game.Turn == currentPlayer.Color)
			{
				(int X, int Y)? selectionStart = null; //position of starting
				(int X, int Y)? from = game.Board.Aggressor is not null ? (game.Board.Aggressor.X, game.Board.Aggressor.Y) : null; //if there is a Aggressor, player must use this piece
																																   //NEW: if a special piece has an extra move, must do this move
				if (from is null && game.Board.PieceWithExtraMove is not null)
				{
					from = (game.Board.PieceWithExtraMove.X, game.Board.PieceWithExtraMove.Y); //select this special piece automatically
				}

				List<Move> moves = game.Board.GetPossibleMoves(game.Turn); //get all posiblility of moves
				if (moves.Select(move => move.PieceToMove).Distinct().Count() is 1) //if only one piece can move
				{
					Move must = moves.First();
					from = (must.PieceToMove.X, must.PieceToMove.Y); //select this piece automatically
					selectionStart = must.To; //moves the cursor here
				}
				while (from is null) //if the player hasn't select
				{
					from = HumanMoveSelection(game); //enable the player to use arrow key to select
					selectionStart = from;
				}
				(int X, int Y)? to = HumanMoveSelection(game, selectionStart: selectionStart, from: from); //the moving end position
				Piece? piece = null;
				piece = game.Board[from.Value.X, from.Value.Y];
				if (piece is null || piece.Color != game.Turn) //if the selected piece isn't conform to rules  
				{
					from = null; //selection become invaild
					to = null;
				}
				if (from is not null && to is not null) //if the selection(start and end) is vaild
				{
					Move? move = game.Board.ValidateMove(game.Turn, from.Value, to.Value); //check if the move is valid
					if (move is not null &&
						(game.Board.Aggressor is null || move.PieceToMove == game.Board.Aggressor))
					{

						//3NEW: confirm the fusion Y/N
						bool turnProceed = true;
						if (move.PieceToFuse is not null)
						{
							turnProceed = ShowFusionConfirmation(game, move);
						}

						//NEW: check if the piece is going to arrive at the edge of the board => get promoted
						if (turnProceed)
						{
							bool piecePromote = false;
							if (move.PieceToMove.Type == PieceType.Normal && !move.PieceToMove.Promoted) //only normal piece can be promoted
							{
								if (move.PieceToMove.Color is Black && move.To.Y == 7)
								{
									piecePromote = true;
								}
								else if (move.PieceToMove.Color is White && move.To.Y == 0)
								{
									piecePromote = true;
								}
							}
							game.PerformMove(move);
							if (piecePromote)
							{
								//let the player choose the promote type of this piece
								PieceType piecePromoteSelectionType = promoteSelection(game, move.PieceToMove);
								move.PieceToMove.Type = piecePromoteSelectionType;
								move.PieceToMove.Promoted = true;
								//clear the console and re-render the board
								Console.Clear();
								RenderGameState(game, playerMoved: currentPlayer, promptPressKey: false);
							}
							//3NEW:
							if (move.PieceToFuse is not null)
							{
								Console.Clear();
								RenderGameState(game, playerMoved: currentPlayer, promptPressKey: false);
							}
						}
					}
				}
			}
		}
		else //computer's turn
		{
			List<Move> moves = game.Board.GetPossibleMoves(game.Turn);
			List<Move> captures = moves.Where(move => move.PieceToCapture is not null).ToList();
			List<Move> fusionMoves = moves.Where(move => move.PieceToFuse is not null).ToList();

			//NEW: define selectedMove
			Move? selectedMove = null;

			//3NEW: fusion moves first
			if (fusionMoves.Count > 0)
			{
				selectedMove = fusionMoves[Random.Shared.Next(fusionMoves.Count)];
			}
			else if (captures.Count > 0) //if pieces can capture other opponent's pieces
			{
				selectedMove = captures[Random.Shared.Next(captures.Count)]; //randomly select a piece to capture
			}
			else if (!game.Board.Pieces.Any(piece => piece.Color == game.Turn && !piece.Promoted)) //if all the pieces has been promoted
			{
				var (a, b) = game.Board.GetClosestRivalPieces(game.Turn); //move to the closest opponent's piece
				Move? priorityMove = moves.FirstOrDefault(move => move.PieceToMove == a && Board.IsTowards(move, b));
				selectedMove = priorityMove ?? moves[Random.Shared.Next(moves.Count)];
			}
			else
			{
				selectedMove = moves[Random.Shared.Next(moves.Count)]; //random move
			}
			//NEW: if the next select move will let one piece get promoted
			bool pieceComPromote = false;
			if (selectedMove.PieceToMove.Type == PieceType.Normal && !selectedMove.PieceToMove.Promoted)
			{
				if (selectedMove.PieceToMove.Color is Black && selectedMove.To.Y == 7)
				{
					pieceComPromote = true;
				}
				else if (selectedMove.PieceToMove.Color is White && selectedMove.To.Y == 0)
				{
					pieceComPromote = true;
				}
			}
			game.PerformMove(selectedMove);
			if (pieceComPromote)
			{
				//define an array to store all piece types
				PieceType[] promoteOptions = { PieceType.Rock, PieceType.Knight, PieceType.King };
				//computer select one type to promote randomly
				selectedMove.PieceToMove.Type = promoteOptions[Random.Shared.Next(0, promoteOptions.Length)];
				selectedMove.PieceToMove.Promoted = true;
			}
		}

		RenderGameState(game, playerMoved: currentPlayer, promptPressKey: true); //render the state after moving
		Console.ReadKey(true);
	}
}

//3NEW: display fusion confirmation
bool ShowFusionConfirmation(Game game, Move move)
{
	//get the result of fusion
	PieceType resultType = Board.GetFusionResult(move.PieceToMove.Type, move.PieceToFuse!.Type);

	Console.Clear();
	Console.WriteLine();
	Console.WriteLine("  ╔═══════════════════════════════════════════════════╗");
	Console.WriteLine("  ║            FUSION CONFIRMATION                    ║");
	Console.WriteLine("  ╚═══════════════════════════════════════════════════╝");
	Console.WriteLine();
	Console.WriteLine($"  Fusion:");
	Console.WriteLine($"    Type1: {move.PieceToMove.Type}");
	Console.WriteLine($"    +");
	Console.WriteLine($"    Type2: {move.PieceToFuse.Type}");
	Console.WriteLine($"    =");
	Console.WriteLine($"  Result: {resultType}");
	Console.WriteLine();
	// List all possiblilities of fusion results, and check which this result would be? Then display its way of moving
	switch (resultType)
	{
		case PieceType.Jet:
			Console.WriteLine("  Jet moves:");
			Console.WriteLine("  Moves diagonally 2 squares");
			break;
		case PieceType.Tank:
			Console.WriteLine("  Tank moves:");
			Console.WriteLine("  Moves straight 2 squares");
			break;
		case PieceType.King:
			Console.WriteLine("  King moves:");
			Console.WriteLine("  Moves in all 8 directions (1 square)");
			break;
		case PieceType.Dragon:
			Console.WriteLine("  Dragon moves:");
			Console.WriteLine("  Moves in all 8 directions (2 squares)");
			break;
	}
	// confirm the fusion: Y/N
	Console.WriteLine();
	Console.WriteLine("  Do you want to proceed with this fusion?");
	Console.WriteLine("  Input [Y]/[N] ");
	while (true)
	{
		Console.CursorVisible = false;
		ConsoleKey key = Console.ReadKey(true).Key;
		switch (key)
		{
			case ConsoleKey.Y:
			    return true;
			case ConsoleKey.N:
			    return false;
			default:
			    continue;
		}
	}
}

//NEW: display the promotion menu (WriteLine text format is adjusted by AI)
PieceType promoteSelection(Game game, Piece piece)
{
	Console.Clear();
	Console.WriteLine();
	Console.WriteLine("  ╔═══════════════════════════════════════╗");
	Console.WriteLine("  ║     PROMOTION - Choose Piece Type     ║");
	Console.WriteLine("  ╚═══════════════════════════════════════╝");
	Console.WriteLine();
	Console.WriteLine($"  Your {piece.Color} piece has reached the end!");
	Console.WriteLine("  Choose what to promote it to:");
	Console.WriteLine();
	Console.WriteLine("  [1] Rock   - Moves straight (horizontal/vertical), 1 square");
	Console.WriteLine("              Gets extra move after capturing");
	Console.WriteLine();
	Console.WriteLine("  [2] Knight - Moves in L-shape");
	Console.WriteLine("              Can be blocked by hobbling horse");
	Console.WriteLine("              Gets extra move after capturing");
	Console.WriteLine();
	Console.WriteLine("  [3] King   - Moves in all 8 directions, 1 square");
	Console.WriteLine("              Gets extra move after capturing");
	Console.WriteLine();
	Console.Write("  Enter your choice (1-3): ");

	while (true)
	{
		Console.CursorVisible = false;
		ConsoleKey key = Console.ReadKey(true).Key;
		switch (key)
		{
			case ConsoleKey.D1:
				Console.WriteLine(" [1] - Rock ");
				return PieceType.Rock;
			case ConsoleKey.D2:
				Console.WriteLine(" [2] - Knight ");
				return PieceType.Knight;
			case ConsoleKey.D3:
				Console.WriteLine(" [3] - King ");
				return PieceType.King;
			default:
				Console.WriteLine(" invalid input, please try again");
				continue;
		}
	}
}

void RenderGameState(Game game, Player? playerMoved = null, (int X, int Y)? selection = null, (int X, int Y)? from = null, bool promptPressKey = false)
{
	const char BlackNormal = '○';
	const char BlackRock = '□'; //NEW
	const char BlackKnight = '△'; //NEW 
	const char BlackKing = '☺';
	const char BlackJet = '◁';
	const char BlackTank = '▽';
	const char BlackDragon = '☆';
	const char WhiteNormal = '●';
	const char WhiteRock = '■'; //NEW
	const char WhiteKnight = '▲'; //NEW 
	const char WhiteKing = '☻';
	const char WhiteJet = '◀';
	const char WhiteTank = '▼';
	const char WhiteDragon = '★';
	const char Vacant = '·';

	Console.CursorVisible = false;
	Console.Clear();
	StringBuilder sb = new();
	sb.AppendLine();
	sb.AppendLine("  Checkers");
	sb.AppendLine();
	sb.AppendLine($"    ╔═══════════════════╗");
	sb.AppendLine($"  8 ║  {B(0, 7)} {B(1, 7)} {B(2, 7)} {B(3, 7)} {B(4, 7)} {B(5, 7)} {B(6, 7)} {B(7, 7)}  ║ Black: {BlackNormal}Black Normal {BlackRock}Black Rock {BlackJet}Black Jet {BlackTank}Black Tank");
	sb.AppendLine($"  7 ║  {B(0, 6)} {B(1, 6)} {B(2, 6)} {B(3, 6)} {B(4, 6)} {B(5, 6)} {B(6, 6)} {B(7, 6)}  ║        {BlackKnight}Black Knight {BlackKing}Black King {BlackRock}Black Rock {BlackDragon}Black Dragon");
	sb.AppendLine($"  6 ║  {B(0, 5)} {B(1, 5)} {B(2, 5)} {B(3, 5)} {B(4, 5)} {B(5, 5)} {B(6, 5)} {B(7, 5)}  ║ White: {WhiteNormal}White Normal {WhiteRock}White Rock {WhiteJet}White Jet {WhiteTank}White Tank");
	sb.AppendLine($"  5 ║  {B(0, 4)} {B(1, 4)} {B(2, 4)} {B(3, 4)} {B(4, 4)} {B(5, 4)} {B(6, 4)} {B(7, 4)}  ║        {WhiteKnight}White Knight {WhiteKing}White King {WhiteRock}White Rock {WhiteDragon}White Dragon");
	sb.AppendLine($"  4 ║  {B(0, 3)} {B(1, 3)} {B(2, 3)} {B(3, 3)} {B(4, 3)} {B(5, 3)} {B(6, 3)} {B(7, 3)}  ║");
	sb.AppendLine($"  3 ║  {B(0, 2)} {B(1, 2)} {B(2, 2)} {B(3, 2)} {B(4, 2)} {B(5, 2)} {B(6, 2)} {B(7, 2)}  ║ Taken:");
	sb.AppendLine($"  2 ║  {B(0, 1)} {B(1, 1)} {B(2, 1)} {B(3, 1)} {B(4, 1)} {B(5, 1)} {B(6, 1)} {B(7, 1)}  ║ {game.TakenCount(White),2} x {White}");
	sb.AppendLine($"  1 ║  {B(0, 0)} {B(1, 0)} {B(2, 0)} {B(3, 0)} {B(4, 0)} {B(5, 0)} {B(6, 0)} {B(7, 0)}  ║ {game.TakenCount(Black),2} x {Black}");
	sb.AppendLine($"    ╚═══════════════════╝");
	sb.AppendLine($"       A B C D E F G H");
	sb.AppendLine();
	if (selection is not null)
	{
		sb.Replace(" $ ", $"[{ToChar(game.Board[selection.Value.X, selection.Value.Y])}]");
	}
	if (from is not null)
	{
		char fromChar = ToChar(game.Board[from.Value.X, from.Value.Y]);
		sb.Replace(" @ ", $"<{fromChar}>");
		sb.Replace("@ ", $"{fromChar}>");
		sb.Replace(" @", $"<{fromChar}");
	}
	PieceColor? wc = game.Winner;
	PieceColor? mc = playerMoved?.Color;
	PieceColor? tc = game.Turn;
	// Note: these strings need to match in length
	// so they overwrite each other.
	string w = $"  *** {wc} wins ***          ";
	string m = $"  {mc} moved                 ";
	string t = $"  {tc}'s turn                ";
	sb.AppendLine(
		game.Winner is not null ? w :
		playerMoved is not null ? m :
		t); //display game state
	string p = "  Press any key to continue...";
	string s = "                              ";
	sb.AppendLine(promptPressKey ? p : s);
	Console.Write(sb);

	char B(int x, int y) =>
		(x, y) == selection ? '$' :
		(x, y) == from ? '@' :
		ToChar(game.Board[x, y]); //use different character to express the start/end selection

	static char ToChar(Piece? piece) =>
		piece is null ? Vacant : //if piece is null, display nothing
								 //NEW: use piece.Type to determine the piece type
		(piece.Color, piece.Type) switch //use two variable to check the piece's state
		{
			(Black, PieceType.Normal) => BlackNormal,
			(Black, PieceType.Rock) => BlackRock,
			(Black, PieceType.Knight) => BlackKnight,
			(Black, PieceType.King) => BlackKing,
			(Black, PieceType.Jet) => BlackJet,
			(Black, PieceType.Tank) => BlackTank,
			(Black, PieceType.Dragon) => BlackDragon,
			(White, PieceType.Normal) => WhiteNormal,
			(White, PieceType.Rock) => WhiteRock,
			(White, PieceType.Knight) => WhiteKnight,
			(White, PieceType.King) => WhiteKing,
			(White, PieceType.Jet) => WhiteJet,
			(White, PieceType.Tank) => WhiteTank,
			(White, PieceType.Dragon) => WhiteDragon,
			_ => throw new NotImplementedException(),
		};
}

(int X, int Y)? HumanMoveSelection(Game game, (int X, int y)? selectionStart = null, (int X, int Y)? from = null)
{
	(int X, int Y) selection = selectionStart ?? (3, 3); //default start selection position
	while (true)
	{
		RenderGameState(game, selection: selection, from: from);
		switch (Console.ReadKey(true).Key)
		{
			case ConsoleKey.DownArrow: selection.Y = Math.Max(0, selection.Y - 1); break;
			case ConsoleKey.UpArrow: selection.Y = Math.Min(7, selection.Y + 1); break;
			case ConsoleKey.LeftArrow: selection.X = Math.Max(0, selection.X - 1); break;
			case ConsoleKey.RightArrow: selection.X = Math.Min(7, selection.X + 1); break;
			case ConsoleKey.Enter: return selection;
			case ConsoleKey.Escape: return null;
			//3NEW: reset board
			case ConsoleKey.R:
				if (ShowResetConfirmation())
				{
					game.ResetGame();
					Console.Clear();
				}
				break;
		}
	}
}

//3NEW: Reset confirmation
bool ShowResetConfirmation()
{
	Console.Clear();
	Console.WriteLine();
	Console.WriteLine("  This will reset the board to the initial state.");
	Console.WriteLine("  Are you sure?");
	Console.WriteLine("  Input [Y]/[N]");
	while (true)
	{
		Console.CursorVisible = false;
		ConsoleKey key = Console.ReadKey(true).Key;
		switch (key)
		{
			case ConsoleKey.Y:
				return true;
			case ConsoleKey.N:
				return false;
			default:
				continue;
		}
	}
}
