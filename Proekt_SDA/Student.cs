using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt_SDA
{
    internal class Student : User
    {
        public string ID { get; set; }
        public List<Grade> Grades { get; set; } = new List<Grade>();
        public Student(string username, string password, string type, string name, string id) : base(username, password, type, name)
        {
            Username = username;
            Password = password;
            Type = "ученик";
            Name = name;
            ID = id;
        }
        public void AddGrade(Grade grade)
        {
            Grades.Add(grade);
        }
        public double GetAverage()
        {
            if (Grades.Count == 0) return 0;

            double sum = 0;
            foreach (Grade g in Grades) sum += g.Value;

            return sum / Grades.Count;
        }
        public bool RemoveGrade(int index)
        {
            if (index < 0 || index >= Grades.Count) return false;

            Grades.RemoveAt(index);

            return true;
        }
        public bool EditGrade(int index, double newValue, DateTime newDate, Subject newSubject)
        {
            if (index < 0 || index >= Grades.Count) return false;

            Grades[index].Value = newValue;

            if (newDate != DateTime.MinValue) Grades[index].Date = newDate;

            if (newSubject != null) Grades[index].Subject = newSubject;

            return true;
        }
        public List<Grade> SearchGrades(string subjectName)
        {
            List<Grade> result = new List<Grade>();

            foreach (Grade grade in Grades) if (string.IsNullOrWhiteSpace(subjectName)) result.Add(grade);

            return result;
        }
        public void SortGrades(string criteria)
        {
            for (int i = 0; i < Grades.Count - 1; i++)
            {
                for (int j = 0; j < Grades.Count; j++)
                {
                    bool shouldSwap = false;

                    switch (criteria.ToLower())
                    {
                        case "date": if (Grades[i].Date > Grades[j].Date) shouldSwap = true; break;
                        case "value": if (Grades[i].Value > Grades[j].Value) shouldSwap = true; break;
                        case "subject": if (Grades[i].Subject.Name.CompareTo(Grades[j].Subject.Name) > 0) shouldSwap = true; break;
                        default: break;
                    }

                    if (shouldSwap)
                    {
                        Grade temp = Grades[i];
                        Grades[i] = Grades[j];
                        Grades[j] = temp;
                    }
                }
            }
        }
        public override string ToString()
        {
            string result = ID + " - " + Name + ", среден успех: " + GetAverage().ToString() + "\nОценки:\n";

            if (Grades.Count == 0) result += "Няма въведени оценки.";
            else foreach (Grade g in Grades) result += " " + g.ToString() + "\n";

            return result;
        }
        public string GetReport()
        {
            var report = $"Справка за ученик: {Name} ({ID}) \nСреден успех: {GetAverage()}\nОценки:\n";

            if (Grades.Count == 0) report += "Няма оценки.\n";
            else
            {
                List<Grade> sortedGrades = new List<Grade>();

                for (int i = 0; i < Grades.Count; i++) sortedGrades.Add(Grades[i]);

                for (int i = 0; i < sortedGrades.Count;i++)
                {
                    for (int j = i + 1; j < sortedGrades.Count; j++)
                    {
                        if (sortedGrades[i].Date > sortedGrades[j].Date)
                        {
                            Grade temp = sortedGrades[i];
                            sortedGrades[i] = sortedGrades[j];
                            sortedGrades[j] = temp;
                        }
                    }
                }

                for (int i = 0;i < sortedGrades.Count; i++)
                {
                    Grade grade = sortedGrades[i];
                    report += " - " + grade.Subject.Name + ": " + grade.Value + " на " + grade.Date.ToShortDateString() + "\n";
                }
            }

            return report;
        }
    }
}
