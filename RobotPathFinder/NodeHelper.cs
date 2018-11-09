using System.Collections.Generic;
using System.Linq;

namespace RobotPathFinder {
	public static class NodeHelper {

		public static IEnumerable<Node> FindNeighbours(this Node node, IRobotGrid grid) => grid.FindNeighbours(node);
		public static int CalculateHeruisticFromNode(this Node node, IRobotGrid grid) {
			grid.CalculateHeruisticFromNode(node);
			return node.Hn;
		}
		public static int CalculateGnFromNode(this Node node, IRobotGrid grid) {
			grid.CalculateGnFromNode(node);
			return node.Gn;
		}

		public static Node SelectNextNode(this Node currentNode, IRobotGrid grid) => grid.SelectNextNode(currentNode);

		public static List<NodePosition> NeighboursIndexes(this Node currentNode, int sizeX, int sizeY) {
			var x = currentNode.Position.X;
			var y = currentNode.Position.Y;
			return new List<NodePosition> {
				new NodePosition {X = x - 1, Y = y - 1},
				new NodePosition {X = x, Y = y - 1},
				new NodePosition {X = x + 1, Y = y - 1},
				new NodePosition {X = x + 1, Y = y},
				new NodePosition {X = x + 1, Y = y + 1},
				new NodePosition {X = x, Y = y + 1},
				new NodePosition {X = x - 1, Y = y + 1},
				new NodePosition {X = x - 1, Y = y}
			}.Where(p => p.X >= 0 && p.X < sizeX && p.Y >= 0 && p.Y < sizeY).ToList();
		}

	}
}