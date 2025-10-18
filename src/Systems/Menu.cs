using RPGProject.Data;
using RPGProject.Utils;
using System;

namespace RPGProject.Systems
{
    public static class Menu
    {
        public static void Show()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine(AsciiArtLoader.Load("Title"));
                Console.WriteLine("RPG Project - Alpha 0.5.7");
                Console.WriteLine("\n\nBem vindo à tela inicial.\nAperte qualquer tecla para continuar.");
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("\nEscolha seu caminho:");
                Console.WriteLine("[1] - Masmorra Infinita (em desenvolvimento)");
                Console.WriteLine("[2] - Escolher inimigo (modo criativo)");
                Console.WriteLine("[3] - Sair");

                int choice = InputUtils.ChooseOption(3);

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("\nMasmorra Infinita ainda não implementada.");
                        Console.WriteLine("Pressione qualquer tecla para voltar...");
                        Console.ReadKey();
                        break;

                    case 2:
                        CreativeMode.Start();
                        break;

                    case 3:
                        running = false;
                        Console.WriteLine("\nSaindo do jogo...");
                        Console.WriteLine("Pressione qualquer tecla para fechar.");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("\n<Erro de input>\n");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
