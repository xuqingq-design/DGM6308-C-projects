//7 round calculator
//
using System;
using System.Collections.Generic;
using System.Linq;
namespace TribalGame
{
    public class RoundCalculator
    {
        public enum RoundOutcome
        {
            Failure,
            Success,
            GreatSuccess,
        }
        public class PlayerRoundResult
        {
            public Player Player {get; set;} = null!;
            public double AddTotal {get; set;} //add all used resource cards
            public double ComboRate {get; set;} //combo rate: 1.0/1.2/1.5
            public double TotalAfterCombo {get; set;} //AddTotal*ComboRate
            public bool IsCursed {get; set;} //ability of ♠A (-20%)
            public int FinalTotal {get; set;} //total point after calculating the curse
            public RoundOutcome Outcome {get; set;}
        }
        public static class RoundCalculator
        {
            //calculate one player's point in this round
            public static PlayerRoundResult Calculate(Player player, EnvironmentCard environment)
            {
                var result = new PlayerRoundResult
                {
                    Player = player,
                    ComboRate = 1.0
                };
                //step1: use joker?

                //step2: add resource cards, if same color/suit, add extra points

                //step3: calculate combo rate

                //step4: check if the player is cursed

                //step5: return result and outcome
            }
        }
    }
}