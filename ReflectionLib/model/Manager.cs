using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionLib.model
{
    public class Manager:Person
    {
        private List<Person> _employees;

        public Manager()
        {
            _employees = new List<Person>();
        }

        public Manager(string name, int birthOfYear) : base(name, birthOfYear)
        {
            _employees = new List<Person>();
        }

        public List<Person> Employees
        {
            get => _employees;
            set => _employees = value;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Employees)}: {String.Join(", ",_employees)}";
        }
    }
}
