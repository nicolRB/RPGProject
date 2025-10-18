using RPGProject.Data;
using RPGProject.Entities;
using RPGProject.Utils;
using System;
using System.Numerics;

namespace RPGProject.Systems
{
    public static class CreativeMode
    {
        public static void Start()
        {
            List<Weapon> allWeapons = WeaponDatabase.Weapons;
            var monsters = MonsterDatabase.Monsters;
            var player = PlayerDatabase.Player;
            bool chose = false;
            player.MaxHP = 20;
            player.CurrentHP = 20;
            player.Attack = 0;
            player.Damage = 0;
            player.Defense = 0;
            player.Speed = 2;
            PlayerDatabase.ClearWeapons();
            PlayerDatabase.AddWeapon(0);
            PlayerDatabase.AddWeapon(1);
            PlayerDatabase.Save();

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
                Console.WriteLine($"[{monsters.Count + 2}] - Configurar inventário do personagem");
                Console.WriteLine("[0] - Voltar ao menu principal");

                int choice = InputUtils.ChooseOptionAllowZero(monsters.Count + 2);

                if (choice == 0) return;
                else if (choice == monsters.Count + 1)
                {
                    ConfigurarStatus();
                }
                else if (choice == monsters.Count + 2)
                {
                    ConfigurarInventario();
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
                Console.WriteLine("[0] - Voltar");

                int choice = InputUtils.ChooseOptionAllowZero(4);
                int novoValor = 0;

                if (choice != 0)
                {
                    Console.Write("\nNovo valor: ");
                    novoValor = InputUtils.ChooseValue();
                }

                if (novoValor == 0 && choice == 1)
                {
                    Console.WriteLine("<HP nao pode ser 0. Pressione uma tecla para continuar>");
                    Console.ReadKey();
                    continue;
                }

                switch (choice)
                {
                    case 0:
                        configurando = false;
                        PlayerDatabase.Save();
                        continue;
                    case 1:
                        player.MaxHP = novoValor;
                        player.CurrentHP = novoValor;
                        break;
                    case 2:
                        player.Defense = novoValor;
                        break;
                    case 3:
                        player.Attack = novoValor;
                        break;
                    case 4:
                        player.Damage = novoValor;
                        break;
                }

                PlayerDatabase.Save();
                Console.WriteLine("\nAtributo atualizado! Pressione uma tecla para continuar...");
                Console.ReadKey();
            }
        }

        public static void ConfigurarInventario()
        {
            bool configurando = true;
            var player = PlayerDatabase.Player;

            while (configurando)
            {
                Console.Clear();
                Console.WriteLine("=== CONFIGURAÇÃO DE INVENTARIO ===");
                Console.WriteLine("[1] - Itens");
                Console.WriteLine("[2] - Armas");
                Console.WriteLine("[0] - Voltar");

                int choice = InputUtils.ChooseOptionAllowZero(2);

                switch (choice)
                {
                    case 0:
                        configurando = false;
                        break;

                    case 1:

                        break;

                    case 2:
                        DefinirArmas();
                        break;
                }
            }
        }

        public static void DefinirItems()
        {
            bool configurando = true;
            var player = PlayerDatabase.Player;

        }

        public static void DefinirArmas()
        {
            bool configurando = true;
            var player = PlayerDatabase.Player;
            List<Weapon> allWeapons = WeaponDatabase.Weapons;
            var ownedWeapons = new List<Weapon>();

            foreach (var wEntry in player.Inventory.Weapons)
            {
                Weapon? w = allWeapons.Find(x => x.Id == wEntry.Id);
                if (w != null)
                    ownedWeapons.Add(w);
            }

            while (configurando)
            {
                Console.Clear();
                Console.WriteLine("===Set de armas atuais===\n");

                for (int i = 0; i < ownedWeapons.Count; i++)
                    Console.WriteLine($"{i + 1}. {ownedWeapons[i].Name}");

                Console.WriteLine("\n[1] - Adicionar Arma\n[2] - Remover Arma\n[0] - Voltar");

                int choice = InputUtils.ChooseOptionAllowZero(2);

                switch(choice) { 
                    case 0:
                        configurando = false;
                        break;

                    case 1:
                        AdicionarArma(ownedWeapons);
                        break;

                    case 2:
                        RemoverArma(ownedWeapons);
                        break;

                    default:
                        Console.WriteLine("\n<Erro de input>\n");
                        break;
                }
            }

            
        }

        public static void AdicionarArma(List<Weapon> inventory)
        {

        }

        public static void RemoverArma(List<Weapon> inventory)
        {
            bool configurando = true;

            while(configurando && inventory.Count > 0)
            {
                Console.Clear();
                Console.WriteLine("===Remover Arma===\n");

                for (int i = 0; i < inventory.Count; i++)
                    Console.WriteLine($"[{i + 1}] - {inventory[i].Name}");
                Console.WriteLine("[0] - Cancelar");

                int choice = InputUtils.ChooseOptionAllowZero(inventory.Count);

                if (choice == 0) configurando = false;
                else
                {
                    PlayerDatabase.RemoveWeapon(choice - 1);
                    PlayerDatabase.Save();
                    Console.WriteLine("\nInventario atualizado! Pressione uma tecla para continuar...");
                    Console.ReadKey();
                }
            }
            if(inventory.Count == 0) Console.WriteLine("Nao ha armas no inventario.");
        }
    }
}
