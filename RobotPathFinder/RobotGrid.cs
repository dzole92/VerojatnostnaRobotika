using System.Collections.Generic;

namespace RobotPathFinder {
	public class RobotGrid : IRobotGrid {

		public RobotGrid(int sizeX, int sizeY) {
			SizeX = sizeX;
			SizeY = sizeY;
		}

		public int SizeX { get; }
		public int SizeY { get; }
		public Node StartNode { get; private set; }
		public Node EndNode { get; private set; }
		public Node[,] AllNodes { get; private set; }
		public IEnumerable<Node> OpenNodes { get; private set; }
		public IEnumerable<Node> ClosedNodes { get; private set; }
		public bool IsInitialized { get; private set; }


		public void Initialize(int x, int y, int horizontalCost, int verticalCost, int diagonalCost) { throw new System.NotImplementedException(); }
		public IEnumerable<Node> FindNeighbours(Node node) { throw new System.NotImplementedException(); }
		public bool CalculateHeruisticFromNode(Node node) { throw new System.NotImplementedException(); }
		public bool CalculateGnFromNode(Node node) { throw new System.NotImplementedException(); }
		public Node SelectNextNode(Node currentNode) { throw new System.NotImplementedException(); }

	}
}