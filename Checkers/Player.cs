//7: one player(can be human/computer)
namespace Checkers;

public class Player 
{
	public bool IsHuman { get; }
	public PieceColor Color { get; }

	public Player(bool isHuman, PieceColor color)
	{
		IsHuman = isHuman;
		Color = color;
	}
}
