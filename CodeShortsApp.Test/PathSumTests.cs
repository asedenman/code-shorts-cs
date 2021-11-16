using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

namespace CodeShortsApp
{
    public class PathSumTests
    {
        public class TryGetValueAtTests
        {
            public int[,] Matrix;

            public TryGetValueAtTests()
            {
                Matrix = new[,] {{0, 0, 0}, {1, 1, 1}, {2, 2, 2}};
            }

            [Theory]
            [MemberData(nameof(TryGetValueAt_TestData))]
            public void TryGetValueAt_Test(int i, int j, int expectedVal, bool expectedResult)
            {
                var pathSum = new PathSum(Matrix);
                bool result = pathSum.GetValueAtOrDefault(out int val, i, j);
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
        }

        public class NodeTests
        {
            [Fact]
            public void CreateNextNodeHasCorrectHistory()
            {
                // from a node create a new node that uses
                var pathSum = new PathSum(null);
                var orgNode = new Node() {Position = new[] {0, 0}};
                var history = new List<Direction>() {Direction.E, Direction.N, Direction.S, Direction.S};
                //int addedValue = 7834;
                var newDir = Direction.E;
                orgNode.History = new List<Direction>(history);

                var newNode = pathSum.CreateNext(orgNode, newDir, 0);

                history.Add(newDir);

                Assert.Subset(history.ToHashSet(), newNode.History.ToHashSet());
                Assert.Superset(history.ToHashSet(), newNode.History.ToHashSet());

                //Assert.Equal(addedValue, newNode.Value);
            }

            [Theory]
            [MemberData(nameof(NodeDirectionChangesAsExpected_Data))]
            public void NodeDirectionChangesAsExpected(Direction[] moves, int expX, int expY)
            {
                var pathSum = new PathSum(null);
                var orgNode = new Node();
                orgNode.History = new List<Direction>();
                orgNode.Position = new[] {0, 0};

                foreach (var direction in moves)
                {
                    orgNode = pathSum.CreateNext(orgNode, direction, 0);
                }

                Assert.Equal(expX, orgNode.Position[1]);
                Assert.Equal(expY, orgNode.Position[0]);
            }

            public static IEnumerable<object[]> NodeDirectionChangesAsExpected_Data()
            {
                yield return new object[] {new Direction[] {Direction.E, Direction.E, Direction.E}, 3, 0};
                yield return new object[] {new Direction[] {Direction.N}, 0, -1};
                yield return new object[] {new Direction[] {Direction.S}, 0, 1};
                yield return new object[] {new Direction[] {Direction.N, Direction.S}, 0, 0};
                yield return new object[] {new Direction[] {Direction.E, Direction.N}, 1, -1};
            }
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
                Assert.Equal(expNumber, _fixture.PathSum.PriorityQueue.Count);
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
                _fixture.PathSum.GetValueAtOrDefault(out int val, initialNodeX, initialNodeY);
                node.Value = val;
                _fixture.PathSum.AddNeighboursToQueue(node);
                Assert.Equal(topPriority, _fixture.PathSum.PriorityQueue.Dequeue().Value);
            }
            
            [Fact]
            public void CorrectlyAddFirstColToQueue()
            {
                _fixture.PathSum.AddFirstColToQueue();
                Assert.Equal(0 ,_fixture.PathSum.PriorityQueue.Dequeue().Value);
                Assert.Equal(16 ,_fixture.PathSum.PriorityQueue.Dequeue().Value);
                Assert.Equal(256 ,_fixture.PathSum.PriorityQueue.Dequeue().Value);
                Assert.Equal(0, _fixture.PathSum.PriorityQueue.Count);
            }
            
            [Theory]
            [InlineData(0, 0, 0, false)]
            [InlineData(2, 0, 256, false)]
            [InlineData(0, 1, 1, false)]
            [InlineData(1, 3, 128, true)]
            [InlineData(2, 3, 2048, true)]
            public void FoundGoalSuccess(int nodeI, int nodeJ, int expVal, bool expResult)
            {
                var node = new Node() {Position = new[] {nodeI, nodeJ}};
                _fixture.PathSum.GetValueAtOrDefault(out int val, nodeI, nodeJ);
                Assert.Equal(expVal, val);
                Assert.Equal(expResult, _fixture.PathSum.CheckNodeIsGoal(node));
            }
            
            public void Dispose()
            {
                _fixture.PathSum.PriorityQueue.Clear();
            }
        }

        public class PathSumSolveTests
        {
            [Fact]
            public void CorrectResultFoundBasicMatrix()
            {
                var pathSum = new PathSum(new[,]
                {
                    {0, 1, 4, 8},
                    {16, 32, 64, 128},
                    {256, 512, 1024, 2048}
                });
                Assert.Equal("13", pathSum.Solve());
            }
            [Fact]
            public void CorrectResultFoundMatrixEulerTest()
            {
                var pathSum = new PathSum(new[,]
                {
                    {131,     673,   234, 103,  18},
                    {201, 96, 342, 965, 150},
                    {630 ,803 ,746 ,422 ,111},
                    {537, 699, 497, 121, 956},
                    {805, 732, 524, 37, 331}
                });
                var exp = (201 + 96 + 342 + 234 + 103 + 18).ToString();
                Assert.Equal(exp, pathSum.Solve());
            }
        }

        
        public class LineCountAndLengthTests
        {
            public PathSum PathSum;
            private const string TestFileNameSnub = "TestData/pathSum_LineCountAndLength_";
            
            public LineCountAndLengthTests()
            {
                PathSum = new PathSum();
            }
            [Fact]
            public void OneEntry()
            {
                DoTest("1Entry.txt", 1, 1);
            }

            [Fact]
            public void BlankSpaces()
            {
                DoTest("BlankSpaces.txt", 3, 2);
            }

            [Fact]
            public void RowMismatch()
            {
                Assert.Throws<InvalidDataException>(()=> DoTest("RowMismatch.txt", 0, 0));
            }

            [Fact]
            public void EmptyFile()
            {
                DoTest("Empty.txt", 0, 0);
            }

            [Fact]
            public void Valid()
            {
                DoTest("Valid.txt", 2, 2);
            }
            
            public void DoTest(string filename, int expLineCount, int expLineLength)
            {
                int lineCount;
                int lineLength;
                using (var sr = new StreamReader(TestFileNameSnub + filename))
                {
                    (lineCount, lineLength) = PathSum.LineCountAndLineLength(sr);
                }
                Assert.Equal((expLineCount,expLineLength),(lineCount, lineLength) );
            }
        }

        public class ReadToMatrixTests
        {
            private PathSum PathSum;
            private const string TestFileNameSnub = "TestData/pathSum_ReadToMatrixTests_";
            public ReadToMatrixTests()
            {
                PathSum = new PathSum();
            }

            // public void RunTarget(int[,] refMatrix, string fileName)
            // {
            //     using (var sr = new StreamReader(TestFileNameSnub + fileName))
            //     {
            //         PathSum.ReadToMatrix(refMatrix, sr);
            //     }
            // }

            public void RunTarget(int[,] expMatrix, [CallerMemberName] string caller = "")
            {
                var actual = new int[expMatrix.GetLength(0), expMatrix.GetLength(1)];
                using (var sr = new StreamReader($"{TestFileNameSnub}{caller}.txt"))
                {
                    PathSum.ReadToMatrix(actual, sr);
                }
                Assert.Equal(expMatrix, actual);
            }
            
            [Fact]
            public void OneEntry()
            {
                var expMatrix = new int[,] {{245}};
                RunTarget(expMatrix);
            }

            [Fact]
            public void OneCol()
            {
                var expMatrix = new int[,] {{243}, {32}, {567}, {45356}, {77456}, {34322}, {334}, {5}, {1}, {0}};
                RunTarget(expMatrix);
            }
            
            [Fact]
            public void OneRow()
            {
                var expMatrix = new int[,] {{10, 11, 23, 34, 55}};
                RunTarget(expMatrix);
            }

            [Fact]
            public void MatrixFromEuler()
            {
                var expMatrix = new[,]
                {
                    {131,     673,   234, 103,  18},
                    {201, 96, 342, 965, 150},
                    {630 ,803 ,746 ,422 ,111},
                    {537, 699, 497, 121, 956},
                    {805, 732, 524, 37, 331}
                };
                RunTarget(expMatrix);
                
            }

            [Fact]
            public void EmptyCell()
            {
                var expMatrix = new int[4, 4];
                Assert.Throws<FormatException>(() => RunTarget(expMatrix));
            }
            
            
            
        }

        public class PathSumFixture
        {
            public PathSum PathSum;

            public PathSumFixture()
            {
                PathSum = new PathSum(new[,]
                {
                    {0, 1, 4, 8},
                    {16, 32, 64, 128},
                    {256, 512, 1024, 2048}
                });
            }
        }
    }
}