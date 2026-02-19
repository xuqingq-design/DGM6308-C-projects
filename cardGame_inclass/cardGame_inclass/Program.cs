using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

int randomNumberPla;
int randomNumberCom;
int score = 0;
int scoreCom = 0;
int turn = 1;
List<int> cardDesk = new List<int>();

while (true)
{
    if (turn >= 26)
    {
        Console.WriteLine($"game ends, your final score is {score}, opponent score is {scoreCom}");
        if (score > scoreCom)
        {
            Console.WriteLine("you win");
        }
        else if (score < scoreCom)
        {
            Console.WriteLine("you lose");
        }
        else
        {
            Console.WriteLine("draw");
        }
        Console.WriteLine("presss any key to quit");
        while (Console.ReadKey(true).Key == ConsoleKey.Enter)
        {
            break;
        }
    }
    Console.Clear();
    Console.WriteLine($"turn{turn}: your score is {score}");
    Console.WriteLine("press enter to begin");
    while (Console.ReadKey(true).Key == ConsoleKey.Enter)
    {
        break;
    }
    randomNumberPla = cardDesk[Random.Shared.Next(0, cardDesk.Count)];
    randomNumberCom = cardDesk[Random.Shared.Next(0, cardDesk.Count)];
    cardDesk.Remove(randomNumberPla);
    cardDesk.Remove(randomNumberCom);
    Console.WriteLine($"your card number is: {randomNumberPla}");
    Console.WriteLine("press enter to compare sizes");
    while (Console.ReadKey(true).Key == ConsoleKey.Enter)
    {
        break;
    }
    if (randomNumberPla > randomNumberCom)
    {
        Console.WriteLine($"your card number is: {randomNumberPla}, opponet card number is: {randomNumberCom}");
        Console.WriteLine("player win");
        score += 1;
    }
    else if (randomNumberPla < randomNumberCom)
    {
        Console.WriteLine($"your card number is: {randomNumberPla}, opponet card number is: {randomNumberCom}");
        Console.WriteLine("computer win");
        scoreCom += 1;
    }
    else
    {
        Console.WriteLine($"your card number is: {randomNumberPla}, opponet card number is: {randomNumberCom}");
        Console.WriteLine("draw");
    }
    Console.WriteLine("press 1 to play again. presss 0 to quit");
    switch (Console.ReadKey(true).Key)
    {
        case ConsoleKey.D1:
            turn++;
            break; //back to begining
        case ConsoleKey.D0:
            return;
        default:
            continue;
    }
}