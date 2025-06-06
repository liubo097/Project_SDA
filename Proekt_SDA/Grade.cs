using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt_SDA
{
    internal class Grade
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
        public override string ToString()
        {
            return $"{Subject.Name}: {Value} ({Date.ToShortDateString()})";
        }
    }
}
