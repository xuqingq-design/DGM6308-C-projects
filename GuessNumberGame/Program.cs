using System;
//requirement D: Betting System
int coins = 100;
Console.WriteLine($"Your initial balance is: ${coins}");
while (coins > 0)
{
    Console.WriteLine($"Your current balance is: ${coins}");
    Console.WriteLine("Enter your bet amount");
    bool betValid = int.TryParse((Console.ReadLine()).Trim(), out int betAmount); //Attempt to convert the player's input into a number. If the result is an integer, store it in `betAmount` with a boolean value of `true`; otherwise, set it to `false`.
    if (!betValid || betAmount <= 0 || betAmount > coins) //Determine whether the blocked amount is reasonable?
    {
        Console.WriteLine("Invalid amount, default value set to $10.");
        betAmount = 10;
    }

    //requirement B: Select Range
    Console.WriteLine("Select difficulty level:");
    Console.WriteLine("(1) 1-100; (2) 1-200; (3) 1-300");
    Console.WriteLine("Enter 1, 2, or 3 (Any input beyond these will default to 100) ...");
    string choice = (Console.ReadLine()).Trim(); //.Trim() means delete the "spaces" entered by the player
    int maxRange = 100;
    if (choice == "2") maxRange = 200;
    else if (choice == "3") maxRange = 300;
    int value = Random.Shared.Next(1, maxRange + 1); //Select a ramdom integer between 1 and maxRange.
    Console.WriteLine($"The number has been generated between 1 and {maxRange}."); //$: makes {maxRange} a specific number(100,200,300)

    //requirement A: Set maximum number of guesses
    int maxGuesses = maxRange / 20; //100--5times; 200--10times; 300--15times
    int remainingGuesses = maxGuesses;
    bool playerWin = false;
    while (remainingGuesses > 0)
    {
        Console.Write($"Guess a number (1-{maxRange}): ");
        Console.WriteLine($"Remaining Guesses: {remainingGuesses}."); //Inform players of the remaining guesses.
        bool valid = int.TryParse((Console.ReadLine() ?? "").Trim(), out int input); //Attempt to remove spaces from the player's input and convert it to a number. If successful, store the converted integer in `input` and set the boolean `valid` to true. If unsuccessful, set `valid` to false.
        if (!valid) Console.WriteLine("Invalid.");
        else if (input == value)
        {
            playerWin = true;
            break;
        }
        else
        {
            remainingGuesses--;
            //requirement C : Provide “cold, warm, hot” prompts based on difference.
            int difference = Math.Abs(input - value); //Calculate the absolute value to ensure it is a positive number.
            string temperature = "";
            if (difference > 30) temperature = "cold..."; //Determine the magnitude of the difference.
            else if (difference > 15) temperature = "warm.";
            else if (difference > 5) temperature = "hot!";
            else temperature = "hotter!!!";
            Console.WriteLine($"Incorrect. Too {(input < value ? "Low" : "High")}. {temperature}");
        }
    }
    if (playerWin) //Determine whether the player guessed the number correctly before running out of remaining Guesses.
    {
        coins += betAmount;
        Console.WriteLine($"You guessed it! You win ${betAmount}.");
    }
    else
    {
        coins -= betAmount;
        Console.WriteLine($"You lose. The correct number was {value}. You lose ${betAmount}.");
    }
    if (coins <= 0)
    {
        Console.WriteLine("You're bankrupt.GameOver.");
        break;
    }
    Console.WriteLine("Want to continue playing? input(Y/N)");
    if (Console.ReadLine().ToLower() != "y") break;
}
Console.Write("Press any key to exit...");
Console.ReadKey(true);