using System;
using System.Text;

namespace RPGProject.Utils
{
    public static class InputUtils
    {
        public static int ReadInt(string prompt = "> ")
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out value))
                    return value;

                Console.WriteLine("\n<Entrada inválida. Digite um número>");
            }
        }

        public static int ChooseOption(int range, string message = "\n")
        {
            int choice;
            do
            {
                choice = ReadInt(message);
                if (choice < 1 || choice > range)
                    Console.WriteLine($"\n<Escolha um número entre 1 e {range}>");
            }
            while (choice < 1 || choice > range);

            return choice;
        }

        public static int ChooseOptionAllowZero(int range, string message = "\n")
        {
            int choice;
            do
            {
                choice = ReadInt(message);
                if (choice < 0 || choice > range)
                    Console.WriteLine($"\n<Escolha um número entre 0 e {range}>");
            }
            while (choice < 0 || choice > range);

            return choice;
        }

        public static int ChooseValue(string message = "\n")
        {
            int choice;
            do
            {
                choice = ReadInt(message);
                if (choice < 0)
                    Console.WriteLine($"\n<Escolha um número positivo>");
            }
            while (choice < 0);

            return choice;
        }

        public static void WaitForKey(string message = "Pressione qualquer tecla para continuar...")
        {
            Console.WriteLine(message);
            Console.ReadKey(true);
        }
    }
}
