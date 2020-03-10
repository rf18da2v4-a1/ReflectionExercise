using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReflectionLib.model;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            XX x = new XX();
            PropertyInfo info = x.GetType().GetProperty("cs");

            string str = DoSomething(info, x);
            Console.WriteLine(str);

            Console.ReadLine();
        }

        private static string DoSomething(PropertyInfo info, Object obj)
        {
            String s = $"\"{info.Name}\":";

            Console.WriteLine(info.PropertyType);
            Console.WriteLine(info.PropertyType.Name);
            Console.WriteLine(info.PropertyType.GetElementType());
            Console.WriteLine(info.PropertyType.GetInterfaces().Any(t => t.IsGenericType
                                                                         && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
            s = s + MakeValue(info,obj);


            return s;
        }

        private static string MakeValue(PropertyInfo info, object o)
        {

            Console.WriteLine("Element Type: " + info.PropertyType.GetElementType() + "::");

            var vars = info.GetValue(o) as IEnumerable;
            foreach (object elem in vars)
            {
                Console.WriteLine("Element Type: " + elem +"::");
                Console.WriteLine("Element Type: " + elem.GetType() + "::");
            }

            

            return "";
        }
    }

    class XX
    {
        public List<Clerk> cs { get; set; }

        public XX()
        {
            cs = new List<Clerk>();
            cs.Add(new Clerk("n1", 1980, new string[] { "kkk", "jjj" }));
            cs.Add(new Clerk("n2", 1991, new string[] { "hhh", "ppp" }));
        }
    }
}
