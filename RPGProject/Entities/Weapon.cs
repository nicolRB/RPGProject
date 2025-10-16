namespace RPGProject.Entities
{
    public class Weapon
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Attack { get; set; }
        public int Damage { get; set; }
        public int Range { get; set; }
        public int Speed { get; set; }
        public string Type { get; set; } = "";
    }
}
