using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionLib.model
{
    public abstract class Person
    {
        private String _name;
        private int _birthOfYear;

        protected Person():this("",0)
        {
        }

        protected Person(string name, int birthOfYear)
        {
            _name = name;
            _birthOfYear = birthOfYear;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int BirthOfYear
        {
            get => _birthOfYear;
            set => _birthOfYear = value;
        }

        public int Age
        {
            get => DateTime.Now.Year - _birthOfYear;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(BirthOfYear)}: {BirthOfYear}, {nameof(Age)}: {Age}";
        }
    }
}
