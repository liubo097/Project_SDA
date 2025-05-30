using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt_SDA
{
    internal class Subject
    {
        public string Name { get; set; }
        public string Teacher { get; set; }
        public Subject(string name, string teacher)
        {
            Name = name;
            Teacher = teacher;
        }
        public override string ToString()
        {
            return $"{Name} - преподавател: {Teacher}";
        }
    }
}
