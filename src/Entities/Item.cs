namespace RPGProject.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Effect { get; set; } = string.Empty;
        public int Power { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}