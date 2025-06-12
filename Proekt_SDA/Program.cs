using System.Data;
using System.Text;
using System.IO;

namespace Proekt_SDA
{
    internal class Program
    {
        static List<User> users = new List<User>();
        static Dictionary<string, Student> students = new Dictionary<string, Student>();
        static HashSet<Subject> subjects = new HashSet<Subject>();
        static User loggedInUser = null;
        const string filePath = "users.txt";

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            LoadUsers(filePath);

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   ▓▓▓ Добре дошли в GradePoint! ▓▓▓");
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
        }
        static void LoadUsers(string file)
        {
            if (!File.Exists(file))
            {
                File.Create(file).Close();
                return;
            }

            string[] lines = File.ReadAllLines(file);

            string subjectLine = lines[0];
            if (!string.IsNullOrWhiteSpace(subjectLine))
            {
                var subjectInput = subjectLine.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var subject in subjectInput)
                {
                    var parts = subject.Split('|');

                    if (parts.Length == 2)
                    {
                        string subjName = parts[0].Trim();
                        string teacherName = parts[1].Trim();

                        if (!subjects.Contains(new Subject(subjName, teacherName)))
                        {
                            subjects.Add(new Subject(subjName, teacherName));
                        }
                    }
                }
            }

            List<User> loadedParents = new List<User>();

            for (int i = 1; i < lines.Length; i++)
            {
                {
                    string[] parts = lines[i].Split(';');
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
                            string[] gradesParts = gradesData.Split('-');
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
                        students[student.Username] = student;
                    }
                    else if (type.ToLower() == "родител" && parts.Length >= 5)
                    {
                        List<string> studUsernames = parts[4].Split(',').ToList();
                        User parent = new User(username, password, type, name, studUsernames);
                        loadedParents.Add(parent);
                    }
                    else if (type.ToLower() == "учител" && parts.Length >= 5)
                    {
                        string subjName = parts[4];

                        users.Add(new User(username, password, type, name, subjName));
                        subjects.Add(new Subject(subjName, name));
                    }
                }
            }

            foreach (User parent in loadedParents)
            {
                foreach (string studName in parent.StudentUsernames)
                {
                    if (students.TryGetValue(studName, out Student student)) parent.Students.Add(student);
                }
                users.Add(parent);
            }
        }
        static void SaveUsers(string file)
        {
            List<string> lines = new List<string>();

            string subjectLine = "";

            foreach (Subject subj in subjects)
            {
                if (subjectLine != "") subjectLine += ";";
                subjectLine += $"{subj.Name}|{subj.Teacher}";
            }
            lines.Add(subjectLine);

            foreach (User user in users)
            {
                if (user.Type.ToLower() == "ученик")
                {
                    if (students.TryGetValue(user.Username, out Student student))
                    {
                        if (student.Grades == null) student.Grades = new List<Grade>();

                        List<string> gradeStrings = new List<string>();
                        foreach (Grade grade in student.Grades)
                        {
                            string gradeStr = $"{grade.Value}|{grade.Date:dd.MM.yyyy}|{grade.Subject.Name}";
                            gradeStrings.Add(gradeStr);
                        }

                        string line = $"{student.Username};{student.Password};{student.Type};{student.Name};{student.ID};{string.Join("-", gradeStrings)}";
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
                else if (user.Type.ToLower() == "учител")
                {
                    lines.Add($"{user.Username};{user.Password};{user.Type};{user.Name};{user.SubjectName}");
                }
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
                students[username] = student;
                users.Add(student);
            }
            else if (type == "родител")
            {
                List<string> linkedStudents = new List<string>();
                User parent = new User(username, password, "родител", name, linkedStudents);

                Console.Write("Въведете потребителското име на детето (може да са няколко, разделени със запетая): ");
                string input = Console.ReadLine();
                string[] inputUsernames = input.Split(',');

                foreach (string uname in inputUsernames.Select(p => p.Trim()))
                {
                    if (students.TryGetValue(uname, out Student student))
                    {
                        linkedStudents.Add(uname);
                        parent.Students.Add(student);
                    }
                    else Console.WriteLine($"⚠ Ученик с потребителско име \"{uname}\" не е намерен.");
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
            else
            {
                Console.Write("Въведете предмет, по който преподавате: ");
                string subjName = Console.ReadLine().Trim();

                subjects.Add(new Subject(subjName, name));
                users.Add(new User(username, password, type, name));
            }

            SaveUsers(filePath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nПрофилът е създаден успешно!");
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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("        ▓▓▓ GradePoint ▓▓▓");
            Console.ResetColor();

            if (loggedInUser != null && loggedInUser.Type.ToLower() == "ученик")
            {
                if (students.TryGetValue(loggedInUser.Username, out Student student))
                {
                    Console.WriteLine("\n======== УЧЕНИЧЕСКИ ПАНЕЛ ========");
                    Console.WriteLine(student);
                }
                else Console.WriteLine("Ученикът не е намерен.");
            }
            Console.WriteLine("\nНатисни Enter за връщане назад.");
            Console.ReadLine();
        }
        static void ShowTeacherPanel()
        {
            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("                ▓▓▓ GradePoint ▓▓▓");
                Console.ResetColor();

                Console.WriteLine("\n================ УЧИТЕЛСКИ ПАНЕЛ ==================");
                Console.WriteLine("----------------- СПИСЪК С ОПЦИИ ------------------");
                Console.WriteLine("1. Добавяне на оценка");
                Console.WriteLine("2. Редакция на оценки");
                Console.WriteLine("3. Търсене на оценки по ученик / предмет");
                Console.WriteLine("4. Сортиране на оценки по стойност / предмет / дата");
                Console.WriteLine("5. Извеждане на стипендианти");
                Console.WriteLine("6. Изготвяне на справка за ученик");
                Console.WriteLine("7. Назад");
                Console.Write("\nВъведете вашия избор: ");

                string choice = Console.ReadLine();
                string answer;

                switch (choice)
                {
                    case "1": AddGradeFlow(); break;
                    case "2":
                        Console.WriteLine("\n1. Редактиране на оценка");
                        Console.WriteLine("2. Изтриване на оценка");
                        Console.WriteLine("3. Назад");
                        Console.Write("\nВъведете вашия избор: ");

                        answer = Console.ReadLine();

                        switch (answer)
                        {
                            case "1": EditGradeFlow(); break;
                            case "2": DeleteGradeFlow(); break;
                            case "3": return; break;
                        }
                        break;
                    case "3":
                        Console.WriteLine("\n1. Търсене на оценки по ученик");
                        Console.WriteLine("2. Търсене на оценки по предмет");
                        Console.WriteLine("3. Назад");
                        Console.Write("\nВъведете вашия избор: ");

                        answer = Console.ReadLine();

                        switch (answer)
                        {
                            case "1": SearchGradesByStudentFlow(); break;
                            case "2": SearchGradesBySubjectFlow(); break;
                            case "3": return; break;
                        }
                        break;
                    case "4":
                        Console.WriteLine("\n1. Сортиране на оценки по стойност");
                        Console.WriteLine("2. Сортиране на оценки по предмет");
                        Console.WriteLine("3. Сортиране на оценки по дата");
                        Console.WriteLine("4. Назад");
                        Console.Write("\nВъведете вашия избор: ");

                        answer = Console.ReadLine();

                        switch (answer)
                        {
                            case "1": SortGradesByValueFlow(); break;
                            case "2": SortGradesBySubjectFlow(); break;
                            case "3": SortGradesByDateFlow(); break;
                            case "4": return; break;
                        }
                        break;
                    case "5": ShowScholarshipStudentsFlow(); break;
                    case "6": ShowStudentReportFlow(); break;
                    case "7": return; break;
                }
            }
        }
        static void AddGradeFlow()
        {
            Console.Write("\nВъведете потребителско име на ученик: ");
            string username = Console.ReadLine();

            if (!students.TryGetValue(username, out Student student))
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }

            Console.Write("\nВъведете оценка: ");
            if (double.TryParse(Console.ReadLine(), out double value))
            {
                if (student.Grades == null) student.Grades = new List<Grade>();

                student.AddGrade(new Grade(value, DateTime.Now, new Subject(loggedInUser.SubjectName, loggedInUser.Name)));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nОценката е добавена успешно.");
                Console.ResetColor();
            }
            else Console.WriteLine("Невалидна стойност за оценка.");
            Console.ReadLine();

            SaveUsers(filePath);
        }
        static void EditGradeFlow()
        {
            Console.Write("\nВъведете потребителско име на ученик: ");
            string studentUsername = Console.ReadLine();

            if (!students.TryGetValue(studentUsername, out Student student))
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
            Console.WriteLine("\nОценки:");
            for (int i = 0; i < student.Grades.Count; i++) Console.WriteLine($"{i + 1}. {student.Grades[i]}");

            Console.Write("\nИзберете номер на оценка за редактиране: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > student.Grades.Count)
            {
                Console.WriteLine("Невалиден избор.");
                Console.ReadLine();
                return;
            }
            index -= 1;

            Console.Write("\nНова стойност на оценката: ");
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
            else newDate = student.Grades[index].Date;

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
            else newSubject = student.Grades[index].Subject;

            bool edited = student.EditGrade(index, newValue, newDate.Value, newSubject);
            Console.WriteLine(edited ? "\nОценката е редактирана." : "\nГрешка при редактиране.");
            SaveUsers(filePath);
            Console.ReadLine();
        }
        static void DeleteGradeFlow()
        {
            Console.Write("\nВъведете потребителско име на ученик: ");
            string studentUsername = Console.ReadLine();

            if (!students.TryGetValue(studentUsername, out Student student))
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
            Console.WriteLine("\nОценки:");
            for (int i = 0; i < student.Grades.Count; i++)
                Console.WriteLine($"{i + 1}. {student.Grades[i]}");

            Console.Write("\nИзберете номер на оценка за изтриване: ");
            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > student.Grades.Count)
            {
                Console.WriteLine("Невалиден избор.");
                Console.ReadLine();
                return;
            }
            index -= 1;

            bool removed = student.RemoveGrade(index);
            Console.WriteLine(removed ? "\nОценката е изтрита." : "\nГрешка при изтриване.");
            SaveUsers(filePath);
            Console.ReadLine();
        }
        static void SearchGradesByStudentFlow()
        {
            Console.Write("\nВъведете потребителско име на ученика: ");
            string studentUsername = Console.ReadLine();

            if (!students.TryGetValue(studentUsername, out Student student))
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }
            else
            {
                Console.WriteLine($"\nОценки на {student.Name} ({student.ID}):");

                if (student.Grades.Count == 0) Console.WriteLine("Няма оценки.");
                else
                {
                    foreach (Grade grade in student.Grades)
                    {
                        Console.WriteLine($" - {grade.Subject.Name}: {grade.Value} на {grade.Date:dd.MM.yyyy}");
                    }
                }
            }

            Console.WriteLine("\nНатиснете Enter за връщане назад...");
            Console.ReadLine();
        }
        static void SearchGradesBySubjectFlow()
        {
            Console.Write("\nВъведете име на предмет: ");
            string subjectName = Console.ReadLine();

            bool found = false;

            Console.WriteLine($"\nОценки по предмет: {subjectName}");

            foreach (Student student in students.Values)
            {
                foreach (Grade grade in student.Grades)
                {
                    if (grade.Subject.Name.ToLower() == subjectName.ToLower())
                    {
                        Console.WriteLine($" - {student.Name} ({student.ID}): {grade.Value} на {grade.Date:dd.MM.yyyy}");
                        found = true;
                    }
                }
            }

            if (!found) Console.WriteLine("Няма оценки по този предмет.");

            Console.WriteLine("\nНатиснете Enter за връщане назад...");
            Console.ReadLine();
        }
        static void SortGradesByValueFlow()
        {
            Console.Write("\nВъведете потребителско име на ученик: ");
            string studentUsername = Console.ReadLine();

            if (!students.TryGetValue(studentUsername, out Student student))
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }
            else if (student.Grades.Count == 0) Console.WriteLine("Няма оценки за показване.");
            else
            {
                GradeBST bst = new GradeBST();

                foreach (Grade grade in student.Grades) bst.Insert(grade);

                Console.WriteLine($"\nОценки на {student.Name} ({student.ID}):");
                bst.InOrderTraversal();
            }

            Console.WriteLine("\nНатиснете Enter за връщане назад.");
            Console.ReadLine();
        }
        static void SortGradesBySubjectFlow()
        {
            Console.Write("\nВъведете потребителско име на ученик: ");
            string username = Console.ReadLine();

            if (!students.TryGetValue(username, out Student student))
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }
            else if (student.Grades.Count == 0) Console.WriteLine("Няма оценки за показване.");
            else
            {
                GradeBST bst = new GradeBST();

                foreach (Grade grade in student.Grades) bst.InsertBySubject(grade);

                Console.WriteLine($"\nОценки на {student.Name} по предмет:");
                bst.InOrderTraversal();
            }

            Console.WriteLine("\nНатиснете Enter за връщане назад...");
            Console.ReadLine();
        }
        static void SortGradesByDateFlow()
        {
            Console.Write("\nВъведете потребителско име на ученик: ");
            string username = Console.ReadLine();

            if (!students.TryGetValue(username, out Student student))
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }
            else if (student.Grades.Count == 0) Console.WriteLine("Няма оценки за показване.");
            else
            {
                GradeBST bst = new GradeBST();

                foreach (Grade grade in student.Grades) bst.InsertByDate(grade);

                Console.WriteLine($"\nОценки на {student.Name} по дата:");
                bst.InOrderTraversal();
            }

            Console.WriteLine("\nНатиснете Enter за връщане назад...");
            Console.ReadLine();
        }
        static void ShowScholarshipStudentsFlow()
        {
            Console.Clear();

            bool hasScholarships = false;

            foreach (Student student in students.Values)
            {
                if (student.Grades.Count == 0) continue;

                double average = student.Grades.Average(g => g.Value);

                if (average >= 5.50 && average < 5.75)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nСписък със стипендианти (успех >= 5.50):\n");
                    Console.ResetColor();

                    hasScholarships = true;
                    Console.WriteLine($"- {student.Name} (№ {student.ID}) - Среден успех: {average:F2}");
                }
                else if (average >= 5.75)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nСписък със стипендианти (успех >= 5.75):\n");
                    Console.ResetColor();

                    hasScholarships = true;
                    Console.WriteLine($"- {student.Name} (№ {student.ID}) - Среден успех: {average:F2}");
                }
            }

            if (!hasScholarships) Console.WriteLine("Няма ученици със стипендия.");

            Console.WriteLine("\nНатиснете Enter за връщане назад...");
            Console.ReadLine();
        }
        static void ShowStudentReportFlow()
        {
            Console.Write("\nВъведете потребителско име на ученик: ");
            string username = Console.ReadLine();

            if (!students.TryGetValue(username, out Student student))
            {
                Console.WriteLine("Ученикът не е намерен.");
                Console.ReadLine();
                return;
            }
            else
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("    ▓▓▓ GradePoint ▓▓▓");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nСправка за {student.Name} (№ {student.ID})");
                Console.ResetColor();

                if (student.Grades.Count == 0) Console.WriteLine("Няма въведени оценки.");
                else
                {
                    Console.WriteLine("\nОценки:");
                    foreach (var grade in student.Grades) Console.WriteLine($"- {grade.Subject.Name}: {grade.Value} ({grade.Date.ToShortDateString()})");

                    double average = student.Grades.Average(g => g.Value);
                    Console.WriteLine($"\nСреден успех: {average:F2}");

                    if (average >= 5.50)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Статус: Стипендиант (успех >= 5.50)");
                        Console.ResetColor();
                    }
                    else if (average >= 5.75)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Статус: Стипендиант (успех >= 5.75)");
                        Console.ResetColor();
                    }
                }
            }

            Console.WriteLine("\nНатиснете Enter за връщане назад...");
            Console.ReadLine();
        }
        static void ShowParentPanel()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("        ▓▓▓ GradePoint ▓▓▓");
            Console.ResetColor();

            Console.WriteLine("\n======== РОДИТЕЛСКИ ПАНЕЛ ========");

            User parent = null;

            foreach (User user in users)
            {
                if (user.Username == loggedInUser.Username && user.Type == "родител") parent = user;
            }

            if (parent.Students.Count == 0) Console.WriteLine("Нямате свързани ученици.");
            else
            {
                foreach (Student student in parent.Students)
                {
                    Console.WriteLine(student);
                }
            }

            Console.WriteLine("\nНатиснете Enter за връщане назад...");
            Console.ReadLine();
        }
    }
}
