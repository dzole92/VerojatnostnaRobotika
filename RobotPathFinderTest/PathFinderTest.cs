using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Extensions;
using RobotPathFinder;
using Should;

namespace RobotPathFinderTest
{
    [TestClass]
    public class PathFinderTest
    {

		private IRobotGrid dummyRobotGrid(){
			var grid = new RobotGrid(6, 7, 10, 10, 14);

			return grid;
		}

		private IEnumerable<Node> dummyNodes() {
			Node node1 = new Node();
			Node node2 = new Node();
			Node node3 = new Node();
			Node node4 = new Node();
			return new Node[] {node1, node2, node3, node4};
		}

        [TestMethod]
        public void InitializeNode()
        {
            // ARRANGE
            var node = new Node();

            // ACT
            int id = 1;
            var index = new RobotPathFinder.NodePosition() {X = 1, Y = 2};
            node.Initialize(id, index, true);

            // ASSERT
            node.IsInitialized.ShouldBeTrue();
            node.Id.ShouldEqual(1);
            node.Index?.X.ShouldEqual(1);
            node.Index?.Y.ShouldEqual(2);
            node.IsUnavailable.HasValue.ShouldBeTrue();
            node.IsUnavailable?.ShouldBeTrue();
            node.Parent.ShouldBeNull();
        }

        [TestMethod]
        public void CheckNodeIsNotInitialized()
        {
            // ARRANGE
            // ACT
            var node = new Node();

            // ASSERT
            node.IsInitialized.ShouldBeFalse();
            node.IsUnavailable.ShouldBeNull();
            node.Index.ShouldBeNull();
            node.Parent.ShouldBeNull();
        }

        [TestMethod]
        public void InitializeGridAndNodeListsM()
        {
            // ARRANGE
			var grid = Substitute.For<IRobotGrid>();
			grid.SizeX.Returns(10);
			grid.SizeY.Returns(20);
			var returnThis = new Node[grid.SizeX, grid.SizeY];
			grid.When(x => x.Initialize(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())).Do(x => {
				grid.AllNodes.Returns(returnThis);
				grid.IsInitialized.Returns(true);
			});

			// ACT
			grid.Initialize(10, 20, 10, 10, 14);

			// ASSERT
			grid.Received(1).Initialize(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>());
			grid.IsInitialized.ShouldEqual(true);
			grid.AllNodes.Length.ShouldEqual(200);
			grid.AllNodes.GetLength(0).ShouldEqual(10);
			grid.AllNodes.GetLength(1).ShouldEqual(20);
		}

		[TestMethod]
        public void FindNeighboursFromNodeM()
        {
            // ARRANGE
			var grid = Substitute.For<IRobotGrid>();
			Node node = Substitute.For<Node>();
			
			List<Node> nodes = dummyNodes().ToList();
            grid.FindNeighbours(Arg.Any<Node>()).Returns(nodes);

			// ACT
			var neighbours = grid.FindNeighbours(node);

			// ASSERT
			grid.Received(1).FindNeighbours(Arg.Any<Node>());
			neighbours.Count().ShouldEqual(4);
		}

        [TestMethod]
        public void SelectTheRightNeighborM()
        {
			// ARRANGE
			var grid = Substitute.For<IRobotGrid>();
			Node currentNode = Substitute.For<Node>();
			Node node1 = Substitute.For<Node>();

			List<Node> nodes = dummyNodes().ToList();
			currentNode.When(y => y.SetNeighbours(nodes, grid)).Do(z => currentNode.Neighbours.Returns(nodes));
			grid.FindNeighbours(currentNode).Returns(x => {
				currentNode.SetNeighbours(nodes, grid);
				return nodes;
			});
			grid.SelectNextNode(Arg.Any<Node>()).Returns(x => {
				grid.FindNeighbours(currentNode);
				node1.SetParent(currentNode);
				return node1;
			});

			// ACT
			var nextNode = grid.SelectNextNode(currentNode);

			// ASSERT
			nextNode.ShouldEqual(node1);
		}


        [TestMethod]
        public void CalculateHeruisticFromNodeM()
        {
			// ARRANGE
			var grid = Substitute.For<IRobotGrid>();
			Node node = Substitute.For<Node>();
			grid.CalculateHeruisticFromNode(Arg.Any<Node>()).Returns(x =>
			{
			    node.Hn = 2;
				return true;
			});
			// ACT
			var t = grid.CalculateHeruisticFromNode(node);
			// ASSERT
			grid.Received(1).CalculateHeruisticFromNode(Arg.Any<Node>());
			node.Hn.ShouldEqual(2);
			t.ShouldEqual(true);
		}

		[TestMethod]
        public void CalculateG_fromNodeM()
        {
			// ARRANGE
			var grid = Substitute.For<IRobotGrid>();
			Node node = Substitute.For<Node>();
			Node node1 = Substitute.For<Node>();
			grid.CalculateGnFromNode(Arg.Any<Node>(), Arg.Any<Node>()).Returns(x => {
				node.Gn = 2;
				return true;
			});
			// ACT
			var t = grid.CalculateGnFromNode(node, node1);
			// ASSERT
			grid.Received(1).CalculateGnFromNode(Arg.Any<Node>(), Arg.Any<Node>());
			node.Gn.ShouldEqual(2);
			t.ShouldEqual(true);
		}

        [TestMethod]
        public void CalculateFnFromNodeM()
        {
			// ARRANGE
			var grid = Substitute.For<IRobotGrid>();
			Node node = Substitute.For<Node>();
			Node node1 = Substitute.For<Node>();
			grid.CalculateHeruisticFromNode(Arg.Any<Node>()).Returns(x => {
				node.Hn = 2;
				return true;
			});
			grid.CalculateGnFromNode(Arg.Any<Node>(), Arg.Any<Node>()).Returns(x => {
				node.Gn = 3;
				return true;
			});
			// ACT
			var t = grid.CalculateGnFromNode(node, node1);
			var t1 = grid.CalculateHeruisticFromNode(node);
			// ASSERT
			grid.Received(1).CalculateGnFromNode(Arg.Any<Node>(), Arg.Any<Node>());
			node.Gn.ShouldEqual(3);
			t.ShouldEqual(true);
			node.Fn.ShouldEqual(5);

		}

        [TestMethod]
        public void AcctualFindPath() {
			// ARRANGE
			var grid = dummyRobotGrid();
			grid.IsInitialized.ShouldBeTrue();
			var startPosition = new NodePosition {X = 2, Y = 1};
			var endPosition = new NodePosition {X = 5, Y = 4};
			// ACT
			var t = grid.FindPath(startPosition, endPosition);
			// ASSERT
			t.Count.ShouldEqual(4);
			t.Select(x=> x.Id).ShouldEqual(new [] {14,21,28,35});
		}

		[TestMethod]
		public void SetObstacles() {
			// ARRANGE
			var grid = dummyRobotGrid();
			grid.IsInitialized.ShouldBeTrue();
			NodePosition[] obstacalesPostions = new [] {new NodePosition {X=2,Y=3}, new NodePosition {X=4, Y=1}, new NodePosition { X =4, Y =3}, new NodePosition { X =5, Y =3} };

			//ACT
			grid.SetObstacles(obstacalesPostions);

			//ASSERT
			obstacalesPostions.ToList().ForEach(x => {
				grid.AllNodes[x.X, x.Y].IsUnavailable?.ShouldBeTrue();
			});

		}

		[TestMethod]
		public void FindActualPathWithObstacles() {
			// ARRANGE
			var grid = dummyRobotGrid();
			grid.IsInitialized.ShouldBeTrue();
			var startPosition = new NodePosition { X = 2, Y = 1 };
			var endPosition = new NodePosition { X = 5, Y = 4 };
			NodePosition[] obstacalesPostions = new[] {new NodePosition { X = 2, Y = 3 }, new NodePosition { X = 4, Y = 1 }, new NodePosition { X = 4, Y = 3 }, new NodePosition { X = 5, Y = 3 } };
			grid.SetObstacles(obstacalesPostions);


			//ACT
			var t = grid.FindPath(startPosition, endPosition);

			//ASSERT
			t.Count.ShouldEqual(5);
			t.Select(x => x.Id).ShouldEqual(new[] { 14, 15, 22, 29, 35 });
		}

		[TestMethod]
		public void UseIdsActualPathFind() {
			// ARRANGE
			var grid = dummyRobotGrid();
			grid.IsInitialized.ShouldBeTrue();
			var startPosition = 14;
			var endPosition = 35;
			var obstacalesPostions = new[] { 16,26,28,34 };
			grid.SetObstacles(obstacalesPostions);


			//ACT
			var t = grid.FindPath(startPosition, endPosition);

			//ASSERT
			t.Count.ShouldEqual(5);
			t.Select(x => x.Id).ShouldEqual(new[] { 14, 15, 22, 29, 35 });

		}

		[TestMethod]
		public void Example1() {
			// ARRANGE
			var grid = dummyRobotGrid();
			grid.IsInitialized.ShouldBeTrue();
			var startPosition = 14;
			var endPosition = 35;
			var obstacalesPostions = new[] { 16, 26, 28, 34, 22 };
			grid.SetObstacles(obstacalesPostions);


			//ACT
			var t = grid.FindPath(startPosition, endPosition);

			//ASSERT
			t.Count.ShouldEqual(6);
			t.Select(x => x.Id).ShouldEqual(new[] { 14, 20, 27, 33, 40, 35 });

		}

		[TestMethod]
		public void Example2() {
			// ARRANGE
			var grid = dummyRobotGrid();
			grid.IsInitialized.ShouldBeTrue();
			var startPosition = 14;
			var endPosition = 35;
			var obstacalesPostions = new[] { 16, 26, 28, 34, 22, 17};
			grid.SetObstacles(obstacalesPostions);


			//ACT
			var t = grid.FindPath(startPosition, endPosition);

			//ASSERT
			t.Count.ShouldEqual(6);
			t.Select(x => x.Id).ShouldEqual(new[] { 14, 20, 27, 33, 40, 35 });

		}

		[TestMethod]
		public void Example3CannotReachTheEnd() {
			// ARRANGE
			var grid = dummyRobotGrid();
			grid.IsInitialized.ShouldBeTrue();
			var startPosition = 14;
			var endPosition = 35;
			var obstacalesPostions = new[] { 16, 26, 28, 34, 22, 17, 18, 40 };
			grid.SetObstacles(obstacalesPostions);


			// ACT ASSERT
			Should.Core.Assertions.Assert.Throws<Exception>(() => {
				var t = grid.FindPath(startPosition, endPosition);
			});
		}




	}
}
