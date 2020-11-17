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
            var b = new B();
            A a2 = b.Map();
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
    }

    public class C
    {
        public A AValue { get; set; }
        public B BValue { get; set; }
    }
}
