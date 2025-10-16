using RPGProject.Data;
using RPGProject.Entities;
using RPGProject.Utils;
using System;
using System.Collections.Generic;
using System.Threading;

namespace RPGProject.Systems
{
    public static class CombatSystem
    {
        public static void StartCombat(Monster monster, PlayerData player, bool escapable)
        {
            Console.Clear();

            int currentPlayerHP = player.CurrentHP;
            int currentMonsterHP = monster.HP;
            int proximity = 0;
            int roomSize = 3;
            int parryValue = 2;
            int stunTurns = 0;
            bool defendingThisTurn = false;

            Console.WriteLine($"Início do combate: {monster.Name} aparece!");
            Console.WriteLine($"HP: {currentMonsterHP}\nATK: {monster.Attack}\nDMG: {monster.Damage}\nDEF: {monster.Defense}({monster.Defense + 6} min)\nRANGE: {monster.Range}");
            Console.WriteLine($"\nSeus status:\nHP: {currentPlayerHP}/{player.MaxHP}\nDEF: {player.Defense}({player.Defense + 6} min)\nATK: +{player.Attack}\nDMG: +{player.Damage}");

            // Carrega armas possuídas
            List<Weapon> allWeapons = WeaponDatabase.Weapons;
            var ownedWeapons = new List<Weapon>();

            foreach (var wEntry in player.Inventory.Weapons)
            {
                Weapon? w = allWeapons.Find(x => x.Id == wEntry.Id);
                if (w != null)
                    ownedWeapons.Add(w);
            }

            int round = 1;
            bool combatEnded = false;

            while (currentPlayerHP > 0 && currentMonsterHP > 0 && !combatEnded)
            {
                Console.WriteLine($"\n\n--- Round {round} ---");
                Console.WriteLine($"Seu HP: {currentPlayerHP}");
                Console.WriteLine($"HP de {monster.Name}: {currentMonsterHP}");
                Console.WriteLine($"Distância: {proximity}/{roomSize}\n");

                // --- Turno do jogador ---
                bool turnDone = false;
                Console.WriteLine("-Seu Turno-\n");
                while (!turnDone)
                {
                    Console.WriteLine("Escolha sua ação:");
                    Console.WriteLine(escapable
                        ? "[1] - Atacar\n[2] - Defender\n[3] - Se Afastar\n[4] - Se Aproximar\n[5] - Escapar\n"
                        : "[1] - Atacar\n[2] - Defender\n[3] - Se Afastar\n[4] - Se Aproximar\n");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Weapon? selectedWeapon = ChooseWeapon(ownedWeapons);
                            if (selectedWeapon == null)
                            {
                                Console.WriteLine("\nVoltando ao menu de ações...\n");
                                continue;
                            }

                            int weaponATK = selectedWeapon.Attack;
                            int weaponDMG = selectedWeapon.Damage;
                            int weaponRange = selectedWeapon.Range;
                            string weaponName = selectedWeapon.Name;

                            if (proximity <= weaponRange)
                            {
                                int playerRoll = RollAttack(stunTurns, player.Attack + weaponATK);
                                int totalPlayerRoll = playerRoll + player.Attack + weaponATK;
                                int minToHit = 6 + monster.Defense;
                                int critDmg = (int)((weaponDMG + player.Damage) * 1.7);

                                if (playerRoll == 10)
                                {
                                    currentMonsterHP -= critDmg;
                                    Console.WriteLine($"\nVocê acerta um ponto vital com {weaponName}, causando {critDmg} de dano!\n");
                                }
                                else if (totalPlayerRoll >= minToHit)
                                {
                                    int damage = weaponDMG + player.Damage;
                                    currentMonsterHP -= damage;
                                    Console.WriteLine($"\nVocê ataca com {weaponName} causando {damage} de dano!");
                                }
                                else
                                {
                                    Console.WriteLine($"\nVocê erra o ataque com {weaponName}!");
                                }

                                currentMonsterHP = Math.Max(0, currentMonsterHP);
                                Console.WriteLine($"\nhit {(playerRoll == 10 ? $"*{totalPlayerRoll}*" : $"{totalPlayerRoll}")} | {minToHit} def inimiga\n");
                                turnDone = true;
                            }
                            else
                            {
                                Console.WriteLine($"\nVocê está longe demais para usar {weaponName}!\n");
                            }
                            break;

                        case "2":
                            defendingThisTurn = true;
                            Console.WriteLine($"\nVocê se prepara para aparar ataques futuros (+{parryValue} DEF temporário).\n");
                            turnDone = true;
                            break;

                        case "3":
                            if (proximity < roomSize)
                            {
                                proximity = Math.Min(roomSize, proximity + 2);
                                Console.WriteLine($"\nVocê se afasta, distância agora {proximity}.\n");
                                turnDone = true;
                            }
                            else Console.WriteLine("\nVocê não pode se afastar mais.\n");
                            break;

                        case "4":
                            if (proximity > 0)
                            {
                                proximity = Math.Max(0, proximity - 2);
                                Console.WriteLine($"\nVocê se aproxima, distância agora {proximity}.\n");
                                turnDone = true;
                            }
                            else Console.WriteLine("\nVocê já está frente a frente.\n");
                            break;

                        case "5":
                            if (escapable)
                            {
                                Console.WriteLine("\nVocê fugiu do combate!");
                                combatEnded = true;
                                turnDone = true;
                            }
                            else Console.WriteLine("Ação inválida!");
                            break;

                        default:
                            Console.WriteLine("Ação inválida!");
                            break;
                    }
                }

                if (currentMonsterHP <= 0 || combatEnded) break;

                // --- Turno do monstro ---
                Console.WriteLine("-Turno Inimigo-\n");
                if (stunTurns > 0)
                {
                    Console.WriteLine($"{monster.Name} está atordoado e não consegue agir!");
                    stunTurns--;
                }
                else
                {
                    if (proximity <= monster.Range)
                    {
                        int monsterRoll = RandomUtils.Roll(10);
                        int totalMonsterRoll = monsterRoll + monster.Attack;
                        int minToHit = 6 + player.Defense + (defendingThisTurn ? parryValue : 0);

                        if (monsterRoll == 10)
                        {
                            int dmg = (int)(monster.Damage * 1.7);
                            currentPlayerHP -= dmg;
                            Console.WriteLine($"{monster.Name} acerta um ponto vulnerável! Você sofre {dmg} de dano.");
                        }
                        else if (totalMonsterRoll >= minToHit)
                        {
                            currentPlayerHP -= monster.Damage;
                            Console.WriteLine($"{monster.Name} acerta você causando {monster.Damage} de dano.");
                        }
                        else if (defendingThisTurn && totalMonsterRoll < minToHit)
                        {
                            Console.WriteLine($"Você aparou o ataque do {monster.Name}, atordoando-o!");
                            stunTurns++;
                        }
                        else
                        {
                            Console.WriteLine($"{monster.Name} erra o ataque!");
                        }

                        currentPlayerHP = Math.Max(0, currentPlayerHP);
                        Console.WriteLine($"\n{(defendingThisTurn ? "def+ " : "def ")}{minToHit} | {(monsterRoll == 10 ? $"*{totalMonsterRoll}*" : $"{totalMonsterRoll} hit")}");
                    }
                    else
                    {
                        proximity = Math.Max(0, proximity - monster.Speed);
                        Console.WriteLine($"\n{monster.Name} se aproxima para {proximity} de distância.");
                    }
                }

                defendingThisTurn = false;
                round++;
                Thread.Sleep(500);
            }

            // --- Resultado ---
            if (currentPlayerHP > 0 && !combatEnded)
                Console.WriteLine($"Você venceu {monster.Name}!");
            else if (currentPlayerHP > 0)
                Console.WriteLine("\nCombate terminou sem vencedor.");
            else
                Console.WriteLine($"\nVocê foi derrotado por {monster.Name}...");

            // Atualiza HP e salva
            player.CurrentHP = Math.Max(0, currentPlayerHP);
            PlayerDatabase.Save();
        }

        private static Weapon? ChooseWeapon(List<Weapon> inventory)
        {
            while (true)
            {
                Console.WriteLine("\nEscolha sua arma:");
                for (int i = 0; i < inventory.Count; i++)
                    Console.WriteLine($"[{i + 1}] - {inventory[i].Name}");

                int punhos = inventory.Count + 1;
                int cancel = inventory.Count + 2;

                Console.WriteLine($"[{punhos}] - Punhos\n[{cancel}] - Cancelar");
                int choice = InputUtils.ChooseOption(cancel);

                if (choice == punhos)
                    return new Weapon { Name = "Punhos", Attack = 1, Damage = 1, Range = 0 };

                if (choice == cancel)
                    return null;

                if (choice >= 1 && choice <= inventory.Count)
                {
                    var w = inventory[choice - 1];
                    Console.WriteLine($"\n-{w.Name}-\nATK: {w.Attack}\nDMG: {w.Damage}\nRANGE: {w.Range}\n[1] - Usar\n[2] - Cancelar");
                    int confirm = InputUtils.ChooseOption(2);
                    if (confirm == 1)
                        return w;
                }
            }
        }

        private static int RollAttack(int stunTurns, int baseAttack)
        {
            int roll = RandomUtils.Roll(10);
            if (stunTurns > 0)
                roll = Math.Max(roll, RandomUtils.Roll(10));
            return roll;
        }
    }
}
