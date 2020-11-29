using System.Collections.Generic;

namespace TypeBridge.Test.TestMappingClasses
{
    class MonsterCampDto<TMonster>
    {
        public IEnumerable<TMonster> Monsters { get; set; }
    }
}
