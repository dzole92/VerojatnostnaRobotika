using System.Collections.Generic;
using RobotPathFinder;

namespace RobotPathFinderTest {
	public interface IRobotGrid {

		int SizeX { get; }
		int SizeY { get; }
		Node StartNode { get; }
		Node EndNode { get; }
		Node[,] AllNodes { get; }
		IEnumerable<Node> OpenNodes { get; }
		IEnumerable<Node> ClosedNodes { get; }
		bool IsInitialized { get; }

		void Initialize(int x, int y, int horizontalCost, int verticalCost, int diagonalCost);

		IEnumerable<Node> FindNeighbours(Node node);

		bool CalculateHeruisticFromNode(Node node);

		bool CalculateGnFromNode(Node node);

		Node SelectNextNode(Node currentNode);

	}
}