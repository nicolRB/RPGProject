using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RPGProject.Entities;

namespace RPGProject.Data
{
    public static class MonsterDatabase
    {
        private static List<Monster>? _monsters;

        public static List<Monster> Monsters
        {
            get
            {
                if (_monsters == null)
                {
                    LoadMonsters();
                }
                return _monsters!;
            }
        }
        public static void LoadMonsters(string relativePath = "Data/Monsters.json")
        {
            _monsters = JsonDatabaseLoader.LoadJsonList<Monster>(relativePath, "MonsterDatabase");
        }

        public static Monster? GetMonsterById(int id)
        {
            return Monsters.Find(m => m.Id == id);
        }
    }
}
