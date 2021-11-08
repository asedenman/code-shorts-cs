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

namespace CodeShortsApp
{
    /// <summary>
    /// Problem solver entry class
    /// </summary>
    public class PathSum
    {
        // A* search: each cell already has value, moving right is heuristic

        public int[,] Matrix;

        public SortedDictionary<Node, int> Queue;
        public bool TryGetValueAt(out int val, int i, int j)
        {
            if (i < 0 || j < 0 || i >= Matrix.GetLength(0) || j >= Matrix.GetLength(1))
            {
                val = 0;
                return false;
            }

            val = Matrix[i, j];
            return true;
        }
        
        public void AddNeighboursToQueue(Node node, int value)
        {
            var curPos = node.Position;
            for (int i = 0; i < 3; i++)
            {
                // needs to test valid positions in each direction
                // then creates nodes for each position
                // then gets sum of value from that position and  current value
                // adds
            }
        }
        
        
        
        public Node CreateNext(Node orgNode, Direction newDir /*, int addedValue*/)
        {
            var newNode = new Node()
            {
                //Value = addedValue + this.Value, 
                History = new List<Direction>(orgNode.History) {newDir},
                Position = new int[] {orgNode.Position[0], orgNode.Position[1]}
            };
            AddDir(newNode.Position, newDir);
            return newNode;
        }
        
        public void AddDir(int[] pos, Direction dir)
        {
            switch (dir)
            {
                case Direction.N:
                    pos[1] += 1;
                    break;
                case Direction.E:
                    pos[0] += 1;
                    break;
                case Direction.S:
                    pos[1] -= 1;
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
    
    public static class DirectionVectors{
        public static void AddDir(int[] pos, Direction dir)
        {
            switch (dir)
            {
                case Direction.N:
                    pos[1] += 1;
                    break;
                case Direction.E:
                    pos[0] += 1;
                    break;
                case Direction.S:
                    pos[1] -= 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(dir), dir, $"{nameof(AddDir)}: Not expected input {dir}");
            }
        }
    }
    
    public class Node
    {
        //public int Value;
        public List<Direction> History;
        public int[] Position;
        
        
    }
}