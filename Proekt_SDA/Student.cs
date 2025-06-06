using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt_SDA
{
    //internal class Student : User
    //{
    //    public string ID { get; set; }
    //    public List<Grade> Grades { get; set; } = new List<Grade>();
    //    public Student(string username, string password, UserType type, string name, string id) : base(username, password, type, name)
    //    {
    //        Username = username;
    //        Password = password;
    //        Type = UserType.Student;
    //        Name = name;
    //        ID = id;
    //    }
    //    public void AddGrade(Grade grade)
    //    {
    //        Grades.Add(grade);
    //    }
    //    public double GetAverage()
    //    {
    //        if (Grades.Count == 0) return 0;

    //        double sum = 0;
    //        foreach (Grade g in Grades)
    //        {
    //            sum += g.Value;
    //        }

    //        return sum / Grades.Count;
    //    }
    //    public override string ToString()
    //    {
    //        string result = ID + " - " + Name + ", среден успех: " + GetAverage().ToString() + "\nОценки:\n";

    //        if (Grades.Count == 0) result += "Няма въведени оценки.";
    //        else
    //        {
    //            foreach (Grade g in Grades)
    //            {
    //                result += " " + g.ToString() + "\n";
    //            }
    //        }

    //        return result;
    //    }
    //}
}
