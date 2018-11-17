using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotPathFinder {
	public static class NodeHelper {

		public static IEnumerable<Node> FindNeighbours(this Node node, IRobotGrid grid) => grid.FindNeighbours(node);
		public static int CalculateHeruisticFromNode(this Node node, IRobotGrid grid) {
			grid.CalculateHeruisticFromNode(node);
			return node.Hn;
		}
		public static int CalculateGnFromNode(this Node node, Node currentNode, IRobotGrid grid) {
			grid.CalculateGnFromNode(node, currentNode);
			return node.Gn;
		}

		public static Node SelectNextNode(this Node currentNode, IRobotGrid grid) => grid.SelectNextNode(currentNode);

		public static List<NodePosition> NeighboursIndexes(this Node currentNode, int sizeX, int sizeY) {
			var x = currentNode.Position.X;
			var y = currentNode.Position.Y;
			return new List<NodePosition> {
				new NodePosition {X = x - 1, Y = y - 1, AdditionalData = MoveType.Diagonal.ToString("G")},
				new NodePosition {X = x, Y = y - 1, AdditionalData = MoveType.Vertical.ToString("G")},
				new NodePosition {X = x + 1, Y = y - 1, AdditionalData = MoveType.Diagonal.ToString("G")},
				new NodePosition {X = x + 1, Y = y, AdditionalData = MoveType.Horizontal.ToString("G")},
				new NodePosition {X = x + 1, Y = y + 1, AdditionalData = MoveType.Diagonal.ToString("G")},
				new NodePosition {X = x, Y = y + 1, AdditionalData = MoveType.Vertical.ToString("G")},
				new NodePosition {X = x - 1, Y = y + 1, AdditionalData = MoveType.Diagonal.ToString("G")},
				new NodePosition {X = x - 1, Y = y, AdditionalData = MoveType.Horizontal.ToString("G")}
			}.Where(p => p.X >= 0 && p.X < sizeY && p.Y >= 0 && p.Y < sizeX).ToList();
		}

		public static MoveType CalculateMoveType(this Node currentNode, Node node, IRobotGrid grid) {
			return (MoveType)Enum.Parse(typeof(MoveType),
										currentNode.NeighboursIndexes(grid.SizeX, grid.SizeY)
											.First(x => x.X == node.Position.X && x.Y == node.Position.Y)
											.AdditionalData);
		}

	}

	public enum MoveType {
		Horizontal,
		Vertical,
		Diagonal
	}
}