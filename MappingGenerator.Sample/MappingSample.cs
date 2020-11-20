using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MappingGenerator.Sample
{
    class MappingSample
    {
        private A _a;

        public A AProperty { get; set; }

        public void Sample()
        {
            //var c = new C();
            //var a = new A();
            //c.BValue = a.Map();
            //var b1 = new B();
            //A a2 = b.Map();
            var b2 = new B();
            //B b3 = b2.Map();
            //AProperty = b2.Map();
            //b1.DoStuff(b2.Map(), 9);
            //b1.DoThing(b2.Map());
            //(A, B) gg = (b1.Map(), b2);

            //var d = new Other.D
            //{
            //    AValue = b2.Map()
            //};

            var d2 = new C(b2.Map());
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

        public void DoThing(A a)
        {

        }

        public void DoStuff<T>(A a, T x)
        {

        }
    }

    public class C
    {
        public A AValue { get; set; }
        public B BValue { get; set; }

        public C(A a)
        {
            AValue = a;
        }

        public C(A a, B b)
        {
            AValue = a;
            BValue = b;
        }
    }
}

namespace MappingGenerator.Other
{
    public class D
    {
        public Sample.A AValue { get; set; }
        public Sample.B BValue { get; set; }

        public D()
        {

        }

        public D(Sample.A a)
        {
            AValue = a;
        }
    }
}

//namespace MappingGenerator.Different
//{
//    public class E
//    {
//        public E()
//        {
//            Other.D d = new Other.D();
//            Other.D d2 = new Other.D();
//            d2.BValue = d.AValue.Map();
//        }
//    }
//}