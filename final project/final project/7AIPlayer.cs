//7 computer logic
//
using System;
using System.Collections.Generic;
using System.Linq;
namespace TribalGame
{
    public class AIPlayer
    {
        public static Card? DecideSpecialCard(Player ai, Player opponent, EnvironmentCard env)
        {
            List<Card> specials = ai.GetSpecialCards();
            if(specials.Count == 0)
            {
                return null;
            }
            List<Card> resources =ai.GetResourceCards();
            int totalScore = 0;
            foreach (Card c in resources)
            {
                totalScore+=ScoreCard(c,env); //calculate ai's hand
            }
            bool aiNearFail = totalScore<env.TargetValue; //use to check if the score is enough or not
            bool aiNearDeath = ai.HP <=1;
            bool opponentNearDeath = opponent.HP <=2;

            Card? FindBigJoker() => specials.FirstOrDefault(c => c.Type == CardType.Joker && c.Rank==16); //use rank to find special cards
            Card? FindSmallJoker() => specials.FirstOrDefault(c => c.Type == CardType.Joker && c.Rank==15);
            Card? FindAce(Suit suit) => specials.FirstOrDefault(c => c.Type == CardType.Special && c.Suit == suit); //check if the suit is correct
            //1.Big Joker: when low HP / nearly fail this round use
            Card? bigJoker = FindBigJoker();
            if(bigJoker != null &&(aiNearDeath || aiNearFail))
            {
                return bigJoker;
            }
            //2.Small Joker: when point is not enough use
            Card? smallJoker = FindSmallJoker();
            if(smallJoker != null && aiNearFail)
            {
                return smallJoker;
            }
            //3.Clubs A: do not lose HP; when low HP use and this round is going to lose => use
            Card? clubsA = FindAce(Suit.Clubs);
            if (clubsA != null && aiNearDeath && aiNearFail)
            {
                return clubsA;
            }
            //4.Hearts A: when low HP use
            Card? heartsA = FindAce(Suit.Hearts);
            if (heartsA != null && aiNearDeath)
            {
                return heartsA;
            }
            //5.Spades A: when player low HP use
            Card? spadesA = FindAce(Suit.Spades);
            if (spadesA != null && opponentNearDeath)
            {
                return spadesA;
            }
            //6.Diamonds A: when point this round is not enough use
            Card? diamondsA = FindAce(Suit.Diamonds);
            if (diamondsA != null && aiNearFail)
            {
                return diamondsA;
            }
            return null;
        }
        public static int ScoreCard(Card card, EnvironmentCard env) //calculate 1 resource card's point value this round
        {
            if(card.Type != CardType.Resource)
            {
                return 0;
            }
            int score = card.PointValue;
            if (card.Red == env.IsRedColor)
            {
                score+=2;
            }
            if (card.Suit == env.TotemSuit)
            {
                score+=5;
            }
            return score;
        }
        public static Card DecideDiscardCard(Player ai, EnvironmentCard env) //choose the lowest point card to discard
        {
            List<Card> resources = ai.GetResourceCards();
            if(resources.Count == 0) return ai.Hand[0]; //if no resource card, discard first special card
            return resources.OrderBy(c => ScoreCard(c,env)).First(); //sort all resource cards from small to big => return the first one (the smallest one)
        }
        public static List<Card> DecideResourceCards(Player ai, EnvironmentCard env) //choose 1-3 resource cards to play
        {
            List<Card> resources = ai. GetResourceCards();
            if(resources.Count == 0) return new List<Card>(); //if no resource cards, do not play
            //sort all resource cards from big to small => take the first 1-3 cards
            return resources.OrderByDescending(c=>ScoreCard(c,env)).Take(Math.Min(3,resources.Count)).ToList(); //OrderByDescending: sort from the highest score to the lowest score =>Take(Math.Min(3, resources.Count)): take resources.Count to 3(Max value) cards from the beginning
        }
    }
}