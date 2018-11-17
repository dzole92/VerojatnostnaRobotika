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
		int HorizontalCost { get; }
		int VerticalCost { get; }
		int DiagonalCost { get; }

		void Initialize(int x, int y, int horizontalCost, int verticalCost, int diagonalCost);

		List<Node> FindNeighbours(Node currentNode);

		bool CalculateHeruisticFromNode(Node node);

		bool CalculateGnFromNode(Node node, Node currentNode);

		Node SelectNextNode(Node currentNode);

		List<Node> FindPath(Node start, Node end);
		List<Node> FindPath(NodePosition start, NodePosition end);
		List<Node> FindPath(int startId, int endId);

		void SetObstacles(NodePosition[] obstacalesPostions);
		void SetObstacles(int[] obstacalesPostions);
		void SetObstacles(int obstacalesPostions);

	}
}