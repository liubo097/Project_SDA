using System.Text;

namespace Proekt_SDA
{
    internal class Program
    {
        static List<Student> students = new List<Student>();
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("GradePoint");

            Console.WriteLine("\n---------------------- МЕНЮ -----------------------");
            Console.WriteLine("1. Добавяне на ученик / предмет / оценка");
            Console.WriteLine("2. Редакция на оценки");
            Console.WriteLine("3. Търсене на оценки по ученик / предмет");
            Console.WriteLine("4. Сортиране на оценки по дата / предмет / стойност");
            Console.Write("\nВъведете вашия избор: ");

            string ans = Console.ReadLine();
            string answer;

            switch (ans)
            {
                case "1":
                    Console.WriteLine("\n1. Добавяне на ученик");
                    Console.WriteLine("2. Добавяне на предмет");
                    Console.WriteLine("3. Добавяне на оценка");

                    answer = Console.ReadLine();

                    switch(answer)
                    {
                        case "1": AddStudent(); break;
                        case "2": AddSubject(); break;
                    }
                    break;
            }
        }
        static void AddStudent()
        {
            Console.Write("Въведете ID: ");
            string id = Console.ReadLine();

            foreach (Student student in students)
            {
                if (student.ID == id)
                {
                    Console.WriteLine("Ученик с такова ID вече съществува.");
                    return;
                }
            }

            Console.Write("Въведете име: ");
            string name = Console.ReadLine();
            students.Add(new Student(name, id));
        }
        static void AddSubject()
        {

        }
    }
}
