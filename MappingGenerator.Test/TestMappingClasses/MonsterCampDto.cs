using System.Collections.Generic;

namespace MappingGenerator.Test.TestMappingClasses
{
    class MonsterCampDto<TMonster>
    {
        public IEnumerable<TMonster> Monsters { get; set; }
    }
}
