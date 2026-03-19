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
        public string GetCardNameDisplay()
        {
            if (Type == CardType.Joker)
            {
                if (Rank == 15)
                {
                    return "[Small Jocker] Priest's Guidance — Normal Success.";
                }
                else
                {
                    return "[Big Jocker] Chief's Authority — Great Success.";
                }
            }
            if (Type == CardType.Special)
            {
                return $"[{GetSuitSymbol()}A] {GetSpecialName()}";
            }
            //resource cards:
            return $"[{GetSuitSymbol()}{GetRankDisplay()}] Point Value:{PointValue}";
        }
        public string GetSpecialName()
        {
            switch (Suit)
            {
                case Suit.Hearts: return "Totem of Life";
                case Suit.Spades: return "Wild Curse";
                case Suit.Diamonds: return "Resource Swap";
                case Suit.Clubs: return "Iron Bastion";
                default: return "???";
            }
        }
        public string GetEffectDescription()
        {
            if (Type == CardType.Joker)
            {
                if (Rank == 15)
                {
                    return "[Small Jocker] Priest's Guidance — Your score this round becomes Environment Target Value." +
                           "You can continue playing the resource card. Reach the Value for great success to upgrade to it.";
                }
                else
                {
                    return "[Big Jocker] Chief's Authority — Automatically gets a great success this round.";
                }
            }
            switch (Suit)
            {
                case Suit.Hearts: return "Heal 1 HP.";
                case Suit.Spades: return "Reduce the Opponent’s final total by 20% this round.";
                case Suit.Diamonds: return "Discard 1 card from your hand and draw a new one from the deck.";
                case Suit.Clubs: return "If you fail this round, you won't lose HP.";
                default: return "???";
            }
        }
        // ---- Card Art ----
        // Designed by AI
        // Resource card example (♠K):       Special card (♥A):      Joker:
        //0   ┌───────┐                          ┌───────┐              ┌───────┐
        //1   │♠ K    │                          │♥ A    │              │  ★    │
        //2   │       │                          │Totem  │              │  BIG  │
        //3   │   13pt│                          │ofLife │              │ JOKER │
        //4   └───────┘                          └───────┘              └───────┘
        public string[] GetCardArtLines()
        {
            string[] lines = new string[5];
            lines[0] = "┌───────┐";
            lines[4] = "└───────┘";
            if (Type == CardType.Joker)
            {
                lines[1] = "│  ★    │";
                if (Rank == 16)
                {
                    lines[2] = "│  BIG  │";
                }
                else
                {
                    lines[2] = "│ SMALL │";
                }
                lines[3] = "│ JOKER │";
            }
            else if (Type == CardType.Special)
            {
                string suit = GetSuitSymbol();
                lines[1] = $"│{suit}  A   │";
                if (Suit == Suit.Hearts)
                {
                    lines[2] = "│ Totem │";
                    lines[3] = "│of Life│";
                }
                else if (Suit == Suit.Spades)
                {
                    lines[2] = "│ Wild  │";
                    lines[3] = "│ Curse │";
                }
                else if (Suit == Suit.Diamonds)
                {
                    lines[2] = "│ Card  │";
                    lines[3] = "│ Swap  │";
                }
                else if (Suit == Suit.Clubs)
                {
                    lines[2] = "│ Iron  │";
                    lines[3] = "│Bastion│";
                }
            }
            else //resource cards
            {
                string suit = GetSuitSymbol();
                string rank = GetRankDisplay();
                //row1
                string row1 = (suit + " " + rank).PadRight(7); //pint ♠ K, then increase to 7 chars
                if(row1.Length>7) row1 = row1.Substring(0,7); //prevent the words beyond the border; Substring(0,7): from the position 0, get 7 characters behind ("HelloWorld"=>"HelloWo")
                //row3
                string row3 = (PointValue.ToString() + "pt").PadLeft(7); //show point value right aligned
                if(row3.Length>7) row3 = row3.Substring(row3.Length -7);
                lines[1] = $"│{row1}│";
                lines[2] = "│       │";
                lines[3] = $"│{row3}│";
            }
            return lines;
        }
    }
}