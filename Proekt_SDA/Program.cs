using System.Data;
using System.Text;
using System.IO;

namespace Proekt_SDA
{
    internal class Program
    {
        static List<User> users = new List<User>();
        static List<Student> students = new List<Student>();
        static List<Subject> subjects = new List<Subject>();
        static User loggedInUser = null;
        const string filePath = "users.txt";

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            LoadUsers("users.txt");

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   ▓▓▓ Добре дошли в GradePoint! ▓▓▓    ");
                Console.ResetColor();
                Console.WriteLine("\n----------- СПИСЪК С ОПЦИИ -----------");
                Console.WriteLine("1. Вход");
                Console.WriteLine("2. Регистрация на потребител");
                Console.WriteLine("3. Изход");
                Console.Write("\nВъведете вашия избор: ");
                string choice = Console.ReadLine();

                if (choice == "1") Login();
                else if (choice == "2") Register();
                else break;
            }

            //Console.WriteLine("\n---------------------- МЕНЮ -----------------------");
            //Console.WriteLine("1. Добавяне на ученик / предмет / оценка");
            //Console.WriteLine("2. Редакция на оценки");
            //Console.WriteLine("3. Търсене на оценки по ученик / предмет");
            //Console.WriteLine("4. Сортиране на оценки по дата / предмет / стойност");
            //Console.Write("\nВъведете вашия избор: ");

            //string ans1 = Console.ReadLine();
            //string answer;

            //switch (ans1)
            //{
            //    case "1":
            //        Console.WriteLine("\n1. Добавяне на ученик");
            //        Console.WriteLine("2. Добавяне на предмет");
            //        Console.WriteLine("3. Добавяне на оценка");

            //        answer = Console.ReadLine();
            //        break;
            //}
        }
        static void LoadUsers(string file)
        {
            if (!File.Exists(file))
            {
                File.Create(file).Close();
                return;
            }

            string[] lines = File.ReadAllLines(file);
            List<User> loadedParents = new List<User>();

            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length < 4) continue;

                string username = parts[0];
                string password = parts[1];
                string type = parts[2];
                string name = parts[3];

                if (type.ToLower() == "ученик" && parts.Length >= 6)
                {
                    string id = parts[4];
                    Student student = new Student(username, password, type, name, id);

                    string gradesData = parts[5];
                    if (!string.IsNullOrWhiteSpace(gradesData))
                    {
                        string[] gradesParts = gradesData.Split(',');
                        foreach (string gradePart in gradesParts)
                        {
                            string[] gradeFields = gradePart.Split('|');
                            if (gradeFields.Length == 3)
                            {
                                if (double.TryParse(gradeFields[0], out double value) && DateTime.TryParse(gradeFields[1], out DateTime date))
                                {
                                    string subjectName = gradeFields[2];

                                    Subject subject = null;
                                    foreach (Subject subj in subjects)
                                    {
                                        if (subj.Name == subjectName)
                                        {
                                            subject = subj;
                                            break;
                                        }
                                    }

                                    if (subject == null)
                                    {
                                        subject = new Subject(subjectName, "неизвестен");
                                        subjects.Add(subject);
                                    }
                                    student.AddGrade(new Grade(value, date, subject));
                                }
                            }
                        }
                    }

                    users.Add(student);
                    students.Add(student);
                }
                else if (type.ToLower() == "родител" && parts.Length >= 5)
                {
                    List<string> studUsernames = parts[4].Split(',').ToList();
                    User parent = new User(username, password, type, name, studUsernames);
                    loadedParents.Add(parent);
                }
                else users.Add(new User(username, password, type, name));
            }

            foreach (User parent in loadedParents)
            {
                foreach (string name in parent.StudentUsernames)
                {
                    foreach (Student stud in students)
                    {
                        if (stud.Username == name)
                        {
                            parent.Students.Add(stud);
                            break;
                        }
                    }
                }
                users.Add(parent);
            }
        }
        static void SaveUsers(string file)
        {
            List<string> lines = new List<string>();

            foreach (User user in users)
            {
                if (user.Type.ToLower() == "ученик")
                {
                    Student student = null;
                    foreach (Student stud in students)
                    {
                        if (user.Username == stud.Username)
                        {
                            student = stud;
                            break;
                        }
                    }

                    if (student != null)
                    {
                        List<string> gradeStrings = new List<string>();
                        foreach (Grade grade in student.Grades)
                        {
                            string gradeStr = $"{grade.Value}|{grade.Date:yyyy-MM-dd}|{grade.Subject.Name}";
                            gradeStrings.Add(gradeStr);
                        }

                        string line = $"{student.Username};{student.Password};{student.Type};{student.Name};{student.ID};{string.Join(",", gradeStrings)}";
                        lines.Add(line);
                    }
                    else lines.Add($"{user.Username};{user.Password};{user.Type};{user.Name}");
                }
                else if (user.Type.ToLower() == "родител")
                {
                    List<string> studUsernames = new List<string>();
                    foreach (Student stud in user.Students)
                    {
                        studUsernames.Add(stud.Username);
                    }
                    lines.Add($"{user.Username};{user.Password};{user.Type};{user.Name};{string.Join(",", studUsernames)}");
                }
                else lines.Add($"{user.Username};{user.Password};{user.Type};{user.Name}");
            }

            File.WriteAllLines(file, lines);
        }
        static void Register()
        {
            Console.Write("\nТип потребител (ученик / учител / родител): ");
            string type = Console.ReadLine().ToLower();

            Console.Write("Въведете потребителско име: ");
            string username = Console.ReadLine();

            bool usernameExists = false;
            foreach (User user in users)
            {
                if (user.Username != null && user.Username.ToLower() == username.ToLower())
                {
                    usernameExists = true;
                    break;
                }
            }

            if (usernameExists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Потребител с това потребителско име вече съществува.");
                Console.ResetColor();

                Console.WriteLine("Натиснете Enter за връщане назад.");
                Console.ReadLine();
                return;
            }

            Console.Write("Въведете парола: ");
            string password = Console.ReadLine();
            Console.Write("Въведете име: ");
            string name = Console.ReadLine();

            if (type == "ученик")
            {
                Console.Write("ID: ");
                string id = Console.ReadLine();
                var student = new Student(username, password, type, name, id);
                students.Add(student);
                users.Add(student);
            }
            else if (type == "родител")
            {
                List<string> linkedStudents = new List<string>();
                User parent = new User(username, password, "родител", name, linkedStudents);

                Console.Write("Въведете потребителското име на детето (може да са няколко, разделени със запетая): ");
                string input = Console.ReadLine();
                string[] inputUsernames = input.Split(',');

                foreach (string studentUsername in inputUsernames)
                {
                    string newUsername = studentUsername.Trim().ToLower();
                    bool found = false;

                    for (int i = 0; i < students.Count; i++)
                    {
                        if (students[i].Username != null && students[i].Username.ToLower() == newUsername)
                        {
                            found = true;
                            linkedStudents.Add(students[i].Username);
                            parent.Students.Add(students[i]);
                            break;
                        }
                    }

                    if (!found)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"⚠ Ученик с потребителско име \"{newUsername}\" не е намерен и няма да бъде добавен.");
                        Console.ResetColor();
                    }
                }

                if (linkedStudents.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Не е добавен нито един валиден ученик.");
                    Console.ResetColor();
                    Console.WriteLine("Натиснете Enter за връщане назад.");
                    Console.ReadLine();
                    return;
                }

                users.Add(parent);
            }
            else users.Add(new User(username, password, type, name));

            SaveUsers(filePath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Профилът е създаден успешно!");
            Console.ResetColor();
            Console.ReadLine();
        }
        static void Login()
        {
            Console.Write("\nПотребителско име: ");
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
                }
            }

            if (loggedInUser == null)
            {
                Console.WriteLine("Невалидни данни.");
                Console.ReadLine();
            }
            else
            {
                switch (type)
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

            Student student = null;
            foreach (Student stud in students)
            {
                student = stud;
                break;
            }

            if (student != null)
            {
                Console.WriteLine("=== Ученически панел ===");
                Console.WriteLine(student);
            }
            else Console.WriteLine("Ученикът не е намерен.");

            Console.WriteLine("\nНатисни Enter за връщане назад.");
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
            SaveUsers(filePath);
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

            bool edited = student.EditGrade(index, newValue, newDate.Value, newSubject);
            Console.WriteLine(edited ? "Оценката е редактирана." : "Грешка при редактиране.");
            SaveUsers(filePath);
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
            SaveUsers(filePath);
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

            User parent = null;

            foreach (User user in users)
            {
                if (user.Username == loggedInUser.Username && user.Type == "родител")
                {
                    parent = user;
                    break;
                }    
            }

            if (parent.Students.Count == 0)
            {
                Console.WriteLine("Нямате свързани ученици.");
            }
            else
            {
                foreach (Student student in parent.Students)
                {
                    Console.WriteLine(student);
                }
            }
            Console.WriteLine("Натисни Enter...");
            Console.ReadLine();
        }
    }
}
