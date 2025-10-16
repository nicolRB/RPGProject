using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RPGProject.Entities;

namespace RPGProject.Data
{
    public static class ItemDatabase
    {
        private static List<Item>? _items;

        public static List<Item> Items
        {
            get
            {
                if (_items == null)
                    LoadItems();
                return _items!;
            }
        }

        public static void LoadItems(string relativePath = "Data/Items.json")
        {
            _items = JsonDatabaseLoader.LoadJsonList<Item>(relativePath, "ItemDatabase");
        }

        public static Item? GetItemById(int id)
        {
            return Items.Find(i => i.Id == id);
        }
    }
}
