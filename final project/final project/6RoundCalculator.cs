//6 round calculator
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace TribalGame
{
    public enum RoundOutcome
    {
        Failure,
        Success,
        GreatSuccess,
    }
    public class PlayerRoundResult
    {
        public Player Player { get; set; } = null!;
        public int AddTotal { get; set; } //add all used resource cards
        public double ComboRate { get; set; } //combo rate: 1.0/1.2/1.5
        public double TotalAfterCombo { get; set; } //AddTotal*ComboRate
        public bool PlayerIsCursed { get; set; } //ability of ♠A (-20%)
        public bool DonotLoseHP { get; set; } //ability of ♣A
        public int FinalTotal { get; set; } //total point after calculating the curse
        public RoundOutcome Outcome { get; set; }
        public List<string> Log { get; set; } = new List<string>(); //display result, use List for Unity to use
    }

    //calculator tool:
    public static class RoundCalculator
    {
        //calculate one player's point in this round
        public static PlayerRoundResult Calculate(Player player, EnvironmentCard environmentCard, bool playerIsCursed)
        {
            var result = new PlayerRoundResult
            {
                Player = player,
                ComboRate = 1.0
            };
            //step1: use joker?
            //use big joker: automatically great success
            if (player.IsAutoGreatSuccess)
            {
                result.AddTotal = environmentCard.GreatSuccessValue;
                result.TotalAfterCombo = environmentCard.GreatSuccessValue;
                result.FinalTotal = environmentCard.GreatSuccessValue;
                result.Outcome = RoundOutcome.GreatSuccess;
                result.Log.Add("  [Big Jocker] Chief's Authority — Automatically gets a great success this round.");
                return result;
            }
            //use small joker: success but still need target value*0.3 points to get a great success:
            if (player.IsAutoSuccess)
            {
                int get = environmentCard.TargetValue;
                int need = environmentCard.GreatSuccessValue - environmentCard.TargetValue;
                result.Log.Add("  [Small Jocker] Priest's Guidance — Automatically gets a normal success this round.");
                result.Log.Add($"  Total Score has been raised to {get}, need {need} points to get a great success");
            }
            //step2: add resource cards, if same color/suit, add extra points
            List<Card> playedResourceCards = player.PlayedResourceCards.Where(c => c.Type == CardType.Resource).ToList(); //called every card in the List => "c"; check c.type is resource card or not; transfrom it into List<Card>
            int cardsSubtotalPoint = 0;
            foreach (Card card in playedResourceCards)
            {
                int pointValue = card.PointValue;
                int colorScore = 0;
                int suitScore = 0;
                //same color(red/black) => +2:
                if (card.Red == environmentCard.IsRedColor)
                {
                    colorScore = 2;
                }
                //same suit => +5 (can do effect at the same time(+7)):
                if (card.Suit == environmentCard.TotemSuit)
                {
                    suitScore = 5;
                }
                int cardTotalPoint = pointValue + colorScore + suitScore;
                cardsSubtotalPoint += cardTotalPoint; //add all cards

                //Log:
                string scoreDesc;
                if (suitScore > 0)
                {
                    scoreDesc = string.Format($"COLOR +{colorScore}!  SUIT +{suitScore}!");
                }
                else if (colorScore > 0)
                {
                    scoreDesc = string.Format($"COLOR +{colorScore}!");
                }
                else
                {
                    scoreDesc = string.Format("Without Bonus.");
                }
                result.Log.Add(string.Format($"  {card.GetCardNameDisplay()}  Base:{pointValue}  + {scoreDesc} = {cardTotalPoint}"));
            }
            result.AddTotal = cardsSubtotalPoint;
            result.Log.Add($"  Subtotal: +{cardsSubtotalPoint}");

            //step3: calculate combo rate
            double Rate = CalculateComboRate(playedResourceCards, result.Log);
            result.ComboRate = Rate;
            result.TotalAfterCombo = result.AddTotal * result.ComboRate;
            //Log:
            if (Rate > 1.0)
            {
                result.Log.Add($"  Combo Rate: *{Rate}");
            }

            double finalTotal = result.TotalAfterCombo;
            //step1 use small joker(Ignore the curse)
            if (player.IsAutoSuccess)
            {
                int need = environmentCard.GreatSuccessValue - environmentCard.TargetValue;
                result.FinalTotal = (int)finalTotal + environmentCard.TargetValue;
                if (finalTotal >= need)
                {
                    result.Outcome = RoundOutcome.GreatSuccess;
                }
                else
                {
                    result.Outcome = RoundOutcome.Success;
                }
                return result;
            }
            //step4: check if the player is cursed, -20%
            if (playerIsCursed)
            {
                double penalty = Math.Floor(result.TotalAfterCombo * 0.20); //Take the whole down and -20%
                finalTotal = result.TotalAfterCombo - penalty;
                result.PlayerIsCursed = true;
                result.Log.Add(string.Format($"  [♠A Curse] -20% (-{penalty}) => {finalTotal}"));
            }
            result.FinalTotal = (int)finalTotal;

            //step5: return result and outcome
            if (result.FinalTotal >= environmentCard.GreatSuccessValue)
            {
                result.Outcome = RoundOutcome.GreatSuccess;
            }
            else if (result.FinalTotal >= environmentCard.TargetValue)
            {
                result.Outcome = RoundOutcome.Success;
            }
            else
            {
                result.Outcome = RoundOutcome.Failure;
            }
            return result;
        }
        public static double CalculateComboRate(List<Card> playedCards, List<string> log)
        {
            if (playedCards.Count < 2) //play less than 2 cards
            {
                return 1.0;
            }
            double rate = 1.0;
            //define all situations
            //add 3 cards rank match
            bool threeOfKind = playedCards.Count == 3 && playedCards[0].Rank == playedCards[1].Rank && playedCards[1].Rank == playedCards[2].Rank;
            //GroupBy(c=>c.Rank): group all playedCards by rank, ep:[♠7, ♥7, ♦K]=>[♠7, ♥7];
            //Any(g=>g.Count()==2): check in all these groups, if there is any 2 pair group
            bool pair = !threeOfKind && playedCards.GroupBy(c => c.Rank).Any(g => g.Count() == 2);
            //all 3 cards suit match
            bool flush = playedCards.Count == 3 && playedCards[0].Suit == playedCards[1].Suit && playedCards[1].Suit == playedCards[2].Suit;
            if (threeOfKind)
            {
                rate *= 1.5;
                log.Add("  [COMBO] Three of Kind! *1.5");
            }
            else if (pair)
            {
                rate *= 1.2;
                log.Add("  [COMBO] Pair! *1.2");
            }
            if (flush)
            {
                rate *= 1.5;
                log.Add("  [COMBO] Flush! *1.5");
            }
            return rate;
        }
    }
}