//8 Game Engine
//main gameplay running logic
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
            //display title:
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
  ████████╗██████╗ ██╗██████╗  █████╗ ██╗
     ██╔══╝██╔══██╗██║██╔══██╗██╔══██╗██║
     ██║   ██████╔╝██║██████╔╝███████║██║
     ██║   ██╔══██╗██║██╔══██╗██╔══██║██║
     ██║   ██║  ██║██║██████╔╝██║  ██║███████╗
     ╚═╝   ╚═╝  ╚═╝╚═╝╚═════╝ ╚═╝  ╚═╝╚══════╝
           -- 10 days Survival --
            ");
            Console.ResetColor();
            //players draw starting hands
            //draw default resource cards and special card
            foreach (Player p in new[] { P1, P2 })
            {
                for (int i = 0; i < 3; i++)
                {
                    p.AddToHand(Deck.DrawResourceCard());//draw 3 resource cards
                }
                p.AddToHand(Deck.DrawSpecialCard());//draw 1 special card
            }
            //display game start
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("---GAME START---");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            string modeText = IsPVE ? "[Player] VS. [Computer]" : "[Player] VS. [Player]";
            Console.WriteLine($"  Mode: {modeText}");
            Console.WriteLine($"  {P1.GetPlayerStatus()}");
            Console.WriteLine($"  {P2.GetPlayerStatus()}");
            Console.WriteLine($"  There are{TotalRounds} Rounds. If HP reduce to 0, gameover.");
            Console.ResetColor();
            Pause("Press Enter to Continue...");

            for (int day = 1; day <= TotalRounds; day++)
            {
                DisplayRoundStart(day);
                PlayOneRound(day);
                if (!P1.IsAlive || !P2.IsAlive)
                {
                    break;
                }
                if (day < TotalRounds)
                {
                    Pause("Press Enter to start next day.");
                }
            }
            DeclareWinner();
        }
        public void DisplayRoundStart(int day)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"---Day {day}/{TotalRounds}---");
            Console.ResetColor();
            Console.WriteLine($"  {P1.GetPlayerStatus()}");
            Console.WriteLine($"  {P2.GetPlayerStatus()}");
        }
        public void PlayOneRound(int day)
        {
            //clear last round status:
            P1.ResetRoundState();
            P2.ResetRoundState();
            //one round: 1.draw environment card
            Card envCard = Deck.DrawResourceCard();
            var environment = new EnvironmentCard(envCard, day);
            environment.Display();
            //2.draw cards to 4:
            RefillHand(P1);
            RefillHand(P2);
            //3.Both player prepare which cards to play. (1. choose to use which special card/do not use => apply effect 2.choose which resource cards to play)
            PreparationPhase(P1, P2, environment);
            PreparationPhase(P2, P1, environment);
            //4. compare points of P1 and P2 with environment
            //display result => handle HP
            ResultPhase(P1, P2, environment);
            //5.discard environment card to the discard pile
            Deck.Discard(envCard);
        }
        public void RefillHand(Player player)
        {
            int need = Player.HandCards - player.Hand.Count;
            if (need <= 0)
            {
                return;
            }
            for (int i = 0; i < need; i++)
            {
                player.AddToHand(Deck.DrawCard()); //draw cards to hand until 4
            }
        }
        public void PreparationPhase(Player p1, Player p2, EnvironmentCard env)
        {
            Console.WriteLine($"---{p1.Name}'s turn---");
            Card? chosenSpecial;
            if (p1.IsHuman) //human playing
            {
                if (!IsPVE)
                {
                    PrivacyScreen(p1, p2, env); //prevent p2 know p1's hand 
                }
                p1.DisplayHand();
                chosenSpecial = HumanPickSpecialCard(p1, env);//Player choose to use special card or not => which special card?
                if (chosenSpecial != null)
                {
                    ApplySpecialCardEffect(chosenSpecial, p1, p2, env);
                }
                //player play resource cards this round:
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  Play 1-3 resource cards?");
                Pause("  >>> press Enter to continue <<<");
                Console.ResetColor();
                Console.Clear();
                p1.DisplayHand();
                p1.PlayedResourceCards = HumanPickResourceCard(p1, env);
            }
            else //computer playing
            {
                chosenSpecial = AIPlayer.DecideSpecialCard(p1, p2, env);
                if (chosenSpecial != null)
                {
                    Console.WriteLine($"  AI Player choose to play {chosenSpecial.GetCardNameDisplay()}");
                    ApplySpecialCardEffect(chosenSpecial, p1, p2, env);
                }
                //computer play resource cards
                p1.PlayedResourceCards = AIPlayer.DecideResourceCards(p1, env);
                Console.WriteLine($"  AI selected {p1.PlayedResourceCards.Count} resource cards");
            }
            foreach (Card c in p1.PlayedResourceCards)
            {
                p1.RemoveFromHand(c); //remove played resource cards
            }
        }
        public Card? HumanPickSpecialCard(Player p, EnvironmentCard env)
        {
            List<Card> specials = p.GetSpecialCards(); //get all special cards of player's hand
            if (specials.Count == 0)
            {
                return null; //no special card => no need to play
            }
            Console.WriteLine("  Your Special Cards:");
            for (int i = 0; i < specials.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {specials[i].GetCardNameDisplay()}");
                Console.WriteLine($"  effect: {specials[i].GetEffectDescription()}");
            }
            Console.WriteLine("  [0] Skip - Don't play any Special Card.");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"  Input your selection (0 - {specials.Count})");
            Console.ResetColor();
            while (true)
            {
                string input = Console.ReadLine()?.Trim() ?? ""; //wait for player to choose
                if (int.TryParse(input, out int number)) //if input number key => convert to int
                {
                    //if it's vaild key:
                    if (number == 0)
                    {
                        return null;
                    }
                    if (number >= 1 && number <= specials.Count)
                    {
                        return specials[number - 1];
                    }
                    Console.Write("  invalid input, please try again.");
                }
            }
        }
        public List<Card> HumanPickResourceCard(Player p, EnvironmentCard env)
        {
            List<Card> resources = p.GetResourceCards();
            if (resources.Count == 0)
            {
                Console.WriteLine("  No resource cards in hand.");
                return new List<Card>();
            }
            Console.WriteLine($"  Target Value: {env.TargetValue}   Great Success: {env.GreatSuccessValue}   Suit: {env.DrawnCard.GetSuitSymbol()}");
            Console.WriteLine();
            for (int i = 0; i < resources.Count; i++)
            {
                Card card = resources[i];//get all resource cards
                int color = card.Red == env.IsRedColor ? 2 : 0; //check all situation and bonus => give hint
                int suit = card.Suit == env.TotemSuit ? 5 : 0;
                int total = card.PointValue + color + suit; //calculate subtotal
                string hint;
                if (suit > 0)
                {
                    hint = $"+7(Suit+Color) Total Score:{total}";
                }
                else if (color > 0)
                {
                    hint = $"+2(Color) Total Score:{total}";
                }
                else
                {
                    hint = $"+0 Total Score:{total}";
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  [{i + 1}] {card.GetCardNameDisplay()}, {hint}");
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  Choose your resource cards this round:");
            Console.WriteLine("  Input Number (ep: 1 or 1 2 3)...");
            Console.ResetColor();
            while (true)
            {
                string input = Console.ReadLine()?.Trim() ?? ""; //input key number
                //split the string(input) into array, if meet ' '(space)/ ','(comma) => split once; StringSplitOptions.RemoveEmptyEntries: remove all empty sting
                string[] numbers = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries); //input: 1 2 3/ 1,2,3 => result: ["1", "2", "3"]
                if (numbers.Length < 1 || numbers.Length > 3)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("  Please choose 1-3 cards");
                    Console.ResetColor();
                    continue;
                }
                var chosen = new List<Card>();
                var usedi = new HashSet<int>(); //HashSet: create a List that no duplication allowed; prevent the player choose the same card repeatedly
                bool valid = true;
                foreach (string number in numbers) //check if it's a valid key
                {
                    if (int.TryParse(number, out int i) && i >= 1 && i <= resources.Count && usedi.Add(i)) //if this card(i) has been chosen, usedi.Add(i) => return false => invalid input
                    {
                        chosen.Add(resources[i - 1]); //valid => add to chosen cards
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"  Invalid input. Please input 1 - {resources.Count}");
                        valid = false;
                        Console.ResetColor();
                        continue;
                    }
                }
                if (!valid)
                {
                    continue;
                }
                Console.WriteLine("  You choose:");
                foreach (Card c in chosen)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  {c.GetCardNameDisplay()}");
                    Console.ResetColor();
                }
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  Confirm? [Y/N]");
                Console.ResetColor();
                string confirm = Console.ReadLine()?.Trim().ToUpperInvariant() ?? ""; //Confirmation?
                if (confirm == "Y" || confirm == "YES")
                {
                    return chosen;
                }
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("  Cancel. Please re-enter the number:");
                Console.ResetColor();
            }
        }
        public void ApplySpecialCardEffect(Card special, Player p1, Player p2, EnvironmentCard env)
        {
            p1.RemoveFromHand(special);
            p1.PlayedSpecialCards = special;
            Deck.Discard(special);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"  {p1.Name} play {special.GetCardNameDisplay()}: {special.GetEffectDescription()}");
            Console.ResetColor();
            if (special.Type == CardType.Joker)
            {
                if (special.Rank == 16)
                {
                    p1.IsAutoGreatSuccess = true; //Big joker
                }
                else
                {
                    p1.IsAutoSuccess = true; //Small joker
                }
                return;
            }
            switch (special.Suit) //check suit of Ace
            {
                case Suit.Hearts: //heal
                    p1.Heal(1);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  {p1.Name} heal 1HP - current: {p1.HP}/{Player.MaxHP}");
                    Console.ResetColor();
                    break;
                case Suit.Diamonds: //draw card
                    DoDiamondsEffect(p1, env);
                    break;
                case Suit.Spades: //curse
                    p2.BeCursed = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  {p2.Name} is cursed! - total score this round -20%.");
                    Console.ResetColor();
                    break;
                case Suit.Clubs: //do not lose HP
                    p1.DonotLoseHP = true;
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine($"  {p1.Name} do not lose HP this round!");
                    Console.ResetColor();
                    break;
            }
        }
        public void DoDiamondsEffect(Player player, EnvironmentCard env)
        {
            List<Card> resources = player.GetResourceCards();
            if (resources.Count == 0)
            {
                Console.WriteLine("  No resource cards in hand.");
                return;
            }
            Card discard;
            if (player.IsHuman)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("  Choose which card to discard?");
                Console.ResetColor();
                for (int i = 0; i < resources.Count; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  [{i + 1}] {resources[i].GetCardNameDisplay()}");
                    Console.ResetColor();
                }
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"  input [0-{resources.Count}]");
                Console.ResetColor();
                discard = resources[0]; // default, overwritten below
                while (true)
                {
                    string input = Console.ReadLine()?.Trim() ?? ""; //wait for player to choose
                    if (int.TryParse(input, out int number)) //if input number key => convert to int
                    {
                        //if it's vaild key:
                        if (number >= 1 && number <= resources.Count)
                        {
                            discard = resources[number - 1];
                            break;
                        }
                        Console.Write("  invalid input, please try again.");
                    }
                }
            }
            else
            {
                discard = AIPlayer.DecideDiscardCard(player, env);
                Console.WriteLine($"  AI Player choose to discard {discard.GetCardNameDisplay()}");
            }
            player.RemoveFromHand(discard);
            Deck.Discard(discard);
            Card draw = Deck.DrawCard(); //draw a new card
            player.AddToHand(draw);
        }
        public void ResultPhase(Player p1, Player p2, EnvironmentCard env)
        {
            Console.Clear();
            //1.reveal played cards
            Reveal(p1);
            Reveal(p2);
            //2.calculate score
            PlayerRoundResult resultP1 = RoundCalculator.Calculate(p1, env, p1.BeCursed);
            PlayerRoundResult resultP2 = RoundCalculator.Calculate(p2, env, p2.BeCursed);
            //3.display score this round
            DisplayScore(resultP1, env);
            DisplayScore(resultP2, env);
            //4.HP calculate
            ApplyHPChanges(resultP1, resultP2);
            //print status
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"  {env.Day} day ends:");
            Console.WriteLine($"  {p1.GetPlayerStatus()}");
            Console.WriteLine($"  {p2.GetPlayerStatus()}");
            Console.ResetColor();
        }
        public void Reveal(Player p)
        {
            Console.WriteLine($"  {p.Name} plays:");
            if (p.PlayedSpecialCards != null)
            {
                Console.WriteLine($"  Special Card: {p.PlayedSpecialCards.GetCardNameDisplay()}");
            }
            if (p.PlayedResourceCards != null)
            {
                Console.WriteLine(string.Join("  ", p.PlayedResourceCards.Select(c => c.GetCardNameDisplay())));
            }
        }
        public void DisplayScore(PlayerRoundResult r, EnvironmentCard env)
        {
            Console.WriteLine($"  {r.Player.Name}'s Score:");
            foreach (string line in r.Log)
            {
                Console.WriteLine(line); //print calculate log
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Final Score: {r.FinalTotal}   Result: {r.Outcome}");
            Console.ResetColor();
        }
        public string Outcome(RoundOutcome o) => o switch
        {
            RoundOutcome.GreatSuccess => "★ Great Success!",
            RoundOutcome.Success => "✓ Normal Success!",
            RoundOutcome.Failure => "✗ Failure."
        };
        public void ApplyHPChanges(PlayerRoundResult r1, PlayerRoundResult r2)
        {
            //1.check failure => reduce HP:
            FailureDamage(r1);
            FailureDamage(r2);
            //2.check great success => reduce HP:
            bool p1GreatSuccess = r1.Outcome == RoundOutcome.GreatSuccess;
            bool p2GreatSuccess = r2.Outcome == RoundOutcome.GreatSuccess;
            if (p1GreatSuccess && !p2GreatSuccess)
            {
                if (r2.Player.DonotLoseHP)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"  but refuse to lose HP because of special card effect");
                    Console.ResetColor();
                }
                else
                {
                    r2.Player.TakeDamage(1);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  {r1.Player.Name} Great Success, but {r2.Player.Name} didn't -- Lose 1 HP!");
                    Console.ResetColor();
                }
            }
            else if (!p1GreatSuccess && p2GreatSuccess)
            {
                if (r1.Player.DonotLoseHP)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"  but refuse to lose HP because of special card effect");
                    Console.ResetColor();
                }
                else
                {
                    r1.Player.TakeDamage(1);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"  {r2.Player.Name} Great Success, but {r1.Player.Name} didn't -- Lose 1 HP!");
                    Console.ResetColor();
                }
            }
            else if (p1GreatSuccess && p2GreatSuccess)
            {
                Console.WriteLine("  Both sides get their Great Success, so no one lose HP.");
            }
        }
        public void FailureDamage(PlayerRoundResult r)
        {
            if (r.Outcome != RoundOutcome.Failure) //do not fail
            {
                return;
            }
            if (r.Player.DonotLoseHP)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"  {r.Player.Name} fails, but refuse to lose HP because of special card effect");
                Console.ResetColor();
            }
            else
            {
                r.Player.TakeDamage(1);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  {r.Player.Name} fails, lose 1 HP!");
                Console.ResetColor();

            }
        }
        public void DeclareWinner()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  ══════════════════════════════════════════");
            Console.WriteLine("              GAME OVER！");
            Console.WriteLine("  ══════════════════════════════════════════");
            if (P1.IsAlive && !P2.IsAlive)
            {
                AnnounceWinner("  P1 still alive", P1);
            }
            else if (!P1.IsAlive && P2.IsAlive)
            {
                AnnounceWinner("  P2 still alive", P2);
            }
            else if (!P1.IsAlive && !P2.IsAlive)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Both sides'HP become 0 at the same time - Draw!");
                Console.ResetColor();
            }
            else
            {
                //both alive, compare HP
                if (P1.HP > P2.HP)
                {
                    AnnounceWinner("  HP: P1 > P2", P1);
                }
                else if (P1.HP < P2.HP)
                {
                    AnnounceWinner("  HP: P2 > P1", P2);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  HP: P1 = P2 - Draw!");
                    Console.ResetColor();
                }
            }
            Console.ResetColor();
        }
        public void AnnounceWinner(string reason, Player winner)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  {reason}, {winner.Name} win!");
            Console.ResetColor();
        }
        public void PrivacyScreen(Player p1, Player p2, EnvironmentCard env)
        {
            Console.WriteLine($">>> {p2.Name} please don't look at the screen. When ready, {p1} press Enter <<<");
            Console.ReadKey();
            Console.Clear();
            DisplayRoundStart(env.Day);
            env.Display();
        }
        public static void Pause(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadLine();
        }
    }
}