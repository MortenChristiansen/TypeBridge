using System.Collections.Generic;

namespace TypeBridge.Test.TestMappingClasses
{
    class GoblinBoss : Goblin
    {
        public GoblinBoss(IEnumerable<Goblin> subjects) : base(sneakyness: 55) {
            Subjects = subjects;
        }

        public IEnumerable<Goblin> Subjects { get; }
    }
}
