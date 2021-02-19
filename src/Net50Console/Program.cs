using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Reffy.Expressions;

namespace Net50Console
{
    public class Foo
    {
        public int MyProperty { get; set; }
        public double MyProperty2 { get; set; }

        public Foo()
        {

        }

        public Foo(int i, double d)
        {
            MyProperty = i;
            MyProperty2 = d;
        }
    }

    public class Test
    {
        private const int length = 1_000;

        [Benchmark]
        public void CallConstructor()
        {
            for (int i = 0; i < length; i++)
            {
                var foo = new Foo(i, (double)i);
            }
        }

        [Benchmark]
        public void UseActivator()
        {
            for (int i = 0; i < length; i++)
            {
                var foo = Activator.CreateInstance(typeof(Foo), new object[] { i, (double)i });
            }
        }

        [Benchmark]
        public void UseConstructor()
        {
            for (int i = 0; i < length; i++)
            {
                var foo = typeof(Foo).Constructor(i, (double)i);
            }
        }

        [Benchmark]
        public void UseRestrictedConstructor()
        {
            for (int i = 0; i < length; i++)
            {
                var foo = typeof(Foo).RestrictedConstructor(i, (double)i);
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Test>();
        }
    }
}
