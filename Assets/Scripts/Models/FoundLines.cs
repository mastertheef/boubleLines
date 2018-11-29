using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    public class FoundLines
    {
        public FoundLines()
        {
            this.horizontal = new List<Node>();
            this.vertical = new List<Node>();
            this.leftDiag = new List<Node>();
            this.rightDiag = new List<Node>();
        }

        public List<Node> horizontal { get; set; }
        public List<Node> vertical { get; set; }
        public List<Node> leftDiag { get; set; }
        public List<Node> rightDiag { get; set; }

        public bool HaveLines(int minLength)
        {
            return horizontal.Count >= minLength ||
                vertical.Count >= minLength ||
                leftDiag.Count >= minLength ||
                rightDiag.Count >= minLength;
        }
    }
}
