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
        public static void StartCombat(Monster monster, int playerHP, int playerDefense, int playerAttack, int playerDamage, bool escapable)
        {
            Console.Clear();

            int currentPlayerHP = playerHP;
            int currentMonsterHP = monster.HP;
            int proximity = 0; // 0 = frente a frente
            int roomSize = 3; // sala padrão
            int parryValue = 2;
            int stunTurns = 0;
            bool defendingThisTurn  = false;

            Console.WriteLine($"Início do combate: {monster.Name} aparece!");
            Console.WriteLine($"HP: {currentMonsterHP}\nATK: {monster.Attack}\nDMG: {monster.Damage}\nDEF: {monster.Defense}({monster.Defense+6} min)\nRANGE: {monster.Range}");
            Console.WriteLine($"\n\nSeus status:\nHP: {currentPlayerHP} / {playerHP}\nDEF: {playerDefense}({playerDefense+6} min)\nATK: +{playerAttack}\nDMG: +{playerDamage}");

            List<Weapon> inventory = WeaponDatabase.Weapons;
            bool[] hasWeapon = new bool[inventory.Count];
            for (int i = 0; i < hasWeapon.Length; i++) hasWeapon[i] = true;

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
                Console.WriteLine("-Seu Turno-\n\n");
                while (!turnDone)
                {
                    Console.WriteLine("Escolha sua ação:");
                    if (escapable)
                    {
                        Console.WriteLine("[1] - Atacar\n[2] - Defender\n[3] - Se Afastar\n[4] - Se Aproximar\n[5] - Escapar\n");
                    }
                    else
                    {
                        Console.WriteLine("[1] - Atacar\n[2] - Defender\n[3] - Se Afastar\n[4] - Se Aproximar\n");
                    }
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1": // Atacar
                            Weapon selectedWeapon = ChooseWeapon(inventory, hasWeapon);
                            if (selectedWeapon == null)
                            {
                                Console.WriteLine("\nVoltando ao menu de ações...\n");
                                continue;
                            }

                            int weaponATK = selectedWeapon.Attack;
                            int weaponDMG = selectedWeapon.Damage;
                            string weaponName = selectedWeapon.Name;
                            int weaponRange = selectedWeapon.Range;

                            if (proximity <= weaponRange)
                            {
                                int playerRoll = RollAttack(stunTurns, playerAttack + weaponATK);
                                int totalPlayerRoll = playerRoll + playerAttack + weaponATK;
                                int minToHit = 6 + monster.Defense;
                                int critDmg = (int)((weaponDMG + playerDamage) * 1.7);

                                if (playerRoll == 10)
                                {
                                    currentMonsterHP -= critDmg;
                                    currentMonsterHP = Math.Max(0, currentMonsterHP);
                                    Console.WriteLine($"\nVocê acerta um ponto vital com {weaponName}, causando {critDmg} de dano!\n");
                                    turnDone = true;
                                }
                                else if (totalPlayerRoll >= minToHit)
                                {
                                    int damage = weaponDMG + playerDamage;
                                    currentMonsterHP -= damage;
                                    currentMonsterHP = Math.Max(0, currentMonsterHP);
                                    Console.WriteLine($"\nVocê ataca com {weaponName} causando {damage} de dano!");
                                    turnDone = true;
                                }
                                else
                                {
                                    Console.WriteLine($"\nVocê erra o ataque com {weaponName}!");
                                    turnDone = true;
                                }
                                Console.WriteLine($"\nhit {(playerRoll == 10 ? $"*{totalPlayerRoll}*" : $"{totalPlayerRoll}")} | {minToHit} def inimiga\n");
                            }
                            else
                            {
                                Console.WriteLine($"\nVocê está longe demais para usar {weaponName}!\n");
                            }
                            break;

                        case "2": // Defender / Parry
                            defendingThisTurn  = true;
                            Console.WriteLine($"\nVocê se prepara para aparar ataques futuros (+{parryValue} DEF temporário).\n");
                            turnDone = true;
                            break;

                        case "3": // Se Afastar
                            if (proximity < roomSize)
                            {
                                proximity += 2;
                                if (proximity > roomSize) proximity = roomSize;
                                Console.WriteLine($"\nVocê se afasta, aumentando a distância para {proximity}.\n");
                                turnDone = true;
                            }
                            else
                                Console.WriteLine("\nVocê não pode se afastar mais.\n");
                            break;

                        case "4": // Se Aproximar
                            if (proximity > 0)
                            {
                                proximity -= 2;
                                if (proximity < 0) proximity = 0;
                                Console.WriteLine($"\nVocê se aproxima, reduzindo a distância para {proximity}.\n");
                                turnDone = true;
                            }
                            else
                                Console.WriteLine("\nVocê já está frente a frente com o inimigo.\n");
                            break;

                        case "5": // Escapar
                            if (escapable)
                            {
                                Console.WriteLine("\nVocê fugiu do combate.");
                                combatEnded = true;
                                turnDone = true;
                            }
                            else
                            {
                                Console.WriteLine("Ação inválida!");
                            }
                                break;

                        default:
                            Console.WriteLine("Ação inválida!");
                            break;
                    }
                }

                if (currentMonsterHP <= 0) break;

                // --- Turno do monstro ---
                Console.WriteLine("-Turno Inimigo-\n\n");
                if (stunTurns > 0)
                {
                    Console.WriteLine($"{monster.Name} está atordoado e não consegue agir!");
                    stunTurns--;
                }
                else if (!combatEnded)
                {
                    if (proximity <= monster.Range)
                    {
                        int monsterRoll = RandomUtils.Roll(10);
                        int totalMonsterRoll = monsterRoll + monster.Attack;
                        int minToHit = 6 + playerDefense + (defendingThisTurn ? parryValue : 0);
                        if (monsterRoll == 10)
                        {
                            currentPlayerHP -= (int)(monster.Damage * 1.7);
                            Console.WriteLine($"{monster.Name} acerta um ponto vulnerável! Você sofre {(int)(monster.Damage * 1.7)} de dano.");
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
                        Console.WriteLine($"\n{(defendingThisTurn  ? "def+ " : "def ")}{minToHit} | {(monsterRoll == 10 ? $"*{totalMonsterRoll}* hit" : $"{totalMonsterRoll} hit")}");
                    }
                    else
                    {
                        proximity -= monster.Speed;
                        if (proximity < 0) proximity = 0;
                        Console.WriteLine($"{monster.Name} se aproxima para {proximity} de distância.");
                    }
                }

                defendingThisTurn  = false; // Reset parry
                round++;
                Thread.Sleep(500);
            }

            // --- Resultado ---
            if (currentPlayerHP > 0 && !combatEnded)
                Console.WriteLine($"\nVocê venceu {monster.Name}!");
            else if (currentPlayerHP > 0)
            {
                Console.WriteLine("\nCombate terminou sem vencedor.");
            }
            else Console.WriteLine($"\nVocê foi derrotado por {monster.Name}...");
        }

        private static Weapon ChooseWeapon(List<Weapon> inventory, bool[] hasWeapon)
        {
            while (true)
            {
                Console.WriteLine("\nEscolha sua arma:");
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (hasWeapon[i])
                    {
                        Weapon w = inventory[i];
                        Console.WriteLine($"[{i + 1}] - {w.Name}");
                    }
                }

                int punhos = inventory.Count + 1;
                int cancel = inventory.Count + 2;

                Console.WriteLine($"[{punhos}] - Punhos\n[{cancel}] - Cancelar");
                int choice = InputUtils.ChooseOption(cancel);

                // --- Punhos ---
                if (choice == punhos)
                {
                    Console.WriteLine("\n-Punhos-\nataque: 1\ndano: 1\nalcance: 0\n\nConfirmar?\n[1] - Sim\n[2] - Não");
                    int confirm = InputUtils.ChooseOption(2);
                    if (confirm == 1)
                        return new Weapon { Name = "Punhos", Attack = 1, Damage = 1, Range = 0 };
                    else
                        continue;
                }

                // --- Cancelar ---
                if (choice == cancel)
                {
                    return null;
                }

                // --- Armas normais ---
                if (choice >= 1 && choice <= inventory.Count)
                {
                    Weapon w = inventory[choice - 1];
                    Console.WriteLine($"\n-{w.Name}-\nataque: {w.Attack}\ndano: {w.Damage}\nalcance: {w.Range}\n\nConfirmar?\n[1] - Sim\n[2] - Não");
                    int confirm = InputUtils.ChooseOption(2);
                    if (confirm == 1)
                        return w;
                    else
                        continue;
                }

                Console.WriteLine("Opção inválida!");
            }
        }


        private static int RollAttack(int stunTurns, int baseAttack)
        {
            int roll = RandomUtils.Roll(10);
            if (stunTurns > 0)
            {
                int roll2 = RandomUtils.Roll(10);
                roll = Math.Max(roll, roll2);
            }
            return roll;
        }
    }
}
