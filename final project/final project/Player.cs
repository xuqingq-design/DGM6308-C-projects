//4 player status
//
using System;
using System.Collections.Generic;
using System.Linq;
namespace TribalGame
{
    public class Player
    {
        public int MaxHP=4;
        public int HandCards=4; //number of hand cards
        public string Name {get;}
        public bool IsHuman {get;}
        public int HP {get; set;}
        public List<Card> Hand {get; set;} //player's hand
        public List<Card> PlayerResourceCards {get; set;}
        public Player(string name, bool isHuman)
        {
            Name = name;
            IsHuman = isHuman;
            HP = MaxHP;
            Hand = new List<Card>(HandCards);
            PlayerResourceCards = new List<Card>();
        }
        public void TakeDamage(int amount)
        {
            HP=Math.Max(0,HP-amount);
        }
        public void Heal(int amount)
        {
            HP=Math.Min(MaxHP,HP+amount);
        }
        public bool IsAlive => HP>0;
        public void AddToHand(Card card)
        {
            Hand.Add(card);
        }
        public bool RemoveFromHand(Card card)
        {
            return Hand.Remove(card);
        }
        public void ResetRoundState()
        {
            
        }
        public void DisplayHand()
        {
            Console.WriteLine("  ── resource cards──");

            Console.WriteLine("  ── special cards ──");
        }
    }
}