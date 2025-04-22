
namespace Repositories.Models
{
    public class SavePlayerDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public SavePlayerDto() { }

        public SavePlayerDto(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}