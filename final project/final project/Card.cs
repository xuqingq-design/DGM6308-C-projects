//2 cards' details
//create main properties of cards(suit, type, rank, ...), display cards' description
using System;
namespace TribalGame
{
    public enum Suit
    {
        Spades, //♠
        Hearts, //♥
        Clubs, //♣
        Diamonds, //♦
        None
    }
    public enum CardType
    {
        Resource, //2-K
        Special, //A
        Joker
    }
    public class Card
    {
        public Suit Suit { get; }
        public int Rank { get; }
        public CardType Type { get; }
        public int PointValue { get; } //the value of point of one card
        public bool Red => Suit == Suit.Hearts || Suit == Suit.Diamonds;
        public bool Black => Suit == Suit.Spades || Suit == Suit.Clubs;
        //one card includes:
        public Card(Suit suit, int rank, CardType type)
        {
            Suit = suit;
            Rank = rank;
            Type = type;
            if (type == CardType.Resource)
            {
                PointValue = rank;
            }
            else
            {
                PointValue = 0;
            }
        }
        //display card information:
        public string GetSuitSymbol()
        {
            switch (Suit)
            {
                case Suit.Spades: return "♠";
                case Suit.Hearts: return "♥";
                case Suit.Diamonds: return "♦";
                case Suit.Clubs: return "♣";
                default: return " ";
            }
        }
        public string GetRankDisplay()
        {
            switch (Rank)
            {
                case 11: return "J";
                case 12: return "Q";
                case 13: return "K";
                case 14: return "A";
                case 15: return "SmallJoker";
                case 16: return "BigJoker";
                default: return Rank.ToString();
            }
        }
        //public string GetCardNameDisplay()
        //public string GetSpecialCardDisplay()
        //public string GetEffectDescription()
    }
}