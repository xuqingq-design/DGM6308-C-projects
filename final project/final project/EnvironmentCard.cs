//6 every round random environment challenge
//
using System;
using System.Dynamic;

namespace TribalGame
{
    public class EnvironmentCard
    {
        public Card DrawnCard {get;}
        public int Day {get;}
        public Suit TotemSuit {get;}
        public bool IsRedColor {get;}
        public int TargetValue {get;}
        public int GreatSuccessValue {get;}
        public EnvironmentCard(Card drawnCard, int day)
        {
            DrawnCard = drawnCard;
            Day = day;
            TotemSuit = drawnCard.Suit;
            IsRedColor = DrawnCard.Red;
            if (day <= 5)
            {
                TargetValue = Random.Shared.Next(10,23);
            }
            else
            {
                TargetValue = Random.Shared.Next(22,39);
            }
            GreatSuccessValue = (int)Math.Ceiling(TargetValue*1.3); //GreatSuccessValue=TargetValue*1.3(Math.Ceiling=>return double=>transfer to int)
        }
        public void Display()
        {
            Console.WriteLine("  ---Environment Card---");

            Console.WriteLine($"  target value: {TargetValue}");
            Console.WriteLine($"  GreatSuccessValue: {GreatSuccessValue}");
            Console.WriteLine($"  TotemSuit: {TotemSuit}");
        }
    }
}