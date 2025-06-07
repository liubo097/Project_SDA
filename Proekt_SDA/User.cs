using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt_SDA
{
    internal class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public List<string> StudentUsernames { get; set; } = new List<string>();
        public List<Student> Students { get; set; } = new List<Student>();
        public User(string username, string password, string type, string name, List<string> students = null)
        {
            Username = username;
            Password = password;
            Type = type;
            Name = name;

            if (type.ToLower() == "parent" && students != null) StudentUsernames = students;
        }
    }
}
