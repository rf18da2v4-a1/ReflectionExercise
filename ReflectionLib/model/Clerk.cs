using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionLib.model
{
    public class Clerk:Person
    {
        private String[] _skills;
        private int skillCount = 0;

        public Clerk()
        {
            _skills = new string[5];
        }

        public Clerk(string name, int birthOfYear, string[] skills) : base(name, birthOfYear)
        {
            _skills = skills;
        }

        public string[] Skills
        {
            get => _skills;
            set => _skills = value;
        }

        public void AddSkill(String skill)
        {
            // ToDo check not exceed capacity
            _skills[skillCount++] = skill;
            
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Skills)}: {string.Join(", ",Skills)}";
        }
    }
}
