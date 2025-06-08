using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proekt_SDA
{
    internal class GradeBST
    {
        private class GradeNode
        {
            public Grade Data { get; set; }
            public GradeNode Left { get; set; }
            public GradeNode Right { get; set; }
            public GradeNode(Grade data)
            {
                Data = data;
            }
        }

        private GradeNode Root;
        public void Insert(Grade grade)
        {
            Root = InsertRecursive(Root, grade);
        }
        private GradeNode InsertRecursive(GradeNode node, Grade grade)
        {
            if (node == null) return new GradeNode(grade);
            if (grade.Value < node.Data.Value) node.Left = InsertRecursive(node.Left, grade);
            else node.Right = InsertRecursive(node.Right, grade);
            return node;
        }
        public void InsertBySubject(Grade grade)
        {
            Root = InsertBySubjectRecursive(Root, grade);
        }
        private GradeNode InsertBySubjectRecursive(GradeNode node, Grade grade)
        {
            if (node == null) return new GradeNode(grade);

            int comparison = string.Compare(grade.Subject.Name, node.Data.Subject.Name, StringComparison.OrdinalIgnoreCase);
            if (comparison < 0) node.Left = InsertBySubjectRecursive(node.Left, grade);
            else node.Right = InsertBySubjectRecursive(node.Right, grade);

            return node;
        }
        public void InsertByDate(Grade grade)
        {
            Root = InsertByDateRecursive(Root, grade);
        }
        private GradeNode InsertByDateRecursive(GradeNode node, Grade grade)
        {
            if (node == null) return new GradeNode(grade);

            if (grade.Date < node.Data.Date) node.Left = InsertByDateRecursive(node.Left, grade);
            else node.Right = InsertByDateRecursive(node.Right, grade);

            return node;
        }
        public void InOrderTraversal()
        {
            InOrderRecursive(Root);
        }
        private void InOrderRecursive(GradeNode node)
        {
            if (node == null) return;
            InOrderRecursive(node.Left);
            Console.WriteLine(node.Data);
            InOrderRecursive(node.Right);
        }
    }
}
