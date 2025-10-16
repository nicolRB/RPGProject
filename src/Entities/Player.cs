namespace RPGProject.Entities
{
    public class PlayerData
    {
        public int MaxHP { get; set; } = 20;
        public int CurrentHP { get; set; } = 20;
        public int Attack { get; set; }
        public int Damage { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; } = 2;
        public int Gold { get; set; }
        public InventoryData Inventory { get; set; } = new();
    }

    public class InventoryData
    {
        public List<ItemEntry> Items { get; set; } = new();
        public List<WeaponEntry> Weapons { get; set; } = new();
    }

    public class ItemEntry
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }

    public class WeaponEntry
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}
