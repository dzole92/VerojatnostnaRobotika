using System.Collections.Generic;

namespace RobotPathFinder {
	public static class NodeHelper {

		static IEnumerable<Node> FindNeighbours(this Node node, IRobotGrid grid) => grid.FindNeighbours(node);
		static int CalculateHeruisticFromNode(this Node node, IRobotGrid grid) { 
			grid.CalculateHeruisticFromNode(node);
			return node.Hn;
		}
		static int CalculateGnFromNode(this Node node, IRobotGrid grid) {
			grid.CalculateGnFromNode(node);
			return node.Gn;
		}

		static Node SelectNextNode(this Node currentNode, IRobotGrid grid) => grid.SelectNextNode(currentNode);

	}
}