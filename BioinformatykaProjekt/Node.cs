using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioinformatykaProjekt
{
    public class Node
    {
        public string Value { get; set; }
        public int Used { get; set; }
        public Dictionary<Node, int> NextNodes;

        public Node(string value)
        {
            Value = value;
            Used = 0;
            NextNodes = new Dictionary<Node, int>();
        }

        public void Use()
        {
            Used++;
        }
    }
}
