//9 Game Engine
//
using System;
using System.Collections.Generic;
using System.Linq;
namespace TribalGame
{
    public class GameEngine
    {
        public Player P1;
        public Player P2;
        public Deck Deck;
        public bool IsPVE;
        public int TotalRounds = 10;
        public GameEngine(bool isPVE)
        {
            IsPVE = isPVE;
            Deck = new Deck();
            P1 = new Player("Player 1", isHuman: true);
            if (isPVE)
            {
                P2 = new Player("Computer", isHuman: false);
            }
            else
            {
                P2 = new Player("Player 2", isHuman: true);
            }
        }
        public void Run()
        {
            //display rules
            //draw default resource cards and special card

            for (int day = 1; day<=TotalRounds; day++)
            {
                //one round: 1.draw environment card

                //2. P1 choose to use special card or not
                //P1 use resource cards

                //3. P2 round...

                //4. compare points of P1 and P2 with environment
                //display result => handle HP

                //5.draw new cards to 4
            }
        }
    }
}