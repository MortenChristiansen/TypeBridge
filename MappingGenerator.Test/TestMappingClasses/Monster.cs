using System;
using System.Collections.Generic;
using System.Text;

namespace MappingGenerator.Test.TestMappingClasses
{
    abstract class Monster
    {
        public string Name { get; }
        public int HitPoints { get; }

        public Monster(string name, int hitPoints)
        {
            Name = name;
            HitPoints = hitPoints;
        }
    }
}
