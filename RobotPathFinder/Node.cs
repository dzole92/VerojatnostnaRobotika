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
		public List<Node> Neighbours { get; } = new List<Node>();

		public NodePosition Position {
			get {
				if (!Index.HasValue) throw new Exception("Node is not set");
				return Index.Value;
			}
		}

		public void SetNeighbours(List<Node> neighbours, IRobotGrid grid) {
			neighbours.AsParallel().ForAll(x => {
				if(x.Parent == null && x.IsInitialized) { 
					Neighbours.Add(x);
					return;
				}
				var oldGn = x.Gn;
				if (x.CalculateGnFromNode(this, grid) < oldGn)
					x.Parent = this;
				else
					x.Gn = oldGn;
				Neighbours.Add(x);
			});
		}
		public void Setparent(Node parent) { Parent = parent; }

		public void Initialize(int id, NodePosition index, bool isUnavailable)
        {
            Id = id;
            Index = index;
            IsUnavailable = isUnavailable;
            IsInitialized = true;
        }

		public override bool Equals(object obj) {
			if (obj != null && typeof(Node) == obj.GetType()) return ((Node)obj).Id == Id;
			return false;

		}


	}

	public class NodeComparer : IEqualityComparer<Node> {

		public bool Equals(Node x, Node y) => y != null && x != null && x.Id == y.Id;

		public int GetHashCode(Node obj) { throw new NotImplementedException(); }

	}
}
