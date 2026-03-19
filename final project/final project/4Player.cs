//4 player status
//
using System;
using System.Collections.Generic;
using System.Linq;
namespace TribalGame
{
    public class Player
    {
        public const int MaxHP = 4;
        public const int HandCards = 4; //number of hand cards
        public string Name { get; }
        public bool IsHuman { get; }
        public int HP { get; set; }
        public List<Card> Hand { get; set; } //player's hand
        public List<Card> PlayedResourceCards { get; set; } //playered resource card this round
        public Card? PlayedSpecialCards { get; set; } //playered special card this round, can be null
        public bool IsAutoSuccess { get; set; }
        public bool IsAutoGreatSuccess { get; set; }
        public bool DonotLoseHP { get; set; } //use clubs A
        public bool BeCursed { get; set; } //other player use spades A

        public Player(string name, bool isHuman)
        {
            Name = name;
            IsHuman = isHuman;
            HP = MaxHP;
            Hand = new List<Card>(HandCards);
            PlayedResourceCards = new List<Card>();
        }
        public void TakeDamage(int amount)
        {
            HP = Math.Max(0, HP - amount); //lower limit to 0
        }
        public void Heal(int amount)
        {
            HP = Math.Min(MaxHP, HP + amount); //higher limit to MaxHP
        }
        public bool IsAlive => HP > 0; //while HP>0, alive status
        public void AddToHand(Card card)
        {
            Hand.Add(card);
        }
        public bool RemoveFromHand(Card card)
        {
            return Hand.Remove(card);
        }
        public List<Card> GetResourceCards()
        {
            return Hand.Where(c => c.Type == CardType.Resource).ToList(); //return all resource cards in player's hand
        }
        public List<Card> GetSpecialCards()
        {
            return Hand.Where(c => c.Type == CardType.Special || c.Type == CardType.Joker).ToList(); //return all special cards in player's hand
        }
        public void ResetRoundState() //use in the begining of every round, set all status of players
        {
            PlayedResourceCards = new List<Card>();
            PlayedSpecialCards = null;
            DonotLoseHP = false;
            BeCursed = false;
            IsAutoGreatSuccess = false;
            IsAutoSuccess = false;
        }
        public void DisplayHandOldVersion() //do not run, just save old version(without card art)
        {
            Console.WriteLine($"  {Name}'s Hand:");
            List<Card> resources = GetResourceCards();
            List<Card> specials = GetSpecialCards();
            Console.WriteLine("  ── Resource Cards──");
            if (resources.Count == 0)
            {
                Console.WriteLine("  (None)");
            }
            else
            {
                for (int i = 0; i < resources.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  [{i + 1}] {resources[i].GetCardNameDisplay()}");
                    Console.ResetColor();
                }
            }
            Console.WriteLine("  ── Special Cards ──");
            if (specials.Count == 0)
            {
                Console.WriteLine("  (None)");
            }
            else
            {
                for (int i = 0; i < specials.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"  [*{i + 1}] {specials[i].GetCardNameDisplay()}");
                    Console.ResetColor();
                }
            }
        }
        public void DisplayHand()
        {
            Console.WriteLine($"  {Name}'s Hand:");
            List<Card> resources = GetResourceCards();
            List<Card> specials = GetSpecialCards();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ── Resource Cards──");
            Console.ResetColor();
            if (resources.Count == 0)
            {
                Console.WriteLine("  (None)");
            }
            else
            {
                DisplayCardsInRow(resources, areResourceCards: true);
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  ── Special Cards ──");
            Console.ResetColor();
            if (specials.Count == 0)
            {
                Console.WriteLine("  (None)");
            }
            else
            {
                DisplayCardsInRow(specials, areResourceCards: false);
            }
        }
        //    [1]        [2]        [3]
        //  ┌───────┐ ┌───────┐ ┌───────┐
        //  │♠ Q    │ │♥ Q    │ │♣ 10   │
        //  │       │ │       │ │       │
        //  │   12pt│ │   12pt│ │   10pt│
        //  └───────┘ └───────┘ └───────┘
        public void DisplayCardsInRow(List<Card> cards, bool areResourceCards)
        {
            string[][] allCardsArt = new string[cards.Count][]; //[0][0: content=>card1: line[0]], [0][1: content=>card1: line[1]], ...,[0][4: content=>card1: line[4]], [1][0: content=>card2: line[0]], ...
            for (int i = 0; i < cards.Count; i++)
            {
                allCardsArt[i] = cards[i].GetCardArtLines();
            }
            if (areResourceCards)
            {
                Console.Write("    ");
                for (int i = 0; i < cards.Count; i++)
                {
                    Console.Write($"  [{i + 1}]      "); //    [1]        [2]        [3]
                }
                Console.WriteLine();
            }
            for (int line = 0; line < 5; line++)
            {
                Console.Write("    ");
                for (int i = 0; i < cards.Count; i++)
                {
                    // Choose color based on card type and suit
                    if (cards[i].Type == CardType.Joker || cards[i].Type == CardType.Special)
                        Console.ForegroundColor = ConsoleColor.Yellow;   // Special cards
                    else if (cards[i].Red)
                        Console.ForegroundColor = ConsoleColor.Red;      // Red suit
                    else
                        Console.ForegroundColor = ConsoleColor.White;    // Black suit

                    Console.Write(allCardsArt[i][line]);
                    Console.ResetColor();
                    Console.Write(" "); //gap between cards
                }
                Console.WriteLine();
            }
        }
        public string GetPlayerStatus()
        {
            string hp = new string('♥', HP) + new string('♡', MaxHP - HP); //use character symbol to display
            return string.Format("{0,-12} — HP: {1}  ({2}/{3})",
                Name, hp, HP, MaxHP); //ep: "Player 1 — HP: ♥♥♡♡ (2/4)"
        }
    }
}