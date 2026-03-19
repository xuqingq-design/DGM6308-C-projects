//5 every round random environment challenge
//
using System;
using System.Dynamic;

namespace TribalGame
{
    public class EnvironmentCard
    {
        public Card DrawnCard { get; }
        public int Day { get; }
        public Suit TotemSuit { get; }
        public bool IsRedColor { get; }
        public int TargetValue { get; }
        public int GreatSuccessValue { get; }
        public EnvironmentCard(Card drawnCard, int day)
        {
            DrawnCard = drawnCard;
            Day = day;
            TotemSuit = drawnCard.Suit;
            IsRedColor = DrawnCard.Red;
            if (day <= 5)
            {
                TargetValue = Random.Shared.Next(10, 23);
            }
            else
            {
                TargetValue = Random.Shared.Next(22, 39);
            }
            GreatSuccessValue = (int)Math.Ceiling(TargetValue * 1.3); //GreatSuccessValue=TargetValue*1.3(Math.Ceiling=>return double=>transfer to int)
        }
        public void Display()
        {
            Console.WriteLine("  ---Environment Card---");
            string suit = TotemSuit switch //convert suit name to symbol
            {
                Suit.Spades => "♠",
                Suit.Hearts => "♥",
                Suit.Diamonds => "♦",
                Suit.Clubs => "♣",
                _ => "???"
            };
            string color = IsRedColor ? "Red" : "Black";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ╔══════════════════════════════════════════╗");
            Console.WriteLine(string.Format(
                "  ║  Day {0,-2} — Environment Card               ║", Day));
            Console.WriteLine("  ╠══════════════════════════════════════════╣");
            Console.Write("  ║  ");
            if (color == "Red")
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.Write($"Totem Suit   : {suit}                        ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("║");
            Console.Write("  ║  ");
            if (color == "Red")
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            //no matter color = "red"(3 character)/"black"(5), the output always occupies 18 spaces, fill in the lack spaces automatically
            Console.Write(string.Format(
                "Environment Color   : {0,-18}", color)); //-18: nagative numbers => left aligned; positive => right aligned; 
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("║");
            Console.WriteLine("  ╠══════════════════════════════════════════╣");
            Console.WriteLine(string.Format(
                "  ║  Target Value  : {0,-24}║", TargetValue));
            Console.WriteLine(string.Format(
                "  ║  Great Success Need   : {0,-17}║", GreatSuccessValue));
            Console.WriteLine("  ╠══════════════════════════════════════════╣");
            Console.WriteLine("  ║  Rules：                                 ║");
            Console.WriteLine("  ║    same color   → +2 each                ║");
            Console.WriteLine("  ║    same suit    → +7 each                ║");
            Console.WriteLine("  ╚══════════════════════════════════════════╝");
            Console.ResetColor();
        }
    }
}