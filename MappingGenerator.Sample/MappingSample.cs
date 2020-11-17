using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingGenerator.Sample
{
    class MappingSample
    {
        public void Sample()
        {
            //var c = new C();
            //var a = new A();
            //c.BValue = a.Map();
            var b1 = new B();
            //A a2 = b.Map();
            var b2 = new B();
            b1.DoStuff(b2.Map());
        }
    }

    public class A
    {
        public int Age { get; set; }
        public string Name { get; set; }
    }

    public class B
    {
        public int Age { get; set; }
        public string Name { get; set; }

        public void DoStuff(A a)
        {

        }
    }

    public class C
    {
        public A AValue { get; set; }
        public B BValue { get; set; }
    }

    /* TODO 
     - Do not generate map method if a valid method with the same name already exists
     - If the two types are the same, just return the original (or do not emit a mapping method)
     - Recursively map complex types (including collections)
     - Support arguments to generic methods.
     - Support property initializer.
     - Support mapping from tuples.
     - Test if there is a valid constructor we can use in the destination class.
     - Handle mapping properties that can be implicitly converted or otherwise assigned to the receiving property.
     - The mapper class should not have all the fields but just have the source object instead. This saves copying all the properties.
     */
}
