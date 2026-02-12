﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

bool closeRequested = false; //check if the player is going to quit the game
State state = State.Main; //State--type: enum(shows in the end of the code), define a variable state
Stopwatch stopwatch = new(); //Stopwatch: use to measure passed time(timer)
TimeSpan framerate = TimeSpan.FromSeconds(1d / 60d);//set framerate to 1/60 second
bool direction = default; //boolean: default=false
						  //Delete: bool playerGoesFirst = default;
int x = 0;
int y = 0;
int x_max = 38;
int y_max = 14;
List<((int X, int Y)? Position, int PlayerIndex)> darts = new(); //NEW: int playerIndex //define a list: darts includes (int X, int Y)?: Position and bool: Player; ?: nullable (before the player throw darts, the result may be null); 
																 //bool Player: true(player's dart), false(computer's dart)
int computer_x = default;
int computer_y = default;
int computer_skip = default;
//NEW:
int playerCount = 1; //default player amount
int[] score = new int[4]; //create an array which can store 4 integer
bool[] isHuman = new bool[4];
int currentPlayerIndex = 0; //currently who's turn
int totalRounds = 5;
int[] dartsThrown = new int[4];
bool reductiveMode = false;

try
{
	Console.CursorVisible = false;
	Console.OutputEncoding = Encoding.UTF8;
	while (!closeRequested)
	{
		Render();
		Update();
	}
}
finally
{
	Console.CursorVisible = true;
	Console.Clear();
	Console.WriteLine("Darts was closed.");
}

void Update()
{
	switch (state) //branch based on variable: state
	{
		case State.Main: //when state == State.Main
			PressEnterToContinue();
			if (closeRequested)
			{
				return;
			}
			//NEW:Delete: playerGoesFirst = Random.Shared.Next(0, 2) % 2 is 0; //50% playerGoesFirst=true
			state = State.Setup;
			Console.Clear();
			break;

		//NEW:setup menu
		case State.Setup:
			Console.Clear();
			Console.WriteLine("  How many players?(enter:1-4)  ");
			while (true)
			{
				var key = Console.ReadKey(true); //when player press a key, store it in an object: key
				if (key.Key == ConsoleKey.Escape) //key.Key: what's the key is--A\B\D1\D2...
				{
					closeRequested = true;
					return;
				}
				if (key.KeyChar >= '1' && key.KeyChar <= '4') //key.KeyChar: the character of this key--'a'\'b'\'1'\'2'
				{
					string input = key.KeyChar.ToString();
					int.TryParse(input, out int output);
					playerCount = output;
					//another way: playerCount = key.KeyChar - '0'; //ASCII: '2'-'0' = 50 - 48 = 2
					break;
				}
			}
			Console.Clear();
			Console.WriteLine("  Select game mode:  ");
			Console.WriteLine("  1.Default Mode  ");
			Console.WriteLine("  2.Reductive Mode  ");
			Console.WriteLine("  Enter 1 / 2  ");
			while (true)
			{
				var key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.Escape)
				{
					closeRequested = true;
					return;
				}
				if (key.KeyChar == '1')
				{
					reductiveMode = false;
					break;
				}
				else if (key.KeyChar == '2')
				{
					reductiveMode = true;
					break;
				}
			}
			//determine there are how many human players
			for (int i = 0; i < 4; i++)
			{
				dartsThrown[i] = 0;
				if (i < playerCount)
				{
					isHuman[i] = true;
				}
				else
				{
					isHuman[i] = false;
				}
				//reset all the players' data
				if (reductiveMode)
				{
					score[i] = 50;
				}
				else
				{
					score[i] = 0;
				}
			}
			currentPlayerIndex = Random.Shared.Next(0, 4);//randomly choose one player to begin first
			state = State.ConfirmRandomTurnOrder;
			Console.Clear();
			break;

		case State.ConfirmRandomTurnOrder:
			PressEnterToContinue();
			if (closeRequested)
			{
				return;
			}
			//NEW:Delete: state = playerGoesFirst 
			//NEW:Delete: ? State.PlayerHorizontal //if playerGoesFirst=true, state=State.PlayerHorizontal--player turn
			//NEW:Delete: : State.ComputerHorizontal; //if playerGoesFirst=false, computer turn
			//NEW:
			if (isHuman[currentPlayerIndex])
			{
				state = State.PlayerHorizontal;
			}
			else
			{
				state = State.ComputerHorizontal;
			}
			direction = true;
			x = 0;
			if (!isHuman[currentPlayerIndex]) //NEW:Delete: if (!playerGoesFirst)
			{
				computer_x = Random.Shared.Next(x_max + 1);
				computer_skip = Random.Shared.Next(1, 4); //computer_skip: after 1-3 flame delay to throw again vertically
			}
			stopwatch.Restart();
			Console.Clear();
			break;

		case State.ConfirmPlayerThrow:
			PressEnterToContinue();
			if (closeRequested)
			{
				return;
			}
			//NEW:
			dartsThrown[currentPlayerIndex]++;
			bool gameEnd = false;
			if (reductiveMode)
			{
				if (score[currentPlayerIndex] == 0)
				{
					gameEnd = true;
				}
			}
			else
			{
				gameEnd = true;
				for (int i = 0; i < 4; i++)
				{
					if (dartsThrown[i] < totalRounds) //only when all players' dartsThrown times>=5, game ends; if dartsThrown[1]=5, dartsThrown[2]=4, gameEnd = false, continue
					{
						gameEnd = false;
						break;
					}
				}
			}
			if (gameEnd)
			{
				state = State.ConfirmGameEnd;
				Console.Clear();
				break;
			}
			//Delete: if (darts.Count >= 10) //darts total throw times
			//{
			//state = State.ConfirmGameEnd;
			//Console.Clear();
			//break;
			//}
			currentPlayerIndex = (currentPlayerIndex + 1) % 4; //one player's round ends, switching to the next player
															   //Delete: state = State.ComputerHorizontal;
			CheckHumanPlayer();
			break;

		case State.ConfirmComputerThrow:
			PressEnterToContinue();
			if (closeRequested)
			{
				return;
			}
			//Delete: if (darts.Count >= 10)
			//{
			//state = State.ConfirmGameEnd;
			//Console.Clear();
			//break;
			//}
			//NEW:
			dartsThrown[currentPlayerIndex]++;
			bool gameEndComputer = false;
			if (reductiveMode)
			{
				if (score[currentPlayerIndex] == 0)
				{
					gameEndComputer = true;
				}
			}
			else
			{
				gameEndComputer = true;
				for (int i = 0; i < 4; i++)
				{
					if (dartsThrown[i] < totalRounds)
					{
						gameEndComputer = false;
						break;
					}
				}
			}
			if (gameEndComputer)
			{
				state = State.ConfirmGameEnd;
				Console.Clear();
				break;
			}
			currentPlayerIndex = (currentPlayerIndex + 1) % 4;
			CheckHumanPlayer();
			break;

		case State.PlayerHorizontal or State.ComputerHorizontal: //when the  aiming is moving horizontally
			if (KeyPressed() && state is State.PlayerHorizontal)
			{
				if (closeRequested)
				{
					return;
				}
				state = State.PlayerVertical;
				direction = true;
				y = 0;
				stopwatch.Restart();
				Console.Clear();
				break;
			}
			if (closeRequested)
			{
				return;
			}
			if (direction) //direction:true--aiming point moves to right
			{
				x++;
			}
			else
			{
				x--;
			}
			if (state is State.ComputerHorizontal && x == computer_x)
			{
				computer_skip--;
				if (computer_skip < 0)
				{
					state = State.ComputerVertical;
					direction = true;
					y = 0;
					stopwatch.Restart();
					computer_y = Random.Shared.Next(y_max + 1);
					computer_skip = Random.Shared.Next(1, 4);
				}
			}
			if (x <= 0 || x >= x_max) //boundary limit
			{
				if (x < 0) x = 0;
				if (x > x_max) x = x_max;
				direction = !direction;
			}
			ControlFrameRate();
			break;

		case State.PlayerVertical or State.ComputerVertical:
			if (KeyPressed() && state is State.PlayerVertical)
			{
				if (closeRequested)
				{
					return;
				}
				state = State.ConfirmPlayerThrow;
				(int X, int Y)? position = (x, y); //record the result and stores in position
				for (int i = 0; i < darts.Count; i++) //check all the result of throwing darts
				{
					if (darts[i].Position == (x, y)) //if there has been a result == current rusult(x,y)
					{
						darts[i] = (null, darts[i].PlayerIndex); //clear the Position(previous result), but remain Player(who throw dart)
						position = null; //clear current result
					}
				}
				darts.Add(new(position, currentPlayerIndex)); //add this new result after calculation to the darts
				Console.Clear();
				break;
			}
			if (closeRequested)
			{
				return;
			}
			if (direction)
			{
				y++;
			}
			else
			{
				y--;
			}
			if (state is State.ComputerVertical && y == computer_y)
			{
				computer_skip--;
				if (computer_skip < 0)
				{
					state = State.ConfirmComputerThrow;
					(int X, int Y)? position = (x, y);
					for (int i = 0; i < darts.Count; i++)
					{
						if (darts[i].Position == (x, y))
						{
							darts[i] = (null, darts[i].PlayerIndex);
							position = null;
						}
					}
					darts.Add(new(position, currentPlayerIndex));
					Console.Clear();
					break;
				}
			}
			if (y <= 0 || y >= y_max)
			{
				if (y < 0) y = 0;
				if (y > y_max) y = y_max;
				direction = !direction;
			}
			ControlFrameRate();
			break;

		case State.ConfirmGameEnd:
			PressEnterToContinue();
			if (closeRequested)
			{
				return;
			}
			state = State.Main;
			darts = new();
			break;

		default:
			throw new NotImplementedException();

	}
}

void ControlFrameRate() //stabilize the time length of each flame
{
	TimeSpan elapsed = stopwatch.Elapsed;//currently elapsed(passed) time
	if (framerate > elapsed)
	{
		Thread.Sleep(framerate - elapsed);
	}
	stopwatch.Restart();
}

void PressEnterToContinue()
{
	while (true)
	{
		switch (Console.ReadKey(true).Key)
		{
			case ConsoleKey.Enter: return;
			case ConsoleKey.Escape: closeRequested = true; return;
		}
	}
}

bool KeyPressed()
{
	bool keyPressed = false;
	while (Console.KeyAvailable) //if press any key
	{
		keyPressed = true;
		if (Console.ReadKey(true).Key is ConsoleKey.Escape)
		{
			closeRequested = true;
		}
	}
	return keyPressed;
}

void Render()
{
	var render = new StringBuilder();

	if (state is State.Main)
	{
		StringBuilder output = new(); //StringBuilder: splicable string, use to join lots of strings
		output.AppendLine();
		output.AppendLine("  Darts"); //Append: add the following strings behind output
		output.AppendLine();
		output.AppendLine("  Welcome to Darts. In this game you and the computer will");
		output.AppendLine("  throw darts at a dart board in attempts to get the most ");
		output.AppendLine("  points. If your dart lands on a line, it will round down");
		output.AppendLine("  amongst all the regions it is touching. If your dart lands");
		output.AppendLine("  on another dart it will knock both darts off the board so");
		output.AppendLine("  they will each be worth 0 points. You and the computer each");
		output.AppendLine("  get to throw 5 darts.");
		output.AppendLine();
		output.AppendLine("  Your darts: ○");
		output.AppendLine("  Computer's darts: ●");
		output.AppendLine();
		output.AppendLine("  Press [escape] at any time to close the game.");
		output.AppendLine();
		output.Append("  Press [enter] to begin...");
		Console.Clear();
		Console.Write(output); //record all the strings before and output them in one time; use to reduce blink
		return;
	}

	string[] board =
	[
		"╔═══════╤═══════╤═══════╤═══════╤═══════╗",
		"║       │       │       │       │       ║",
		"║   1   │   2   │   3   │   2   │   1   ║",
		"║      ┌┴┐    ┌─┴─┐   ┌─┴─┐    ┌┴┐      ║",
		"╟──────┤6├────┤ 5 ├───┤ 5 ├────┤6├──────╢",
		"║      └┬┘    └─┬─┘   └─┬─┘    └┬┘      ║",
		"║   2   │   3   │   4   │   3   │   2   ║",
		"║       │       │  ┌─┐  │       │       ║",
		"╟───────┼───────┼──┤9├──┼───────┼───────╢",
		"║       │       │  └─┘  │       │       ║",
		"║   2   │   3   │   4   │   3   │   2   ║",
		"║      ┌┴┐    ┌─┴─┐   ┌─┴─┐    ┌┴┐      ║",
		"╟──────┤6├────┤ 5 ├───┤ 5 ├────┤6├──────╢",
		"║      └┬┘    └─┬─┘   └─┬─┘    └┬┘      ║",
		"║   1   │   2   │   3   │   2   │   1   ║",
		"║       │       │       │       │       ║",
		"╚═══════╧═══════╧═══════╧═══════╧═══════╝",
	];
	for (int i = 0; i < board.Length; i++) //traverse all lines
	{
		for (int j = 0; j < board[i].Length; j++) //traverse all columns
		{
			foreach (var dart in darts) //traverse all thrown darts
			{
				if (dart.Position == (j - 1, i - 1)) //if a dart is on the board, render it
				{
					render.Append(isHuman[dart.PlayerIndex] ? '○' : '●'); //NEW: check this dart is thrown by player/computer--true:'○'
					goto DartRendered;
				}
			}
			render.Append(board[i][j]); //if not just draw the board
		DartRendered:
			continue; //run the code behind this loop(for (int j = 0; ...)---run: if (dart.Position...
		}
		if (state is State.PlayerHorizontal or State.PlayerVertical or State.ComputerHorizontal or State.ComputerVertical or State.ConfirmPlayerThrow or State.ConfirmComputerThrow)
		{
			render.Append(' '); //if the dart hasn't been thrown, do not render the dart
			if (i - 1 == y && state is not State.PlayerHorizontal and not State.ComputerHorizontal)
			{
				render.Append("│██│");
			}
			else
			{
				render.Append(i is 0 ? "┌──┐" : i == board.Length - 1 ? "└──┘" : "│  │");
			}
		}
		render.AppendLine();
	}

	//NEW: display score 
	CalculateScores(); //Delete: var (playerScore, computerScore) = 
	render.AppendLine();
	render.AppendLine("  Scores:  ");
	//NEW: display different players' score
	for (int i = 0; i < 4; i++)
	{
		if (isHuman[i])
		{
			render.AppendLine($"  Player[{i + 1}]: {score[i]}  ( {dartsThrown[i]} / {totalRounds} )  ");
		}
		else
		{
			render.AppendLine($"  Computer[{i + 1}]: {score[i]}  ( {dartsThrown[i]} / {totalRounds} )  ");
		}
	}

	if (state is State.PlayerHorizontal or State.PlayerVertical or State.ComputerHorizontal or State.ComputerVertical or State.ConfirmPlayerThrow or State.ConfirmComputerThrow)
	{
		render.AppendLine("┌───────────────────────────────────────┐");
		for (int j = 0; j <= x_max + 2; j++)
		{
			render.Append(
				j - 1 == x ? '█' :
				j is 0 ? '│' :
				j == x_max + 2 ? '│' :
				' ');
		}
		render.AppendLine();
		render.AppendLine("└───────────────────────────────────────┘");
	}
	if (state is State.PlayerHorizontal or State.PlayerVertical)
	{
		render.AppendLine();
		render.AppendLine($"  It is Player[{currentPlayerIndex + 1}]'s turn."); //NEW
		render.Append("  Press any key to aim your ○ dart... ");
	}
	if (state is State.ComputerHorizontal or State.ComputerVertical)
	{
		render.AppendLine();
		render.Append($"  Computer[{currentPlayerIndex + 1}]'s turn. Wait for it to throw it's ● dart."); //NEW
	}
	if (state is State.ConfirmRandomTurnOrder)
	{
		//NEW:decide first player
		string firstPlayer = "";
		if (isHuman[currentPlayerIndex])
		{
			firstPlayer = $"Player[{currentPlayerIndex + 1}]";
		}
		else
		{
			firstPlayer = $"Computer[{currentPlayerIndex + 1}]";
		}
		render.AppendLine();
		render.AppendLine("  Flip a coin and decide that ");
		render.AppendLine($"  {firstPlayer} will go first."); //NEW
		render.AppendLine();
		render.Append("  Press [enter] to continue...");
	}
	if (state is State.ConfirmPlayerThrow)
	{
		render.AppendLine();
		render.AppendLine($"  Player[{currentPlayerIndex + 1}] threw a dart.");
		if (darts[^1].Position is null)
		{
			render.AppendLine();
			render.AppendLine("  Dart collision! Both darts fell off the board.");
		}
		render.AppendLine();
		render.Append("  Press [enter] to continue...");
	}
	if (state is State.ConfirmComputerThrow)
	{
		render.AppendLine();
		render.AppendLine($"  Computer[{currentPlayerIndex + 1}] threw a dart.");
		if (darts[^1].Position is null)
		{
			render.AppendLine();
			render.AppendLine("  Dart collision! Both darts fell off the board.");
		}
		render.AppendLine();
		render.Append("  Press [enter] to continue...");
	}
	if (state is State.ConfirmGameEnd) //current score display
	{
		//NEW:Delete: var (playerScore, computerScore) = CalculateScores();
		render.AppendLine();
		render.AppendLine("  Game Complete! Final Scores...");
		//NEW:display everyone's score
		for (int i = 0; i < 4; i++)
		{
			if (isHuman[i])
			{
				render.AppendLine($"  Player[{i + 1}]: {score[i]}  ( {dartsThrown[i]} / {totalRounds} )  ");
			}
			else
			{
				render.AppendLine($"  Computer[{i + 1}]: {score[i]}  ( {dartsThrown[i]} / {totalRounds} )  ");
			}
		}
		render.AppendLine();

		//NEW: conform the winner
		int winnerIndex = -1;
		int bestScore;
		if (reductiveMode)
		{
			for (int i = 0; i < 4; i++)
			{
				if (score[i] == 0)
				{
					winnerIndex = i;
					break;
				}
			}
			if (winnerIndex == -1)
			{
				bestScore = int.MaxValue; //define the largest int(about 2.1Billon)
				for (int i = 0; i < 4; i++)
				{
					if (score[i] < bestScore) //if next player's score < bestScore, it will be bestScore; if larger, the previous one will still be bestScore... at last, bestScore becomes the winner
					{
						bestScore = score[i];
						winnerIndex = i;
					}
				}
			}
		}
		else
		{
			bestScore = int.MinValue;//about -2.1Billon
			for (int i = 0; i < 4; i++)
			{
				if (score[i] > bestScore)
				{
					bestScore = score[i];
					winnerIndex = i;
				}
			}
		}
		if (isHuman[winnerIndex])
		{
			render.AppendLine($"  Player[{winnerIndex + 1}] wins  ");
		}
		else
		{
			render.AppendLine($"  Computer[{winnerIndex + 1}] wins  ");
		}

		render.AppendLine();
		render.Append("  Press [enter] to return to the main screen...");
	}

	Console.CursorVisible = false;
	Console.Clear(); //NEW
	Console.SetCursorPosition(0, 0);
	Console.Write(render);
}

void CalculateScores()
{
	string[] scoreBoard =
	[
		"111111112222222233333332222222211111111",
		"111111112222222233333332222222211111111",
		"111111112222222233333332222222211111111",
		"111111162222225553333355522222261111111",
		"222222223333333344444443333333322222222",
		"222222223333333344444443333333322222222",
		"222222223333333344444443333333322222222",
		"222222223333333344494443333333322222222",
		"222222223333333344444443333333322222222",
		"222222223333333344444443333333322222222",
		"222222223333333344444443333333322222222",
		"111111162222225553333355522222261111111",
		"111111112222222233333332222222211111111",
		"111111112222222233333332222222211111111",
		"111111112222222233333332222222211111111",
	];

	//NEW: reset the scores
	if (reductiveMode)
	{
		for (int i = 0; i < 4; i++)
		{
			score[i] = 50;
		}
	}
	else
	{
		for (int i = 0; i < 4; i++)
		{
			score[i] = 0;
		}
	}

	//NEW: calculate score when hit the board
	foreach (var dart in darts)
	{
		if (dart.Position.HasValue)
		{
			int dartScore = scoreBoard[dart.Position.Value.Y][dart.Position.Value.X] - '0'; 
			int playerIndex = dart.PlayerIndex;
			if (reductiveMode)
			{
				int newScore = score[playerIndex] - dartScore; //if newScore < 0, player's score won't be update, until newScore>=0
				if (newScore >= 0)
				{
					score[playerIndex] = newScore;
				}
			}
			else
			{
				score[playerIndex] += dartScore;
			}
		}
	}
}

void CheckHumanPlayer()
{
	if (isHuman[currentPlayerIndex])
	{
		state = State.PlayerHorizontal;
	}
	else
	{
		state = State.ComputerHorizontal;
	}
	//as same as: state = isHuman[currentPlayerIndex]
	//		          ? State.PlayerHorizontal
	//		          : State.ComputerHorizontal;
	direction = true;
	x = 0;
	if (!isHuman[currentPlayerIndex])
	{
		computer_x = Random.Shared.Next(x_max + 1);
		computer_skip = Random.Shared.Next(1, 4);
	}
	stopwatch.Restart();
	Console.Clear();
}

enum State
{
	Main,
	Setup,
	ConfirmRandomTurnOrder,
	PlayerHorizontal,
	PlayerVertical,
	ConfirmPlayerThrow,
	ComputerHorizontal,
	ComputerVertical,
	ConfirmComputerThrow,
	ConfirmGameEnd,
}

//Summary:
//This code is much longer than before, and it takes me lots of time to locate everytime. The source code is more challenging than before because there are lots of new code that I'm unfamilar with. I asked AI multiple times about new code formatting and which types of code could meet my logic. At last I understood and resolved all the problems.
//RequirementA: I use int[] to define arrays to store int, which can simplify the code. I use isHuman[i] to check current player is human or computer, and use player[i] and computer[i] to display current player. I also add remaining rounds.
//RequirementB: I add State.Setup to display the game menu, and the input(1-4) will become the value of playerCount(current human player amount), and use it to change the value(true/false) of array: isHuman.
//RequirementC: I use reductiveMode to represent the second mode. I use bool to check the current mode. I also need to change the condition for ending the game, if(score[currentPlayerIndex] == 0) state=State.ConfirmGameEnd; I use a variable to store the calculation result of current player, only when it >= 0, it will be output the score