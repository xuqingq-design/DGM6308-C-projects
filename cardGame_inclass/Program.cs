using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualBasic;

int randomNumberPla;
int randomNumberCom;
int score = 0;
int scoreCom = 0;
int turn = 1;
List<int> cardDesk = new List<int>();
ShuffleDesk(cardDesk);
List<int> playerDesk = cardDesk.Take(26).ToList();
List<int> computerDesk = cardDesk.Skip(26).ToList();

while (true)
{
    if (turn > 26)
    {
        Console.WriteLine($"game ends, your final score is {score}, computer score is {scoreCom}");
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
        Console.WriteLine("press any key to quit");
        Console.ReadKey();
        return;
    }

    Console.Clear();
    Console.WriteLine($"turn{turn}: your score is {score}, computer score is {scoreCom}");

    //debug:
    //for (int i = 0; i < cardDesk.Count; i++)
    //{
    //Console.Write($"desk: {cardDesk[i]}  ");
    //}
    for (int i = 0; i < playerDesk.Count; i++)
    {
        Console.Write($"player: {playerDesk[i]}  ");
        Console.Write($"computer: {computerDesk[i]}  ");
    }

    Console.WriteLine("press enter to begin");
    while (Console.ReadKey(true).Key != ConsoleKey.Enter)
    {
        Console.WriteLine("wrong key, press again");
    }
    randomNumberPla = playerDesk[Random.Shared.Next(0, playerDesk.Count)];
    randomNumberCom = computerDesk[Random.Shared.Next(0, computerDesk.Count)];
    playerDesk.Remove(randomNumberPla);
    computerDesk.Remove(randomNumberCom);
    //convert number to card number:
    string cardNumberPla;
    string cardNumberCom;
    if (randomNumberPla == 1)
    {
        cardNumberPla = "A";
    }
    else if (randomNumberPla == 11)
    {
        cardNumberPla = "J";
    }
    else if (randomNumberPla == 12)
    {
        cardNumberPla = "Q";
    }
    else if (randomNumberPla == 13)
    {
        cardNumberPla = "K";
    }
    else
    {
        cardNumberPla = randomNumberPla.ToString();
    }
    if (randomNumberCom == 1)
    {
        cardNumberCom = "A";
    }
    else if (randomNumberCom == 11)
    {
        cardNumberCom = "J";
    }
    else if (randomNumberCom == 12)
    {
        cardNumberCom = "Q";
    }
    else if (randomNumberCom == 13)
    {
        cardNumberCom = "K";
    }
    else
    {
        cardNumberCom = randomNumberCom.ToString();
    }

    Console.WriteLine($"your card:");
    Console.WriteLine($"┌───────┐",
                        "│███████│",
                        "│███████│",
                        "│███████│",
                        "│███████│",
                        "│███████│",
                        "└───────┘",);
    Console.WriteLine("press enter to face up your card");
    while (Console.ReadKey(true).Key != ConsoleKey.Enter)
    {
        Console.WriteLine("wrong key, press again");
    }
    if (randomNumberPla > randomNumberCom)
    {
        string a = cardNumberPla;
        Console.WriteLine("┌───────┐",
                         $"│{a}████│",
                           "│███████│",
                           "│███████│",
                           "│███████│",
                           $"│████{a}│",
                           "└───────┘",);
        Console.WriteLine($"your card number is: {cardNumberPla}, computer card number is: {cardNumberCom}");
        Console.WriteLine("player win");
        score += 1;
    }
    else if (randomNumberPla < randomNumberCom)
    {
        Console.WriteLine($"your card number is: {cardNumberPla}, computer card number is: {cardNumberCom}");
        Console.WriteLine("computer win");
        scoreCom += 1;
    }
    else
    {
        Console.WriteLine($"your card number is: {cardNumberPla}, computer card number is: {cardNumberCom}");
        Console.WriteLine("draw");
    }
    Console.WriteLine("press any key to continue");
    Console.ReadKey(true);
    turn++;
}

void ShuffleDesk(List<int> desk)
{
    desk.Clear();
    for (int p = 0; p < 4; p++)
    {
        for (int i = 1; i <= 13; i++)
        {
            desk.Add(i);
        }
    }
    for (int i = 0; i < desk.Count; i++)
    {
        int j = Random.Shared.Next(desk.Count);
        int temp = desk[i];
        desk[i] = desk[j];
        desk[j] = temp;
    }
    //debug:
    //for (int i = 0; i < desk.Count; i++)
    //{
    //Console.Write($"desk: {desk[i]}  ");
    //}
}