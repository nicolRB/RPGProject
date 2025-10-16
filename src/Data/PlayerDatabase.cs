using System;
using System.IO;
using System.Text.Json;
using RPGProject.Entities;

namespace RPGProject.Data
{
    public static class PlayerDatabase
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Player.json");
        private static PlayerData? _player;

        public static PlayerData Player
        {
            get
            {
                if (_player == null)
                    Load();
                return _player!;
            }
        }

        public static void Load()
        {
            if (!File.Exists(FilePath))
            {
                _player = new PlayerData();
                Save();
            }
            else
            {
                string json = File.ReadAllText(FilePath);
                _player = JsonSerializer.Deserialize<PlayerData>(json) ?? new PlayerData();
            }
        }

        public static void Save()
        {
            if (_player == null) return;

            string json = JsonSerializer.Serialize(_player, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        // === ITENS ===
        public static void AddItem(int itemId, int quantity = 1)
        {
            var entry = Player.Inventory.Items.Find(i => i.Id == itemId);
            if (entry != null)
                entry.Quantity += quantity;
            else
                Player.Inventory.Items.Add(new ItemEntry { Id = itemId, Quantity = quantity });

            Save();
        }

        public static bool RemoveItem(int itemId, int quantity = 1)
        {
            var entry = Player.Inventory.Items.Find(i => i.Id == itemId);
            if (entry == null || entry.Quantity < quantity)
                return false;

            entry.Quantity -= quantity;
            if (entry.Quantity <= 0)
                Player.Inventory.Items.Remove(entry);

            Save();
            return true;
        }

        public static void ClearItems()
        {
            // Limpa o inventário atual de armas
            Player.Inventory.Items.Clear();

            Save();
        }

        // === ARMAS ===
        public static void AddWeapon(int weaponId, int quantity = 1)
        {
            var entry = Player.Inventory.Weapons.Find(w => w.Id == weaponId);
            if (entry != null)
                entry.Quantity += quantity;
            else
                Player.Inventory.Weapons.Add(new WeaponEntry { Id = weaponId, Quantity = quantity });

            Save();
        }

        public static bool RemoveWeapon(int weaponId, int quantity = 1)
        {
            var entry = Player.Inventory.Weapons.Find(w => w.Id == weaponId);
            if (entry == null || entry.Quantity < quantity)
                return false;

            entry.Quantity -= quantity;
            if (entry.Quantity <= 0)
                Player.Inventory.Weapons.Remove(entry);

            Save();
            return true;
        }

        public static void ClearWeapons()
        {
            // Limpa o inventário atual de armas
            Player.Inventory.Weapons.Clear();

            Save();
        }

        public static void CreativeWeapons()
        {
            // Adiciona 1 de cada arma disponível
            foreach (var weapon in WeaponDatabase.Weapons)
            {
                Player.Inventory.Weapons.Add(new WeaponEntry
                {
                    Id = weapon.Id,
                    Quantity = 1
                });
            }

            Save();
        }

        // === OURO ===
        public static void AddGold(int amount)
        {
            Player.Gold += amount;
            Save();
        }

        public static bool SpendGold(int amount)
        {
            if (Player.Gold < amount)
                return false;

            Player.Gold -= amount;
            Save();
            return true;
        }

        public static void ClearGold()
        {
            // Limpa o inventário atual de armas
            Player.Gold = 0;
            Save();
        }
    }
}
