using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace CodeShortsApp
{
    public class PathSumTests
    {

        public class PathSumTestTryGetValueAt
        {

            public int[,] Matrix;

            public PathSumTestTryGetValueAt()
            {
                Matrix = new[,] {{0, 0, 0}, {1, 1, 1}, {2, 2, 2}};
            }

            [Theory]
            [MemberData(nameof(TryGetValueAt_TestData))]
            public void TryGetValueAt_Test(int i, int j, int expectedVal, bool expectedResult)
            {
                var pathSum = new PathSum();
                pathSum.Matrix = Matrix;
                bool result = pathSum.TryGetValueAt(out int val, i, j);
                Assert.Equal(expectedResult, result);
                Assert.Equal(expectedVal, val);
            }

            public static IEnumerable<object[]> TryGetValueAt_TestData()
            {
                // ReSharper disable HeapView.BoxingAllocation
                yield return new object[] {0, 0, 0, true};
                yield return new object[] {1, 1, 1, true};
                yield return new object[] {2, 2, 2, true};
                yield return new object[] {4, 0, 0, false};
                yield return new object[] {0, 4, 0, false};
                yield return new object[] {-1, 0, 0, false};
                yield return new object[] {0, -1, 0, false};

                // Resharper enable HeapView.BoxingAllocation

            }

            [Fact]
            public void CreateNextNodeHasCorrectHistory()
            {
                // from a node create a new node that uses
                var pathSum = new PathSum();
                var orgNode = new Node() {Position = new[] {0, 0}};
                var history = new List<Direction>() {Direction.E, Direction.N, Direction.S, Direction.S};
                //int addedValue = 7834;
                var newDir = Direction.E;
                orgNode.History = new List<Direction>(history);

                var newNode = pathSum.CreateNext(orgNode, newDir);

                history.Add(newDir);

                Assert.Subset(history.ToHashSet(), newNode.History.ToHashSet());
                Assert.Superset(history.ToHashSet(), newNode.History.ToHashSet());

                //Assert.Equal(addedValue, newNode.Value);

            }

            [Theory]
            [MemberData(nameof(NodeDirectionChangesAsExpected_Data))]
            public void NodeDirectionChangesAsExpected(Direction[] moves, int expX, int expY)
            {
                var pathSum = new PathSum();
                var orgNode = new Node();
                orgNode.History = new List<Direction>();
                orgNode.Position = new[] {0, 0};

                foreach (var direction in moves)
                {
                    orgNode = pathSum.CreateNext(orgNode, direction);
                }

                Assert.Equal(orgNode.Position[0], expX);
                Assert.Equal(orgNode.Position[1], expY);
            }

            public static IEnumerable<object[]> NodeDirectionChangesAsExpected_Data()
            {
                yield return new object[] {new Direction[] {Direction.E, Direction.E, Direction.E}, 3, 0};
                yield return new object[] {new Direction[] {Direction.N}, 0, 1};
                yield return new object[] {new Direction[] {Direction.S}, 0, -1};
                yield return new object[] {new Direction[] {Direction.N, Direction.S}, 0, 0};
                yield return new object[] {new Direction[] {Direction.E, Direction.N}, 1, 1};
            }







            public class PathSumQueueTests : IClassFixture<PathSumFixture>, IDisposable
            {
                private PathSumFixture _fixture;

                public PathSumQueueTests(PathSumFixture fixture)
                {
                    _fixture = fixture;
                }


                [Theory]
                [InlineData(0, 0, 2)]
                [InlineData(0, 3, 1)]
                [InlineData(1, 1, 3)]
                [InlineData(2, 0, 2)]
                [InlineData(2, 3, 1)]
                public void CorrectNumNeighboursAddedToQueue(int initialNodeX, int initialNodeY, int expNumber)
                {
                    var node = new Node()
                    {
                        History = new List<Direction>(),
                        Position = new[] {initialNodeX, initialNodeY}
                    };
                    _fixture.PathSum.AddNeighboursToQueue(node);
                    Assert.Equal(expNumber, _fixture.PathSum.Queue.Count);
                }
                
                [Theory]
                [InlineData(0, 0, 1)]
                [InlineData(1, 0, 16)]
                [InlineData(2, 1, 544)]
                [InlineData(1, 2, 68)]
                [InlineData(0, 3, 136)]
                public void CorrectTopPriorityAddedToQueue(int initialNodeX, int initialNodeY, int topPriority)
                {
                    var node = new Node()
                    {
                        History = new List<Direction>(),
                        Position = new[] {initialNodeX, initialNodeY}
                    };
                    
                    _fixture.PathSum.AddNeighboursToQueue(node);
                    Assert.Equal(_fixture.PathSum.Queue.Values.First(), topPriority);
                }
                
                
                

                public void Dispose()
                {
                    _fixture.PathSum.Queue.Clear();
                }
            }

            public class PathSumFixture
            {
                public PathSum PathSum;

                public PathSumFixture()
                {
                    PathSum = new PathSum()
                    {
                        Queue = new SortedDictionary<Node, int>(),
                        Matrix = new[,]
                        {
                            {0, 1, 4, 8},
                            {16, 32, 64, 128},
                            {256, 512, 1024, 2048}
                        }
                    };
                }
            }

        }



    }
}