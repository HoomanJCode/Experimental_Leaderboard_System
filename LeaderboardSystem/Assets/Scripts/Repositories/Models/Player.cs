
namespace Repositories.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Player() { }

        public Player(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}