using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotPathFinder
{
    public class Node
    {

		public bool IsInitialized { get; set; }
        public int Id { get; set; }
        public NodePosition? Index { get; set; }
        public bool? IsUnavailable { get; set; }
        public Node Parent { get; private set; }
		public int Hn { get; set; }
		public int Gn { get; set; }
		public int Fn => Hn + Gn;
		public IEnumerable<Node> Neighbours { get; private set; }

		public void SetNeighbours(IEnumerable<Node> neighbours) { Neighbours = neighbours; }
		public void Setparent(Node parent) { Parent = parent; }

		public void Initialize(int id, NodePosition index, bool isUnavailable)
        {
            Id = id;
            Index = index;
            IsUnavailable = isUnavailable;
            IsInitialized = true;
        }
    }
}
