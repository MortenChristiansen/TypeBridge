using System;
using System.Collections.Generic;
using System.Text;

namespace TypeBridge.Test.TestMappingClasses
{
    class GenericMonster : Monster
    {
        public GenericMonster(string name, int hitPoints)
            : base(name, hitPoints)
        {

        }
    }
}
