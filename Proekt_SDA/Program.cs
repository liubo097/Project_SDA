using System.Data;
using System.Text;

namespace Proekt_SDA
{
    //internal class Program
    //{
    //    static List<Student> students = new List<Student>();
    //    static List<Subject> subjects = new List<Subject>();
    //    static List<User> users = new List<User>();
    //    static Student? loggedInStudent = null;
    //    static User? loggedInUser = null;
    //    static void Main(string[] args)
    //    {
    //        Console.OutputEncoding = Encoding.UTF8;

    //        Console.ForegroundColor = ConsoleColor.Green;
    //        Console.WriteLine("         ▓▓▓ Добре дошли в GradePoint! ▓▓▓         ");
    //        Console.ResetColor();

    //        //Console.WriteLine("\nИмате ли акаунт?");
    //        //string ans = Console.ReadLine();

    //        //if (ans.ToLower() == "да")
    //        //{
    //        //    Console.Write("\nВъведете потребителско име: ");
    //        //    string username = Console.ReadLine();
    //        //    Console.Write("Въведете парола: ");
    //        //    string pass = Console.ReadLine();
    //        //}

    //        //Console.WriteLine("\n---------------------- МЕНЮ -----------------------");


    //        //Console.WriteLine("\n---------------------- МЕНЮ -----------------------");
    //        //Console.WriteLine("1. Добавяне на ученик / предмет / оценка");
    //        //Console.WriteLine("2. Редакция на оценки");
    //        //Console.WriteLine("3. Търсене на оценки по ученик / предмет");
    //        //Console.WriteLine("4. Сортиране на оценки по дата / предмет / стойност");
    //        //Console.Write("\nВъведете вашия избор: ");

    //        //string ans = Console.ReadLine();
    //        //string answer;

    //        //switch (ans)
    //        //{
    //        //    case "1":
    //        //        Console.WriteLine("\n1. Добавяне на ученик");
    //        //        Console.WriteLine("2. Добавяне на предмет");
    //        //        Console.WriteLine("3. Добавяне на оценка");

    //        //        answer = Console.ReadLine();

    //        //        switch(answer)
    //        //        {
    //        //            case "1": AddStudent(); break;
    //        //            case "2": AddSubject(); break;
    //        //        }
    //        //        break;
    //        //}
    //    }
    //    static void AddStudent()
    //    {
    //        Console.Write("Въведете ID: ");
    //        string id = Console.ReadLine();

    //        foreach (Student student in students)
    //        {
    //            if (student.ID == id)
    //            {
    //                Console.WriteLine("Ученик с такова ID вече съществува.");
    //                return;
    //            }
    //        }

    //        Console.Write("Въведете име: ");
    //        string name = Console.ReadLine();
    //        students.Add(new Student(name, id));
    //    }
    //    static void AddSubject()
    //    {

    //    }
    //}
    class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public List<string> StudentUsernames { get; set; } = new List<string>();
        public User(string username, string password, string type, string name, List<string> students = null)
        {
            Username = username;
            Password = password;
            Type = type;
            Name = name;

            if (type.ToLower() == "parent" && students != null) StudentUsernames = students;
        }
    }

    class Subject
    {
        public string Name { get; set; }
        public string Teacher { get; set; }
        public Subject(string name, string teacher)
        {
            Name = name;
            Teacher = teacher;
        }
        public override string ToString() => $"{Name} - {Teacher}";
    }

    class Grade
    {
        public double Value { get; set; }
        public DateTime Date { get; set; }
        public Subject Subject { get; set; }

        public Grade(double value, DateTime date, Subject subject)
        {
            Value = value;
            Date = date;
            Subject = subject;
        }
        public override string ToString() => $"{Subject.Name}: {Value} ({Date.ToShortDateString()})";
    }

    class Student : User
    {
        public string ID { get; set; }
        public List<Grade> Grades { get; set; } = new List<Grade>();

        public Student(string username, string password, string type, string name, string id)
            : base(username, password, type, name)
        {
            ID = id;
        }

        public void AddGrade(Grade grade) => Grades.Add(grade);

        public double GetAverage() => Grades.Count == 0 ? 0 : Grades.Average(g => g.Value);

        public bool RemoveGrade(int index)
        {
            if (index < 0 || index >= Grades.Count)
                return false;
            Grades.RemoveAt(index);
            return true;
        }

        public bool EditGrade(int index, double newValue, DateTime? newDate = null, Subject newSubject = null)
        {
            if (index < 0 || index >= Grades.Count)
                return false;
            Grades[index].Value = newValue;
            if (newDate.HasValue)
                Grades[index].Date = newDate.Value;
            if (newSubject != null)
                Grades[index].Subject = newSubject;
            return true;
        }

        public List<Grade> SearchGrades(string subjectName = null)
        {
            if (string.IsNullOrEmpty(subjectName))
                return new List<Grade>(Grades);
            return Grades.Where(g => g.Subject.Name.Equals(subjectName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void SortGrades(string criteria)
        {
            switch (criteria.ToLower())
            {
                case "date":
                    Grades = Grades.OrderBy(g => g.Date).ToList();
                    break;
                case "value":
                    Grades = Grades.OrderBy(g => g.Value).ToList();
                    break;
                case "subject":
                    Grades = Grades.OrderBy(g => g.Subject.Name).ToList();
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            string result = ID + " - " + Name + ", среден успех: " + GetAverage().ToString("F2") + "\nОценки:\n";
            if (Grades.Count == 0) result += "Няма въведени оценки.";
            else Grades.ForEach(g => result += " " + g + "\n");
            return result;
        }

        public string GetReport()
        {
            var report = $"Справка за ученик: {Name} ({ID})\nСреден успех: {GetAverage():F2}\nОценки:\n";
            if (Grades.Count == 0) report += "Няма оценки.\n";
            else
            {
                var sortedGrades = Grades.OrderBy(g => g.Date).ToList();
                foreach (var grade in sortedGrades)
                {
                    report += $" - {grade.Subject.Name}: {grade.Value} на {grade.Date.ToShortDateString()}\n";
                }
            }
            return report;
        }
    }

    class GradeBST
    {
        private class GradeNode
        {
            public Grade Data { get; set; }
            public GradeNode Left { get; set; }
            public GradeNode Right { get; set; }

            public GradeNode(Grade data) => Data = data;
        }

        private GradeNode Root;

        public void Insert(Grade grade) => Root = InsertRecursive(Root, grade);

        private GradeNode InsertRecursive(GradeNode node, Grade grade)
        {
            if (node == null) return new GradeNode(grade);
            if (grade.Value < node.Data.Value) node.Left = InsertRecursive(node.Left, grade);
            else node.Right = InsertRecursive(node.Right, grade);
            return node;
        }

        public void InOrderTraversal() => InOrderRecursive(Root);

        private void InOrderRecursive(GradeNode node)
        {
            if (node == null) return;
            InOrderRecursive(node.Left);
            Console.WriteLine(node.Data);
            InOrderRecursive(node.Right);
        }
    }

    class Program
    {
        static List<User> users = new List<User>();
        static List<Student> students = new List<Student>();
        static List<Subject> subjects = new List<Subject>();
        static User loggedInUser = null;

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            string file = "users.txt";
            List<User> list = new List<User>();

            if (!File.Exists(file)) users = list;

            string[] lines = File.ReadAllLines(file);
            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length < 4) continue;

                string username = parts[0];
                string password = parts[1];
                string type = parts[2];
                string name = parts[3];

                if (type.ToLower() == "ученик" && parts.Length >= 5)
                {
                    Student student = new Student(username, password, type, name, parts[4]);
                    users.Add(student);
                    students.Add(student);
                }
                else if (type.ToLower() == "родител" && parts.Length >= 5)
                {
                    var studUsernames = parts[4].Split(',').ToList();
                    users.Add(new User(username, password, type, name, studUsernames));
                }
                else users.Add(new User(username, password, type, name));
            }
            StartMenu();
        }
        static void StartMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Вход");
                Console.WriteLine("2. Регистрация");
                Console.WriteLine("3. Изход");
                Console.Write("Избор: ");
                string choice = Console.ReadLine();

                if (choice == "1") Login();
                else if (choice == "2") Register();
                else if (choice == "3") break;
            }
        }
        static void Register()
        {
            Console.Write("Тип (Student/Teacher/Parent): ");
            string type = Console.ReadLine();

            Console.Write("Потребителско име: ");
            string username = Console.ReadLine();
            Console.Write("Парола: ");
            string password = Console.ReadLine();
            Console.Write("Име: ");
            string name = Console.ReadLine();

            if (type.ToLower() == "ученик")
            {
                Console.Write("ID: ");
                string id = Console.ReadLine();
                var student = new Student(username, password, type, name, id);
                students.Add(student);
                users.Add(student);

                Console.Write("Свържи с родител (д/н): ");
                if (Console.ReadLine().ToLower() == "д")
                {
                    Console.Write("Потребителско име на родителя: ");
                    string parentUsername = Console.ReadLine();

                    User parent = null;
                    foreach (User user in users)
                    {
                        if (user.Type.ToLower() == "родител" && user.Username == parentUsername)
                        {
                            parent = user;
                            break;
                        }
                    }

                    if (parent != null) parent.StudentUsernames.Add(username);
                    else Console.WriteLine("Родител не е намерен.");
                }
            }
            else
            {
                users.Add(new User(username, password, type, name));
            }
            SaveUsers(users);
        }

        static void Login()
        {
            Console.Write("Потребителско име: ");
            string username = Console.ReadLine();
            Console.Write("Парола: ");
            string password = Console.ReadLine();
            string type = null;

            loggedInUser = null;
            foreach (User user in users)
            {
                if (user.Username == username && user.Password == password)
                {
                    type = user.Type;
                    loggedInUser = user;
                    break;
                }
            }

            if (loggedInUser == null)
            {
                Console.WriteLine("Невалидни данни.");
                Console.ReadLine();
            }
            else
            {
                switch (type.ToLower())
                {
                    case "ученик": ShowStudentPanel(); break;
                    case "учител": ShowTeacherPanel(); break;
                    case "родител": ShowParentPanel(); break;
                }
            }
        }

        static void ShowStudentPanel()
        {
            Console.Clear();
            var student = students.FirstOrDefault(s => s.Username == loggedInUser.Username);
            if (student != null)
            {
                Console.WriteLine("=== Ученически панел ===");
                Console.WriteLine(student);
            }
            Console.WriteLine("Натисни Enter...");
            Console.ReadLine();
        }

        static void ShowTeacherPanel()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Учителски панел ===");
                Console.WriteLine("1. Добави предмет");
                Console.WriteLine("2. Добави оценка");
                Console.WriteLine("3. Редактирай оценка");
                Console.WriteLine("4. Изтрий оценка");
                Console.WriteLine("5. Търси оценки");
                Console.WriteLine("6. Сортирай оценки");
                Console.WriteLine("7. Изведи стипендианти");
                Console.WriteLine("8. Справка за ученик");
                Console.WriteLine("9. Назад");
                Console.Write("Избор: ");
                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("Име на предмет: ");
                    string subjectName = Console.ReadLine();
                    if (!subjects.Any(s => s.Name.Equals(subjectName, StringComparison.OrdinalIgnoreCase)))
                        subjects.Add(new Subject(subjectName, loggedInUser.Name));
                    else Console.WriteLine("Предметът вече съществува.");
                    Console.ReadLine();
                }
                else if (choice == "2")
                {
                    AddGradeFlow();
                }
                else if (choice == "3")
                {
                    EditGradeFlow();
                }
                else if (choice == "4")
                {
                    DeleteGradeFlow();
                }
                else if (choice == "5")
                {
                    SearchGradesFlow();
                }
                else if (choice == "6")
                {
                    SortGradesFlow();
                }
                else if (choice == "7")
                {
                    ShowScholarshipStudentsFlow();
                }
                else if (choice == "8")
                {
                    ShowStudentReportFlow();
                }
                else if (choice == "9") break;
            }
        }

        static void AddGradeFlow()
        {
            Console.Write("Потребителско име на ученик: ");
            string studentUsername = Console.ReadLine();

            Student student = null;
            foreach (Student stud in students)
            {
                if (stud.Username == student.Username)
                {
                    student = stud;
                    break;
                }
            }

            if (student == null)
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }

            Console.Write("Предмет: ");
            string subjectName = Console.ReadLine();
            var subject = subjects.FirstOrDefault(s => s.Name.Equals(subjectName, StringComparison.OrdinalIgnoreCase));
            if (subject == null)
            {
                subject = new Subject(subjectName, loggedInUser.Name);
                subjects.Add(subject);
            }
            Console.Write("Оценка: ");
            if (!double.TryParse(Console.ReadLine(), out double value))
            {
                Console.WriteLine("Невалидна стойност.");
                Console.ReadLine();
                return;
            }
            var grade = new Grade(value, DateTime.Now, subject);
            student.AddGrade(grade);
            Console.WriteLine("Оценката е добавена.");
            Console.ReadLine();
        }

        static void EditGradeFlow()
        {
            Console.Write("Потребителско име на ученик: ");
            string studentUsername = Console.ReadLine();

            Student student = null;
            foreach (Student stud in students)
            {
                if (stud.Username == student.Username)
                {
                    student = stud;
                    break;
                }
            }

            if (student == null)
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }
            if (student.Grades.Count == 0)
            {
                Console.WriteLine("Няма оценки за редактиране.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Оценки:");
            for (int i = 0; i < student.Grades.Count; i++)
                Console.WriteLine($"{i + 1}. {student.Grades[i]}");

            Console.Write("Избери номер на оценка за редактиране: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > student.Grades.Count)
            {
                Console.WriteLine("Невалиден избор.");
                Console.ReadLine();
                return;
            }
            index -= 1;

            Console.Write("Нова стойност на оценката: ");
            if (!double.TryParse(Console.ReadLine(), out double newValue))
            {
                Console.WriteLine("Невалидна стойност.");
                Console.ReadLine();
                return;
            }

            Console.Write("Нова дата (формат ГГГГ-ММ-ДД) или празно за запазване на старата: ");
            string dateInput = Console.ReadLine();
            DateTime? newDate = null;
            if (!string.IsNullOrEmpty(dateInput))
            {
                if (DateTime.TryParse(dateInput, out DateTime parsedDate))
                    newDate = parsedDate;
                else
                {
                    Console.WriteLine("Невалиден формат на дата.");
                    Console.ReadLine();
                    return;
                }
            }

            Console.Write("Нов предмет или празно за запазване на стария: ");
            string newSubjectName = Console.ReadLine();
            Subject newSubject = null;
            if (!string.IsNullOrEmpty(newSubjectName))
            {
                newSubject = subjects.FirstOrDefault(s => s.Name.Equals(newSubjectName, StringComparison.OrdinalIgnoreCase));
                if (newSubject == null)
                {
                    newSubject = new Subject(newSubjectName, loggedInUser.Name);
                    subjects.Add(newSubject);
                }
            }

            bool edited = student.EditGrade(index, newValue, newDate, newSubject);
            Console.WriteLine(edited ? "Оценката е редактирана." : "Грешка при редактиране.");
            Console.ReadLine();
        }

        static void DeleteGradeFlow()
        {
            Console.Write("Потребителско име на ученик: ");
            string studentUsername = Console.ReadLine();

            Student student = null;
            foreach (Student stud in students)
            {
                if (stud.Username == student.Username)
                {
                    student = stud;
                    break;
                }
            }

            if (student == null)
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }
            if (student.Grades.Count == 0)
            {
                Console.WriteLine("Няма оценки за изтриване.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Оценки:");
            for (int i = 0; i < student.Grades.Count; i++)
                Console.WriteLine($"{i + 1}. {student.Grades[i]}");

            Console.Write("Избери номер на оценка за изтриване: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > student.Grades.Count)
            {
                Console.WriteLine("Невалиден избор.");
                Console.ReadLine();
                return;
            }
            index -= 1;

            bool removed = student.RemoveGrade(index);
            Console.WriteLine(removed ? "Оценката е изтрита." : "Грешка при изтриване.");
            Console.ReadLine();
        }

        static void SearchGradesFlow()
        {
            Console.Write("Потребителско име на ученик: ");
            string studentUsername = Console.ReadLine();

            Student student = null;
            foreach (Student stud in students)
            {
                if (stud.Username == student.Username)
                {
                    student = stud;
                    break;
                }
            }

            if (student == null)
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }

            Console.Write("Търси по предмет (остави празно за всички): ");
            string subjectName = Console.ReadLine();

            var results = student.SearchGrades(subjectName);
            if (results.Count == 0)
            {
                Console.WriteLine("Няма съвпадения.");
            }
            else
            {
                Console.WriteLine("Намерени оценки:");
                foreach (var grade in results)
                    Console.WriteLine(grade);
            }
            Console.ReadLine();
        }

        static void SortGradesFlow()
        {
            Console.Write("Потребителско име на ученик: ");
            string studentUsername = Console.ReadLine();

            Student student = null;
            foreach (Student stud in students)
            {
                if (stud.Username == student.Username)
                {
                    student = stud;
                    break;
                }
            }

            if (student == null)
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }
            if (student.Grades.Count == 0)
            {
                Console.WriteLine("Няма оценки за сортиране.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Сортирай по (date/value/subject): ");
            string criteria = Console.ReadLine();
            student.SortGrades(criteria);

            Console.WriteLine("Оценките след сортиране:");
            foreach (var grade in student.Grades)
                Console.WriteLine(grade);
            Console.ReadLine();
        }

        static void ShowScholarshipStudentsFlow()
        {
            Console.Write("Минимален среден успех за стипендия: ");
            if (!double.TryParse(Console.ReadLine(), out double threshold))
            {
                Console.WriteLine("Невалидна стойност.");
                Console.ReadLine();
                return;
            }
            var scholars = students.Where(s => s.GetAverage() >= threshold).ToList();
            if (scholars.Count == 0)
                Console.WriteLine("Няма стипендианти.");
            else
            {
                Console.WriteLine("Стипендианти:");
                foreach (var s in scholars)
                    Console.WriteLine($"{s.Name} ({s.ID}) - среден успех: {s.GetAverage():F2}");
            }
            Console.ReadLine();
        }

        static void ShowStudentReportFlow()
        {
            Console.Write("Потребителско име на ученик: ");
            string studentUsername = Console.ReadLine();

            Student student = null;
            foreach (Student stud in students)
            {
                if (stud.Username == student.Username)
                {
                    student = stud;
                    break;
                }
            }

            if (student == null)
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine(student.GetReport());
            Console.ReadLine();
        }

        static void ShowParentPanel()
        {
            Console.Clear();
            Console.WriteLine("=== Родителски панел ===");
            var parent = loggedInUser;
            if (parent.StudentUsernames.Count == 0)
            {
                Console.WriteLine("Нямате свързани ученици.");
            }
            else
            {
                foreach (var studUsername in parent.StudentUsernames)
                {
                    Student student = null;
                    foreach (Student stud in students)
                    {
                        if (stud.Username == studUsername)
                        {
                            student = stud;
                            break;
                        }
                    }

                    if (student != null) Console.WriteLine(student);
                    else Console.WriteLine($"- Неизвестен ученик: {studUsername}");
                }
            }
            Console.WriteLine("Натисни Enter...");
            Console.ReadLine();
        }
        static void SaveUsers(List<User> users)
        {
            List<string> lines = new List<string>();
            foreach (var user in users)
            {
                if (user.Type.ToLower() == "ученик")
                {
                    Student student = null;
                    foreach (Student stud in students)
                    {
                        if (stud.Username == user.Username)
                        {
                            student = stud;
                            break;
                        }
                    }

                    if (student != null) lines.Add($"{student.Username};{student.Password};{student.Type};{student.Name};{student.ID}");
                }
                else if (user.Type.ToLower() == "родител")
                {
                    lines.Add($"{user.Username};{user.Password};{user.Type};{user.Name};{string.Join(",", user.StudentUsernames)}");
                }
                else lines.Add($"{user.Username};{user.Password};{user.Type};{user.Name}");
            }
            File.WriteAllLines("users.txt", lines);
        }
    }
}
