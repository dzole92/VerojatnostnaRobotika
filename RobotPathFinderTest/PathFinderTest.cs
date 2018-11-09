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
			var grid = Substitute.For<IRobotGrid>();
			grid.SizeX.Returns(3);
			grid.SizeY.Returns(3);

			Node[,] allNodes = new Node[3,3];

			grid.AllNodes.Returns(allNodes);
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
			
			IEnumerable<Node> nodes = dummyNodes();
			node.When(y => y.SetNeighbours(nodes)).Do(z => node.Neighbours.Returns(nodes));
			grid.FindNeighbours(node).Returns(x=> {
				node.SetNeighbours(nodes);
				return nodes;
			});
			// ACT
			var neighbours = grid.FindNeighbours(node);

			// ASSERT
			grid.Received(1).FindNeighbours(Arg.Any<Node>());
			neighbours.Count().ShouldEqual(4);
			node.Neighbours.ShouldEqual(neighbours);
		}

        [TestMethod]
        public void SelectTheRightNeighborM()
        {
			// ARRANGE
			var grid = Substitute.For<IRobotGrid>();
			Node currentNode = Substitute.For<Node>();
			Node node1 = Substitute.For<Node>();

			IEnumerable<Node> nodes = dummyNodes();
			currentNode.When(y => y.SetNeighbours(nodes)).Do(z => currentNode.Neighbours.Returns(nodes));
			grid.FindNeighbours(currentNode).Returns(x => {
				currentNode.SetNeighbours(nodes);
				return nodes;
			});
			grid.SelectNextNode(Arg.Any<Node>()).Returns(x => {
				grid.FindNeighbours(currentNode);
				node1.Setparent(currentNode);
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
			grid.CalculateHeruisticFromNode(node).Returns(x => { node.Hn.Returns(2);
				return true;
			});
			// ACT
			var t = grid.CalculateHeruisticFromNode(node);
			// ASSERT
			grid.Received(1).CalculateHeruisticFromNode(node);
			node.Hn.ShouldEqual(2);
			t.ShouldEqual(true);
		}

		[TestMethod]
        public void CalculateG_fromNodeM()
        {
			// ARRANGE
			var grid = Substitute.For<IRobotGrid>();
			Node node = Substitute.For<Node>();
			grid.CalculateGnFromNode(Arg.Is(node)).Returns(x => {
				node.Gn = 2;
				return true;
			});
			// ACT
			var t = grid.CalculateGnFromNode(node);
			// ASSERT
			grid.Received(1).CalculateGnFromNode(node);
			node.Gn.ShouldEqual(2);
			t.ShouldEqual(true);
		}

        [TestMethod]
        public void CalculateFnFromNodeM()
        {
			// ARRANGE
			var grid = Substitute.For<IRobotGrid>();
			Node node = Substitute.For<Node>();
			grid.CalculateHeruisticFromNode(node).Returns(x => {
				node.Hn = 2;
				return true;
			});
			grid.CalculateGnFromNode(node).Returns(x => {
				node.Gn = 3;
				return true;
			});
			// ACT
			var t = grid.CalculateGnFromNode(node);
			var t1 = grid.CalculateHeruisticFromNode(node);
			// ASSERT
			grid.Received(1).CalculateGnFromNode(node);
			node.Gn.ShouldEqual(3);
			t.ShouldEqual(true);
			node.Fn.ShouldEqual(5);

		}

        

        


        


        
    }
}
