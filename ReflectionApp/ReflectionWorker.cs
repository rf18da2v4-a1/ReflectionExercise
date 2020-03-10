using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Channels;
using MyJsonLib.json;
using Newtonsoft.Json;
using ReflectionLib.model;

namespace ReflectionApp
{
    internal class ReflectionWorker
    {
        public void Start()
        {
            /*
             * Start up Reflection
             */

            // create two objects
            Clerk c = new Clerk("Mia", 1988, new string[2] { "EASY", "Reception" });
            Manager m = new Manager("Per", 1964);
            m.Employees.Add(c);
            m.Employees.Add(new Manager("Michael", 1955));

            // printout Clerk
            Console.WriteLine();
            Console.WriteLine("Start with clerk");
            Console.WriteLine();
            Console.WriteLine(c);
            TryReflection(c);

            // printout Manager
            Console.WriteLine();
            Console.WriteLine("Then comes manager");
            Console.WriteLine();
            Console.WriteLine(m);
            TryReflection(m);


            /*
             * Serializing
             */
            Console.WriteLine();
            Console.WriteLine(" ===> Serializing <===");
            Console.WriteLine();

            // class with simple property types
            TestVarClass tvc = new TestVarClass(2000000000, -32000, 5000000000, 255, 't', true, "peter");
            string jsonSimpleTypes = JsonConvert.SerializeObject(tvc);
            Console.WriteLine(jsonSimpleTypes);

            string jsonSimpleTypes2 = MyJsonConverter.Serialize(tvc);
            Console.WriteLine(jsonSimpleTypes2);

            // class with object property type
            AnotherTestClass atc = new AnotherTestClass() { InnerClass = new TestVarClass(30, 30, 30, 30, 'r', false, "anewObject") };
            string jsonObjectTypes = JsonConvert.SerializeObject(atc);
            Console.WriteLine(jsonObjectTypes);

            string jsonObjectTypes2 = MyJsonConverter.Serialize(atc);
            Console.WriteLine(jsonObjectTypes2);

            // Class with List of strings - Clerk
            String jsonListStrings = JsonConvert.SerializeObject(c);
            Console.WriteLine(jsonListStrings);
            Console.WriteLine(MyJsonConverter.Serialize(c));

            // Class with List of objects - Manager
            String jsonListObjects = JsonConvert.SerializeObject(m);
            Console.WriteLine(jsonListObjects);
            Console.WriteLine(MyJsonConverter.Serialize(m));

            // Class with list of lists
            TestListInListClass tc = new TestListInListClass();
            tc.ListClerks.Add(new List<Clerk>() { new Clerk("df", 1999, new string[] { "hej", "davs" }) });
            tc.ListClerks.Add(new List<Clerk>() { new Clerk("dif", 2011, new string[] { "hej", "davs" }) });
            tc.ListClerks.Add(new List<Clerk>() { new Clerk("fd", 2001, new string[] { "hej", "davs" }) });

            Console.WriteLine(JsonConvert.SerializeObject(tc));
            Console.WriteLine(MyJsonConverter.Serialize(tc));



            /*
             * Deserializing
             */
            Console.WriteLine();
            Console.WriteLine(" ===> Deserializing <===");
            Console.WriteLine();

            // Simple types as properties
            TestVarClass tc2 = MyJsonConverter.Deserialize<TestVarClass>(jsonSimpleTypes);
            Console.WriteLine(tc2);

            // Object type as property
            AnotherTestClass atc2 = MyJsonConverter.Deserialize<AnotherTestClass>(jsonObjectTypes);
            Console.WriteLine(atc2);


            // clerk with list of strings
            //Clerk newClerk = MyJsonConverter.Deserialize<Clerk>(jsonListStrings);
            //Console.WriteLine("Deserialized clerk: " + newClerk);

            // Manager with list of objects
            //Manager newManager = MyJsonConverter.Deserialize<Manager>(jsonListObjects);
            //Console.WriteLine("Deserialized Manager: " + newManager);

        }

        public void TryReflection(Object obj)
        {
            Type oType = obj.GetType();
            TryReflection(obj, oType);
        }

        public void TryReflection(Object obj, Type oType)
        {
            // simple meta date
            Console.WriteLine("   === Simple Meta Data ===");
            Console.WriteLine("Name : " + oType.Name);
            Console.WriteLine("FullName : " + oType.FullName);
            Console.WriteLine("Is class : " + oType.IsClass);
            Console.WriteLine("Is abstract : " + oType.IsAbstract);
            Console.WriteLine("is interface : " + oType.IsInterface);
            Console.WriteLine("is generic : " + oType.IsGenericType);

            // properties
            Console.WriteLine("   === Properties ===");
            foreach (PropertyInfo info in oType.GetProperties())
            {
                Console.WriteLine($"Property Name={info.Name} Value={info.GetValue(obj)}");
            }

            // methods
            Console.WriteLine("   === Methods ===");
            foreach (MethodInfo info in oType.GetMethods())
            {
                String mParams = String.Join<ParameterInfo>(", ", info.GetParameters());
                Console.WriteLine($"Method Name={info.Name} Param={mParams} returnValueType={info.ReturnParameter}");
            }

            Type baseType = oType.BaseType;
            if (baseType != null && baseType.Name != "Object")
            {
                Console.WriteLine("   >>> Base Class <<<");
                TryReflection(obj, baseType);
            }

        }
    }


    internal class TestListInListClass
    {
        public List<List<Clerk>> ListClerks { get; set; }

        public TestListInListClass()
        {
            ListClerks = new List<List<Clerk>>();
        }
    }

    internal class AnotherTestClass
    {
        public TestVarClass InnerClass{ get; set; }

        public override string ToString()
        {
            return $"{nameof(InnerClass)}: {InnerClass}";
        }
    }
}