using System;
using System.Reflection;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks;
using CallingConventions = Reloaded.Hooks.Definitions.X64.CallingConventions;

namespace ConsoleApp1
{
    public class SolidClass
    {
        public void RunMsg(string msg)
        {
           
            Console.WriteLine($"Hook {msg}");
        }

        public void RunMsg_Origin(string msg)
        {
            return;
        }

        public static void TestStatic()
        {
            Console.WriteLine("Test Static Solid Hook");
        }
    }

    public class MyTestClass
    {
        public void RunMsg(string msg)
        {
            Console.WriteLine($"Fix Hook {msg}");
        }

        public static void TestStatic()
        {
            Console.WriteLine("Test Static MyTestClass Hook");
        }
        
    }
 
    class Program
    {
        [Function(CallingConventions.Microsoft)]
        public delegate void TestStaticFunc();

        private static IHook<TestStaticFunc> _TestStaticFunc;

        public void SomeClass()
        {
            
        }
        
        public static void TestStaticImp()
        {
            Console.WriteLine("Test Static MyTestClass Hook");
            _TestStaticFunc.OriginalFunction();
        }

        static void Main(string[] args)
        {
            
        }
    }
}