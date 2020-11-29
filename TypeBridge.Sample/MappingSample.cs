using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeBridge.Sample
{
    class MappingSample
    {
        private A _a;

        public A AProperty { get; set; }

        public void Sample()
        {
            //// Property assignment
            //var c1 = new C();
            //var a1 = new A();
            //c1.BValue = a1.Map();

            //// Variable assignment
            //var b2 = new B();
            //A a2 = b2.Map();

            //// Generic constructors
            //var b3 = new B();
            //var f3 = new F<A>(b3.Map());

            //// Property assignments
            //AProperty = b3.Map();

            //// Generic methods
            //var b4 = new B();
            //b4.DoStuff<A>(b2.Map());
            //b4.DoThing(b2.Map());
            
            //// Object initializers
            //var d5 = new Other.D
            //{
            //    AValue = b2.Map()
            //};

            //// Constructor arguments
            //var d6 = new C(b2.Map());

            //// Nested types, directly assignable
            //var c7 = new C(new A());
            //G g7 = c7.Map();
            //A a7 = new A();

            //// Mapping collections
            //var m1 = new M11();
            //M22 m2 = m1.Map();

            //// Map to base type
            //BaseA a8base = b3.Map();

            //// Map nested types - both direct match and one requiring a mapping
            //var g9 = new G();
            //C c9 = g9.Map();

            // Extension
            var h10 = new H();
            var k10 = new K();
            A a10 = h10.Map().Extend(Fun());
            //J j10 = h10.Map();
        }

        private K Fun() =>
            default;
    }

    public class H
    {
        public int Age { get; set; }
    }

    public class K
    {
        public string Name { get; set; }
    }

    public class J
    {
        public int Age { get; set; }
    }

    public class M1
    {
        public A[] Items { get; set; }
    }

    public class M2
    {
        public IEnumerable<B> Items { get; set; }
    }

    public class M11
    {
        public List<M1> Items { get; set; }
    }

    public class M22
    {
        public M2[] Items { get; set; }
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

        public C()
        {

        }

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

namespace TypeBridge.Other
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
