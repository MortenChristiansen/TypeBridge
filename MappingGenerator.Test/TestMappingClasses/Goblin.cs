namespace MappingGenerator.Test.TestMappingClasses
{
    class Goblin : Monster
    {
        public Goblin(int sneakyness) : base("goblin", 10) {
            Sneakyness = sneakyness;
        }

        public int Sneakyness { get; }
    }
}
