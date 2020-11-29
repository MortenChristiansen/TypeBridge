namespace TypeBridge.Test.TestMappingClasses
{
    class MonsterCamp<TMonster> where TMonster : Monster
    {
        public MonsterCamp(params TMonster[] monsters)
        {
            Monsters = monsters;
        }

        public TMonster[] Monsters { get; }
    }
}
