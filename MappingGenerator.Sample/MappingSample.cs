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
            //var b2 = new B();
            //var f = new F<A>(b2.Map());
            //B b3 = b2.Map();
            //AProperty = b2.Map();
            //b1.DoStuff<A>(b2.Map());
            //b1.DoThing(b2.Map());
            //(A, B) gg = (b1.Map(), b2);

            //var d = new Other.D
            //{
            //    AValue = b2.Map()
            //};

            //var d2 = new C(b2.Map());

            //var c = new C(new A());
            //G g = c.Map();
            //A a1 = new A();
            var b = new B();
            //A a2 = a1.Map();


            BaseA a3 = b.Map();
            A a = b.Map();
        }
    }

    public abstract class BaseA
    {
        public int Age { get; set; }
    }

    public class A : BaseA, IA
    {
        public string Name { get; set; }
    }

    public class BaseB
    {
        public string Name { get; set; }
    }

    public class B : BaseB
    {
        public int Age { get; set; }

        public void DoThing(A a)
        {

        }

        public void DoStuff<T>(A a, T x)
        {

        }

        public void DoStuff<T>(T x)
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

    public interface IA
    {
        int Age { get; set; }
        string Name { get; set; }
    }

    public class F<T>
    {
        public F(T t)
        {

        }

        public F(A a, T t)
        {

        }
    }

    public class G
    {
        public IA AValue { get; set; }
        public B BValue { get; set; }
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