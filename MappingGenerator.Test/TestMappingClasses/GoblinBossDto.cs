using System.Collections.Generic;

namespace MappingGenerator.Test.TestMappingClasses
{
    class GoblinBossDto
    {
        public string Name { get; set; }
        public int HitPoints { get; set; }
        public int Sneakyness { get; set; }
        public IEnumerable<GoblinDto> Subjects { get; set; }
    }
}
