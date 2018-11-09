using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RobotPathFinder {
	public class RobotGrid : IRobotGrid {

		public RobotGrid() {
			
		}

		public RobotGrid(int sizeX, int sizeY, int horizontalCost, int verticalCost, int diagonalCost) {
			SizeX = sizeX;
			SizeY = sizeY;
			HorizontalCost = horizontalCost;
			VerticalCost = verticalCost;
			DiagonalCost = diagonalCost;
			AllNodes = new Node[SizeX, SizeY];
			IsInitialized = true;
		}

		public int SizeX { get; private set; }
		public int SizeY { get; private set; }
		public Node StartNode { get; private set; }
		public Node EndNode { get; private set; }
		public Node[,] AllNodes { get; private set; }
		public List<Node> OpenNodes { get;} = new List<Node>();
		public List<Node> ClosedNodes { get;} = new List<Node>();
		public bool IsInitialized { get; private set; }
		public int HorizontalCost { get; private set; }
		public int VerticalCost { get; private set; }
		public int DiagonalCost { get; private set; }
		private readonly NodeComparer nodeComparer = new NodeComparer();


		public void Initialize(int x, int y, int horizontalCost, int verticalCost, int diagonalCost) {
			SizeX = x;
			SizeY = y;
			HorizontalCost = horizontalCost;
			VerticalCost = verticalCost;
			DiagonalCost = diagonalCost;
			AllNodes = new Node[SizeX, SizeY];
			IsInitialized = true;
		}

		public List<Node> FindNeighbours(Node currentNode) {
			if (!IsInitialized || AllNodes.Length == 0 || !currentNode.IsInitialized) throw new Exception("Is not initialized");
			if (currentNode.IsUnavailable ?? false) throw new Exception("currentNode is Unavailable");

			var actualNeighbours = new ConcurrentBag<Node>();
			currentNode.NeighboursIndexes(SizeX, SizeY).AsParallel().ForAll(p => {
				if(AllNodes[p.X, p.Y].IsInitialized && !(AllNodes[p.X, p.Y].IsUnavailable ?? false)) actualNeighbours.Add(AllNodes[p.X, p.Y]);
			});
			var neighboursList = actualNeighbours.Where(x=> x.Parent == null || !Equals(x.Parent, currentNode.Parent)).OrderBy(x => x.Id).ToList();
			OpenNodes.AddRange(neighboursList.Where(x=> !OpenNodes.Contains(x, nodeComparer)));
			currentNode.SetNeighbours(neighboursList, this);
			return currentNode.Neighbours;
		}

		public bool CalculateHeruisticFromNode(Node node) {
			if(!IsInitialized || EndNode == null || !EndNode.IsInitialized) throw new Exception("End point is not set.");
			if(!node.IsInitialized) throw new Exception("Current currentNode is not intialized.");
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