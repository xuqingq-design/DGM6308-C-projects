//main program entry: start first
//choose game mode(PVE/PVE), create game menu => game engine.run()
using System;

namespace TribalGame //as a main folder
{
    class Program
    {
        static void Main(string[] args) //static: don't need to create objects to use; string[] args: need it to use 'Main' in C# 
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; //display symbols
            while (true)
            {
                Console.Clear();
                Console.WriteLine("    Tribal: Trial of the Wild");
                Console.WriteLine("    choose game mode:");
                Console.WriteLine("    [1] PVE");
                Console.WriteLine("    [2] PVP");
                Console.WriteLine("    [0] QUIT");
                bool PVEmode = true;
                bool modeChosen = false;
                while (!modeChosen)
                {
                    string key = Console.ReadLine()?.Trim() ?? "";
                    switch (key)
                    {
                        case "1":
                            PVEmode = true;
                            modeChosen = true;
                            break;
                        case "2":
                            PVEmode = false;
                            modeChosen = true;
                            break;
                        case "0":
                            Console.WriteLine("    game ends");
                            return;
                        default:
                            Console.WriteLine("    invaild key");
                            break;
                    }
                }
                var engine = new GameEngine(PVEmode); //pass bool result to GameEngine
                engine.Run(); //use engine.Run() in GameEngine.cs
                //after 1 game play:
                Console.WriteLine("    Play again? [Y/N]:");
                ConsoleKeyInfo again = Console.ReadKey(true);
                if(again.KeyChar != 'y' && again.KeyChar != 'Y')
                {
                    break;
                }
            }
            Console.WriteLine("    game ends");
        }
    }
}