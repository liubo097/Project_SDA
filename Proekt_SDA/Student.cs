using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt_SDA
{
    internal class Student
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public List<Grade> Grades = new List<Grade>();
        public Student(string iD, string name)
        {
            ID = iD;
            Name = name;
        }
        public override string ToString()
        {
            if (Grades.Count != 0)
            {
                foreach (Grade g in Grades)
                {
                    return $"{Name} {ID} --- {g.ToString()}";
                }
            }
            return $"{Name} {ID}";
        }
    }
}
