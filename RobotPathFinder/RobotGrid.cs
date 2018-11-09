using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
		public List<Node> OpenNodes { get; private set; } = new List<Node>();
		public List<Node> ClosedNodes { get; private set; } = new List<Node>();
		public bool IsInitialized { get; private set; }


		public void Initialize(int x, int y, int horizontalCost, int verticalCost, int diagonalCost) { throw new System.NotImplementedException(); }

		public List<Node> FindNeighbours(Node currentNode) {
			if (!IsInitialized || AllNodes.Length == 0 || !currentNode.IsInitialized) throw new Exception("Is not initialized");
			if (currentNode.IsUnavailable ?? false) throw new Exception("currentNode is Unavailable");

			var actualNeighbours = new ConcurrentBag<Node>();
			currentNode.NeighboursIndexes(SizeX, SizeY).AsParallel().ForAll(p => {
				if(AllNodes[p.X, p.Y].IsInitialized && !(AllNodes[p.X, p.Y].IsUnavailable ?? false)) actualNeighbours.Add(AllNodes[p.X, p.Y]);
			});
			currentNode.SetNeighbours(actualNeighbours.OrderBy(x => x.Id).ToList());
			return currentNode.Neighbours.ToList();
		}

		public bool CalculateHeruisticFromNode(Node node) {
			if(!IsInitialized || EndNode == null || !EndNode.IsInitialized) throw new Exception("End point is not set.");
			if(!node.IsInitialized) throw new Exception("Current currentNode is not intialized.");
			node.Hn = Math.Abs(node.Position.X - EndNode.Position.X) + Math.Abs(node.Position.Y - EndNode.Position.Y);
			return true;
		}

		public bool CalculateGnFromNode(Node node) { throw new System.NotImplementedException(); }
		public Node SelectNextNode(Node currentNode) { throw new System.NotImplementedException(); }

		public List<Node> FindPath(Node start, Node end) {
			if(!IsInitialized) throw new Exception("First you need to Initialize the grid.");
			if(start?.IsUnavailable == true || end?.IsUnavailable == true) throw new Exception("Start currentNode or End currentNode is unavailable. Pick others.");
			StartNode = start;
			EndNode = end;

			ClosedNodes.Add(StartNode);
			var t = FindNeighbours(StartNode); 
			OpenNodes.AddRange(FindNeighbours(StartNode));

			//TODO: Not finished
			return null;
		}

	}
}