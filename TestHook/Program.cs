

using MyHoot;

namespace TestHook
{
    public class OriginClass
    {
        public static string GetMsg(string msg)
        {
            return msg + " Origin";
        }
    }

    public class HookClass
    {
        public static string GetMsg(string msg)
        {
            var res = msg + " Hook\n";
            // res += GetMsg_Origin(msg);
            return res;
        }

        public static string GetMsg_Origin(string msg)
        {
            return $"Again {msg}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var originMethod = typeof(OriginClass).GetMethod("GetMsg");
            var hookMethod = typeof(HookClass).GetMethod("GetMsg");
            var proxyMethod = typeof(HookClass).GetMethod("GetMsg_Origin");
            Hook myHook = new Hook(originMethod, hookMethod, proxyMethod);
            Console.WriteLine("==============");
            myHook.Enable();
            var msg = OriginClass.GetMsg("Hello World");
            Console.WriteLine($"{HookClass.GetMsg_Origin("Hello World")}");
            Console.WriteLine("==============");
            myHook.Disable();
            // msg = OriginClass.GetMsg("Hello World");
            // Console.WriteLine($"{msg}");
        }
    }
}