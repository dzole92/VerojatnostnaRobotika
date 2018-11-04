using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotPathFinder;
using Should;

namespace RobotPathFinderTest
{
    [TestClass]
    public class PathFinderTest
    {
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

            // ACT

            // ASSERT

        }

        [TestMethod]
        public void FindNeighborsFromNodeM()
        {
            // ARRANGE

            // ACT

            // ASSERT

        }

        [TestMethod]
        public void SelectTheRightNeighborM()
        {
            // ARRANGE

            // ACT

            // ASSERT

        }


        [TestMethod]
        public void CalculateHeruisticFromNodeM()
        {
            // ARRANGE

            // ACT

            // ASSERT

        }

        [TestMethod]
        public void CalculateG_fromNodeM()
        {
            // ARRANGE

            // ACT

            // ASSERT

        }

        [TestMethod]
        public void CalculateFnFromNodeM()
        {
            // ARRANGE

            // ACT

            // ASSERT

        }

        

        


        


        
    }
}
