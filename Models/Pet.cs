namespace APIisBEESinItaly.Models
{
    public class Pet
    {
        public int Id { get; set; }                           // ID único para cada perro
        public string Name { get; set; } = string.Empty;     // Nombre del pet

        //public int Age { get; set; } = 0;                 // Pet's age
        // public Breed ---
        public DateTime? BirthDate { get; set; } // Nullable date
    }
}