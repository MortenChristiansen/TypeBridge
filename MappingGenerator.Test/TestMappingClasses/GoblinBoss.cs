using System.Collections.Generic;

namespace MappingGenerator.Test.TestMappingClasses
{
    class GoblinBoss : Goblin
    {
        public GoblinBoss(IEnumerable<Goblin> subjects) : base(sneakyness: 55) {
            Subjects = subjects;
        }

        public IEnumerable<Goblin> Subjects { get; }
    }
}
