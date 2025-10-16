namespace RPGProject.Entities
{
    public class Monster
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Damage { get; set; }
        public int Defense { get; set; }
        public int Range { get; set; }
        public int Speed { get; set; }
    }
}
