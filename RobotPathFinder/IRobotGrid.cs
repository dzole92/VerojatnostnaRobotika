using System.Collections.Generic;

namespace RobotPathFinder {
	public interface IRobotGrid {

		int SizeX { get; }
		int SizeY { get; }
		Node StartNode { get; }
		Node EndNode { get; }
		Node[,] AllNodes { get; }
		List<Node> OpenNodes { get; }
		List<Node> ClosedNodes { get; }
		bool IsInitialized { get; }

		void Initialize(int x, int y, int horizontalCost, int verticalCost, int diagonalCost);

		List<Node> FindNeighbours(Node currentNode);

		bool CalculateHeruisticFromNode(Node node);

		bool CalculateGnFromNode(Node node);

		Node SelectNextNode(Node currentNode);

		List<Node> FindPath(Node start, Node end);

	}
}