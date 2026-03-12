//3 card deck
//
using System;
using System.Collections.Generic;
namespace TribalGame
{
    public class Deck
    {
        public List<Card> drawPile;
        public List<Card> discardPile;
        public Deck()
        {
            drawPile = new List<Card>(54);
            discardPile = new List<Card>(54);
            BuildFullDeck();
            Shuffle(drawPile);
        }
        //build the card deck
        public void BuildFullDeck()
        {
            drawPile.Clear();
            Suit[] allSuits = { Suit.Spades, Suit.Hearts, Suit.Clubs, Suit.Diamonds };
            foreach (Suit suit in allSuits)
            {
                for (int rank = 2; rank <= 13; rank++)
                {
                    drawPile.Add(new Card(suit, rank, CardType.Resource));
                }
                drawPile.Add(new Card(suit, 14, CardType.Special));
            }
            drawPile.Add(new Card(Suit.None, 15, CardType.Joker));
            drawPile.Add(new Card(Suit.None, 16, CardType.Joker));
        }
        public void Shuffle(List<Card> deck)
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(0, i + 1);
                Card temp = deck[i];
                deck[i] = deck[j];
                deck[j] = temp;
            }
        }
        //draw cards & assign default resource/special cards
        public Card DrawCard()
        {
            EnsurePileNotEmpty();
            int top = drawPile.Count - 1;
            Card card = drawPile[top];
            drawPile.RemoveAt(top);
            return card;
        }
        public Card DrawResourceCard()
        {
            var setAside = new List<Card>();
            Card foundCard = null!;
            while (foundCard == null)
            {
                EnsurePileNotEmpty();
                Card drawnCard = DrawCard();
                if(drawnCard.Type == CardType.Resource)
                {
                    foundCard=drawnCard;
                }
                else
                {
                    setAside.Add(drawnCard);
                }
            }
            drawPile.InsertRange(0,setAside); //add temp special cards(set aside before) to the end of the drawPile
            return foundCard;
        }
        public Card DrawSpecialCard()
        {
            var setAside = new List<Card>();
            Card foundCard = null!;
            while (foundCard == null)
            {
                EnsurePileNotEmpty();
                Card drawnCard = DrawCard();
                if(drawnCard.Type == CardType.Special || drawnCard.Type == CardType.Joker)
                {
                    foundCard=drawnCard;
                }
                else
                {
                    setAside.Add(drawnCard);
                }
            }
            drawPile.InsertRange(0,setAside); //add temp resource cards(set aside before) to the end of the drawPile
            return foundCard;
        }
        public void EnsurePileNotEmpty()
        {
            if (drawPile.Count > 0)
            {
                return;
            }
            if (drawPile.Count == 0)
            {
                Console.WriteLine("drawPile empty, use discardPile");
                drawPile.AddRange(discardPile);
                discardPile.Clear();
                Shuffle(drawPile);
            }
        }
    }
}