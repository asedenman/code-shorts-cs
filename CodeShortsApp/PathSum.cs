/* ----------------------------------------------------------------------------------------------------------------
 ---------------------------- CodeShorts ----------------------------
 ---------------------------- CodeShortsApp ----------------------------
 ----------------------------------------------------------------------------------------------------------------
 
 File: PathSum.cs
 Created: 07/11/2021
 By: ASED
 Euler Problem 82  
The minimal path sum in the 5 by 5 matrix below, by starting in any cell in the left column and finishing in any cell in
the right column, and only moving up, down, and right, is indicated in red and bold; the sum is equal to 994.

131     673   {234} {103}  {18}
{201}   {96}  {342} 965   150
630     803   746   422   111
537     699   497   121   956
805     732   524   37    331

Find the minimal path sum from the left column to the right column in matrix.txt, a 31K text file containing an 80 by 80 matrix.

 ---------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CodeShortsApp
{
    /// <summary>
    /// Problem solver entry class
    /// </summary>
    public class PathSum : IProblemSolver
    {
        // Uniform cost graph search
        public int numRows;
        public int numCols;
        private int[,] _matrix;
        private const string ProblemDataFile = "EulerProblemData/p082_matrix.txt";

        public PathSum()
        {
        }

        public PathSum(int[,] matrix)
        {
            Matrix = matrix;
            PriorityQueue = new PriorityQueue<Node, int>();
        }

        public int[,] Matrix
        {
            get => _matrix;
            set
            {
                _matrix = value;
                numRows = _matrix?.GetLength(0) ?? 0;
                numCols = _matrix?.GetLength(1) ?? 0;

            }
        }

        public int[,] BestValues;

        public string Solve()
        {
            var visited = new bool[numRows, numCols];
            var queue = new PriorityQueue<(int, int), int>();
            for (int i = 0; i < numRows; i++)
            {
                queue.Enqueue((i, 0), Matrix[i, 0]);
            }

            int result = 0;
            while (queue.TryDequeue(out var item, out int val))
            {
                if (numCols - 1 == item.Item2)
                {
                    result = val;
                    break;
                }

                for (int i = 0; i < 3; i++)
                {
                    var x1 = item.Item1 + DirX[i];
                    var y1 = item.Item2 + DirY[i];
                    if (GetValueAtOrDefault(out int newVal, x1, y1))
                    {
                        if (!visited[x1, y1])
                        {
                            visited[x1, y1] = true;
                            queue.Enqueue((x1, y1), val + newVal);
                        }
                    }
                }

            }

            return result.ToString();
        }


        public bool CheckNodeIsGoal(Node node)
        {
            return node.Position[1] == numCols - 1;
        }




        public void AddFirstColToQueue()
        {
            for (int i = 0; i < numRows; i++)
            {
                GetValueAtOrDefault(out int val, i, 0);
                var node = new Node() {Position = new[] {i, 0}, History = new List<Direction>(), Value = val};
                PriorityQueue.Enqueue(node, val);
            }
        }

        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULL = (char) 0;
        private const char COMMA = ',';

        public (int, int) LineCountAndLineLength(StreamReader sr)
        {
            var lineCount = 0;
            var lineLength = 0;
            var maxLineLength = 0;
            var byteBuffer = new char[1024 * 1024];
            var detectedEOL = NULL;
            var currentChar = NULL;
            int bytesRead;
            while ((bytesRead = sr.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    currentChar = byteBuffer[i];
                    if (detectedEOL != NULL)
                    {
                        if (currentChar == detectedEOL)
                        {
                            lineCount++;
                            lineLength++;
                            if (lineLength != maxLineLength)
                            {
                                throw new InvalidDataException(
                                    $"Invalid input .csv file: row lengths not homogenous.");
                            }

                            lineLength = 0;
                        }
                        else if (currentChar == COMMA)
                        {
                            lineLength++;
                        }
                    }
                    else if (currentChar == LF || currentChar == CR)
                    {
                        detectedEOL = currentChar;
                        lineCount++;
                        lineLength++;
                        maxLineLength = lineLength;
                        lineLength = 0;

                    }
                    else if (currentChar == COMMA)
                    {
                        lineLength++;
                    }
                }
            }

            if (currentChar != LF && currentChar != CR && currentChar != NULL)
            {
                lineCount++;
                lineLength++;
                if (maxLineLength == 0)
                {
                    maxLineLength = lineLength;
                }
                else if (lineLength != maxLineLength)
                {
                    throw new InvalidDataException(
                        $"Invalid input .csv file: row lengths not homogenous.");
                }
            }

            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            return (lineCount, maxLineLength);
        }

        public void LoadData()
        {
            using (StreamReader sr = new StreamReader(ProblemDataFile))
            {
                (int lineCount, int lineLength) = LineCountAndLineLength(sr);
                Matrix = new int[lineCount, lineLength];
                ReadToMatrix(Matrix, sr);
            }

            PriorityQueue = new PriorityQueue<Node, int>();
        }

        public void ReadToMatrix(int[,] matrix, StreamReader sr)
        {
            if (matrix is null)
            {
                throw new NullReferenceException("Matrix parameter can not be null.");
            }

            int row = 0;
            int col = 0;
            while (sr.ReadLine() is string line)
            {

                foreach (var numStr in line.Split(','))
                {
                    try
                    {
                        matrix[row, col] = int.Parse(numStr.Trim());
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine($"Invalid String was: {numStr}");
                        throw;
                    }

                    col++;
                }

                if (matrix.GetLength(1) != col)
                {
                    throw new ArgumentException("Matrix parameter was incorrect size for input.");
                }

                col = 0;
                row++;
            }

            if (matrix.GetLength(0) != row)
            {
                throw new ArgumentException("Matrix parameter was incorrect size for input.");
            }


        }


        // Try using MS Priority queue
        public PriorityQueue<Node, int> PriorityQueue;

        // Assumes a square matrix
        public bool IsValidIndex(int i, int j)
        {
            return !(i < 0 || j < 0 || i >= numRows || j >= numCols);
        }

        public bool GetValueAtOrDefault(out int val, int i, int j)
        {
            var result = IsValidIndex(i, j);
            val = result ? Matrix[i, j] : 0;
            return result;
        }

        public int[] DirY = new[] {0, 1, 0};
        public int[] DirX = new[] {-1, 0, 1};


        public void AddNeighboursToQueue(Node node)
        {
            int x = node.Position[0];
            int y = node.Position[1];
            int val = node.Value;
            int newX = 0;
            int newY = 0;
            for (int i = 0; i < 3; i++)
            {
                var x_1 = x + DirX[i];
                var y_1 = y + DirY[i];
                if (GetValueAtOrDefault(out int newVal, x_1, y_1))
                {
                    // prune
                    if (val + newVal < BestValues[x_1, y_1])
                    {
                        BestValues[x_1, y_1] = val + newVal;
                        PriorityQueue.Enqueue(CreateNext(node, (Direction) i, newVal), val + newVal);
                    }
                }
            }
        }

        public Node CreateNext(Node orgNode, Direction newDir, int addedValue)
        {
            var newNode = new Node()
            {
                //Value = addedValue + this.Value, 
                History = new List<Direction>(orgNode.History) {newDir},
                Position = new int[] {orgNode.Position[0], orgNode.Position[1]}
            };
            AddDir(newNode.Position, newDir);
            newNode.Value = orgNode.Value + addedValue;
            return newNode;
        }

        public void AddDir(int[] pos, Direction dir)
        {
            switch (dir)
            {
                case Direction.N:
                    pos[0] -= 1;
                    break;
                case Direction.E:
                    pos[1] += 1;
                    break;
                case Direction.S:
                    pos[0] += 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(dir), dir, $"{nameof(AddDir)}: Not expected input {dir}");
            }
        }


    }

    public enum Direction
    {
        N = 0,
        E = 1,
        S = 2 /*, W = 3*/
    };
    
    public class Node
    {
        //public int Value;
        public List<Direction> History;
        public int[] Position;
        public int Value;


    }
}