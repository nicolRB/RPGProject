using System;
using RPGProject.Data;
using RPGProject.Entities;

namespace RPGProject.Systems
{
    public static class CreativeMode
    {
        public static void Start()
        {
            Console.Clear();
            Console.WriteLine("Modo Criativo - Escolha um inimigo:\n");

            // Carrega monstros do banco (caso ainda não tenha sido feito)
            var monsters = MonsterDatabase.Monsters;

            // Exibe monstros numerados a partir de 1
            for (int i = 0; i < monsters.Count; i++)
            {
                Monster monster = monsters[i];
                Console.WriteLine($"[{i + 1}] - {monster.Name} (HP: {monster.HP}, ATK: {monster.Attack}, DMG: {monster.Damage}, RANGE: {monster.Range})");
            }

            Console.WriteLine("[0] - Voltar ao menu\n");

            int choice;
            while (true)
            {
                string? input = Console.ReadLine();

                if (!int.TryParse(input, out choice) || choice < 0 || choice > monsters.Count)
                {
                    Console.WriteLine("Escolha inválida! Tente novamente.");
                    continue;
                }
                break;
            }

            if (choice == 0)
                return;

            // Ajusta o índice (pois a listagem começa em 1)
            Monster selectedMonster = monsters[choice - 1];

            // Estatísticas básicas do jogador (ajustáveis)
            int playerHP = 20;
            int playerDefense = 2;
            int playerAttack = 0;
            int playerDamage = 0;

            // Inicia o combate
            CombatSystem.StartCombat(selectedMonster, playerHP, playerDefense, playerAttack, playerDamage, true);

            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
            Console.ReadKey();
        }
    }
}
