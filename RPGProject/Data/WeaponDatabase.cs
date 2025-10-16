using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RPGProject.Entities;
using RPGProject.Utils;

namespace RPGProject.Data
{
    public static class WeaponDatabase
    {
        private static List<Weapon>? _weapons;

        public static List<Weapon> Weapons
        {
            get
            {
                if (_weapons == null)
                    LoadWeapons();
                return _weapons!;
            }
        }

        public static void LoadWeapons(string relativePath = "Data/Weapons.json")
        {
            _weapons = JsonDatabaseLoader.LoadJsonList<Weapon>(relativePath, "WeaponDatabase");
        }

        public static Weapon? GetWeaponById(int id)
        {
            return Weapons.Find(w => w.Id == id);
        }
    }
}
