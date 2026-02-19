//1 (run first)
//2/18 update: Firstly, increase the types of chess pieces: for Knight, they can move in a "日" style: (1, A) to (3, B). If there is a chess piece on the front grid (2, A), they cannot move forward; Rock: can only walk up, down, left, right, not diagonally; King: It can move in 8-directions; Use different symbols to distinguish them.
//Then change the distribution of special chess pieces on the board: Noraml:○  Rock:□  Knight:△  King:☺. Only normal pieces can be promoted, and the player can choose any piece type before promoting.
//When special pieces capture any opponent piece, they will have an extra turn to choose continue capture other pieces or withdraw.
//main changes on: Board.cs; Game.cs; Piece.cs; Program.cs; add: PieceType.cs. I add why I do this beside each changed code.
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using Checkers;
global using static Statics;
