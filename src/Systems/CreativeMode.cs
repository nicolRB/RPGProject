using System;
using RPGProject.Data;
using RPGProject.Entities;

namespace RPGProject.Systems
{
    public static class CreativeMode
    {
        public static void Start()
        {
            var monsters = MonsterDatabase.Monsters;
            var player = PlayerDatabase.Player;
            bool chose = false;
            player.MaxHP = 20;
            player.CurrentHP = 20;
            player.Attack = 0;
            player.Damage = 0;
            player.Defense = 0;
            player.Speed = 2;

            while (!chose)
            {
                player = PlayerDatabase.Player;
                Console.Clear();

                Console.WriteLine("=== MODO CRIATIVO ===\nEscolha um inimigo:\n");

                for (int i = 0; i < monsters.Count; i++)
                {
                    Monster monster = monsters[i];
                    Console.WriteLine($"[{i + 1}] - {monster.Name} (HP: {monster.HP}, ATK: {monster.Attack}, DMG: {monster.Damage}, RANGE: {monster.Range})");
                }

                Console.WriteLine($"[{monsters.Count + 1}] - Configurar status do personagem");
                Console.WriteLine("[0] - Voltar ao menu principal");

                int choice;
                while (true)
                {
                    Console.Write("\nOpção: ");
                    string? input = Console.ReadLine();

                    if (!int.TryParse(input, out choice) || choice < 0 || choice > monsters.Count + 1)
                    {
                        Console.WriteLine("Escolha inválida! Tente novamente.");
                        continue;
                    }
                    break;
                }

                if (choice == 0) return;
                else if (choice == monsters.Count + 1)
                {
                    ConfigurarStatus();
                }
                else
                {
                    chose = true;
                    Monster selectedMonster = monsters[choice - 1];

                    CombatSystem.StartCombat(selectedMonster, player, true);

                    Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
                    Console.ReadKey();
                }
            }
        }

        public static void ConfigurarStatus()
        {
            bool configurando = true;
            var player = PlayerDatabase.Player;

            while (configurando)
            {
                Console.Clear();
                Console.WriteLine("=== CONFIGURAÇÃO DE STATUS ===");
                Console.WriteLine($"[1] - HP: {player.MaxHP}");
                Console.WriteLine($"[2] - Defesa: {player.Defense}");
                Console.WriteLine($"[3] - Ataque: {player.Attack}");
                Console.WriteLine($"[4] - Dano: {player.Damage}");
                Console.WriteLine("[0] - Voltar\n");

                Console.Write("Escolha um atributo para modificar: ");
                string? input = Console.ReadLine();
                if (!int.TryParse(input, out int opcao) || opcao < 0 || opcao > 4)
                {
                    Console.WriteLine("Opção inválida! Pressione uma tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                if (opcao == 0)
                {
                    configurando = false;
                    PlayerDatabase.Save();
                    continue;
                }

                Console.Write("Novo valor: ");
                string? novoValor = Console.ReadLine();

                if (!int.TryParse(novoValor, out int valor))
                {
                    Console.WriteLine("Valor inválido! Pressione uma tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                switch (opcao)
                {
                    case 1:
                        player.MaxHP = valor;
                        player.CurrentHP = Math.Min(player.CurrentHP, player.MaxHP);
                        break;
                    case 2:
                        player.Defense = valor;
                        break;
                    case 3:
                        player.Attack = valor;
                        break;
                    case 4:
                        player.Damage = valor;
                        break;
                }

                PlayerDatabase.Save();
                Console.WriteLine("Atributo atualizado! Pressione uma tecla para continuar...");
                Console.ReadKey();
            }
        }
    }
}
