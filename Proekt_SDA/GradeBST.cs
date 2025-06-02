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
                Left = null;
                Right = null;
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
