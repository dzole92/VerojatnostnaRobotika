using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace RobotPathFinder {
	public class RobotGrid : IRobotGrid {

		public RobotGrid() {

		}

		public RobotGrid(int sizeX, int sizeY, int horizontalCost, int verticalCost, int diagonalCost) {
			Initialize(sizeX, sizeY, horizontalCost, verticalCost, diagonalCost);
		}

		public int SizeX { get; private set; }
		public int SizeY { get; private set; }
		public Node StartNode { get; private set; }
		public Node EndNode { get; private set; }
		public Node[,] AllNodes { get; private set; }
		public List<Node> OpenNodes { get; } = new List<Node>();
		public List<Node> ClosedNodes { get; } = new List<Node>();
		public bool IsInitialized { get; private set; }
		public int HorizontalCost { get; private set; }
		public int VerticalCost { get; private set; }
		public int DiagonalCost { get; private set; }
		private readonly NodeComparer nodeComparer = new NodeComparer();


		public void Initialize(int x, int y, int horizontalCost, int verticalCost, int diagonalCost) {
			SizeX = y;
			SizeY = x;
			HorizontalCost = horizontalCost;
			VerticalCost = verticalCost;
			DiagonalCost = diagonalCost;
			AllNodes = new Node[SizeX, SizeY];
			var counter = 0;
			for (var i = 0; i < SizeX; i++)
				for (var j = 0; j < SizeY; j++) {
					AllNodes.SetValue(new Node(), i, j);
					AllNodes[i, j].Initialize(++counter, new NodePosition { X = i, Y = j }, false);
				}

			IsInitialized = true;
		}

		public List<Node> FindNeighbours(Node currentNode) {
			if (!IsInitialized || AllNodes.Length == 0 || !currentNode.IsInitialized) throw new Exception("Is not initialized");
			if (currentNode.IsUnavailable ?? false) throw new Exception("currentNode is Unavailable");

			var actualNeighbours = new ConcurrentBag<Node>();
			currentNode.NeighboursIndexes(SizeX, SizeY).AsParallel().ForAll(p => {
				try {
					var node = AllNodes.GetValue(p.X, p.Y) as Node;
					if (node == null) return;
					if (node.IsInitialized && !(node.IsUnavailable ?? false)) actualNeighbours.Add(node);
				} catch (Exception e) {
					Console.Write(e);
				}

			});

			currentNode.SetNeighbours(actualNeighbours.OrderBy(x => x.Id).ToList(), this);
			return currentNode.Neighbours;
		}

		public bool CalculateHeruisticFromNode(Node node) {
			if (!IsInitialized || EndNode == null || !EndNode.IsInitialized) throw new Exception("End point is not set.");
			if (!node.IsInitialized) throw new Exception("Current currentNode is not intialized.");
			node.Hn = Math.Abs(node.Position.X - EndNode.Position.X) + Math.Abs(node.Position.Y - EndNode.Position.Y);
			return true;
		}

		public bool CalculateGnFromNode(Node node, Node currentNode) {
			int newGn;
			switch (currentNode.CalculateMoveType(node, this)) {
				case MoveType.Horizontal:
					newGn = currentNode.Gn + HorizontalCost;
					break;
				case MoveType.Vertical:
					newGn = currentNode.Gn + VerticalCost;
					break;
				case MoveType.Diagonal:
					newGn = currentNode.Gn + DiagonalCost;
					break;
				default:
					throw new Exception("Unkown Move. Something went wrong.");
			}
			node.Gn = newGn;
			return true;
		}

		public Node SelectNextNode(Node currentNode) {
			if (!IsInitialized) throw new Exception("Grid is not initialized");
			if (FindNeighbours(currentNode).Count <= 0) return OpenNodes.LastOrDefault();
			var neighboursList = currentNode.Neighbours.Where(x => !ClosedNodes.Contains(x, nodeComparer)).ToList();
			OpenNodes.AddRange(neighboursList.Where(x => !OpenNodes.Contains(x, nodeComparer)));
			return neighboursList.OrderBy(x => x.Fn).ThenBy(x => x.Id).FirstOrDefault() ?? OpenNodes.LastOrDefault();
		}

		public List<Node> FindPath(Node start, Node end) {
			if (!IsInitialized) throw new Exception("First you need to Initialize the grid.");
			if (start?.IsUnavailable == true || end?.IsUnavailable == true) throw new Exception("Start currentNode or End currentNode is unavailable. Pick others.");
			StartNode = start;
			EndNode = end;
			CalculateHeruisticFromNode(StartNode);
			OpenNodes.Add(StartNode);

			var currentNode = StartNode;
			while (currentNode != null && !currentNode.Equals(EndNode)) {
				ClosedNodes.Add(currentNode);
				OpenNodes.Remove(currentNode);
				currentNode = SelectNextNode(currentNode);
				if (currentNode.Id == 29) {
					Console.Write("v");
				}
			}

			if (currentNode == null) throw new Exception("Cannot reach the end.");
			var pathList = new List<Node>();
			var nod = currentNode;
			nod.FindNeighbours(this);
			while (nod != null && !nod.Equals(StartNode)) {
				pathList.Add(nod);
				if (!nod.Neighbours.Any()) nod.FindNeighbours(this);
				nod = nod.Neighbours.OrderBy(x => x.Fn).FirstOrDefault(x => !(x.IsUnavailable ?? false));
			}
			if (nod == null) throw new Exception("Something went wrong. !!!!");
			pathList.Add(StartNode);
			pathList.Reverse();
			return pathList;
		}

		public List<Node> FindPath(NodePosition start, NodePosition end) {
			if (!IsInitialized) throw new Exception("First you need to Initialize the grid.");
			try {
				var startNode = AllNodes.GetValue(start.X, start.Y) as Node;
				var endNode = AllNodes.GetValue(end.X, end.Y) as Node;
				return FindPath(startNode, endNode);
			} catch (IndexOutOfRangeException error) {
				Console.WriteLine(error);
				throw;
			} catch (Exception ex) {
				Console.WriteLine(ex);
				throw;
			}
		}

		public List<Node> FindPath(int startId, int endId) {
			Node start = new Node(), end = new Node();
			for (int i = 0; i < SizeX; i++)
				for (int j = 0; j < SizeY; j++) {
					if (AllNodes[i, j].Id == startId) start = AllNodes[i, j];
					if (AllNodes[i, j].Id == endId) end = AllNodes[i, j];
				}

			if (!start.IsInitialized || !end.IsInitialized) throw new Exception("Could not found star or end.");
			return FindPath(start, end);
		}

		public void SetObstacles(NodePosition[] obstacalesPostions) {
			obstacalesPostions.AsParallel().ForAll(x => {
				try {
					var node = AllNodes.GetValue(x.X, x.Y) as Node;
					if (node == null) return;
					node.IsUnavailable = true;
					AllNodes.SetValue(node, x.X, x.Y);
				} catch (Exception e) {
					Console.WriteLine(e);
				}
			});
		}

		public void SetObstacles(int[] obstacalesPostions) {
			obstacalesPostions.AsParallel().ForAll(x => {
				for (int i = 0; i < SizeX; i++)
				for (int j = 0; j < SizeY; j++)
					if (AllNodes[i, j].Id == x) AllNodes[i, j].IsUnavailable = true;
			});
		}

	}


}