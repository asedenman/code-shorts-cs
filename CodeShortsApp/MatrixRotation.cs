// ----------------------------------------------------------------------------------------------------------------
// ---------------------------- CodeShorts ----------------------------
// ---------------------------- CodeShortsApp ----------------------------
// ----------------------------------------------------------------------------------------------------------------
// 
// File: public class MatrixRotation.cs
// Created: 12/11/2021
// By: ASED
// Matrix Layer Rotation
// ----------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace CodeShortsApp
{
    public class MatrixRotation
    {
        /// <summary>
        /// Rotates the matrix discretly by layers
        ///     1  2  3  4  5
        ///     6  7  8  9  10
        ///     11 12 13 14 15
        ///     16 17 18 19 20
        /// becomes
        ///     2  3  4  5  10
        ///     1  8  9  14 15
        ///     6  7  12 13 20
        ///     11 16 17 18 19
        /// </summary>
        /// <param name="matrix"> 2D array of integers </param>
        /// <param name="r"> rotation factor </param>
        public static void matrixRotation(List<List<int>> matrix, int r)
        {
            // minimum side
            int k = matrix.Count > matrix[0].Count ? matrix[0].Count : matrix.Count;

            // calculate number of layers
            int numLayers = k / 2;
            
            // create list of layers
            List<Layer> layers = new List<Layer>(numLayers);
            // for n < numLayers: new layer(n, n, matrix.Count - 1 - n, matrix[0].Count - 1 - n
            for (int i = 0; i < numLayers; i++)
            {
                layers.Add(new Layer(i, i, matrix.Count -1 -(2* i), matrix[0].Count -1 -(2*i)));
            }
            // NOTE: each layer starts at (i,j) where i == j
            // NOTE: each layer ends at (countI - 1 - i, countJ - 1 - j)
            int[,] result = new int[matrix.Count, matrix[0].Count];

            foreach (var layer in layers)
            {
                CopyRotateLayer(matrix, result, r, layer);
            }

            for (int i = 0; i < matrix.Count; i++){
                for (int j = 0; j < matrix[0].Count; j++){
                    Console.Write($"{result[i,j].ToString()} ");
                    
                }
                Console.WriteLine();
            } 
        }

        public static void CopyRotateLayer(List<List<int>> source, int[,] dest, int r, Layer layer)
        {
            
            int dstI = layer.startI;
            int dstJ = layer.startJ;
            (int srcI, int srcJ) = layer.ReverseRotate(r);

            int dstSide = 0;
            int srcSide;
            if (srcI != dstI && srcJ == dstJ)
            {
                // left side but
                // not start corner (srcI == dstI && srcJ == dstJ)
                srcSide = 3;
            }
            else if (srcI == dstI + layer.lengthI)
            {
                // bottom but not BL corner (caught by above)
                srcSide = 2;
            }
            else if (srcJ == dstJ + layer.lengthJ)
            {
                // right side but not BR corner 
                srcSide = 1;
            }
            else if (srcI == dstI)
            {
                // top side inc start corner
                srcSide = 0;
            }
            else
            {
                throw new ArgumentException("src i,j not in layer!");
            }
            while (true)
            {
                // copy value from source[srcI, srcJ] to dest[dstI, dstJ]
                dest[dstI,dstJ] = source[srcI][srcJ];

                // add (VecI[side], VecJ[side]) to both (where side == srcSide/dstSide
                dstI += Layer.VecI[dstSide];
                dstJ += Layer.VecJ[dstSide];
                srcI += Layer.VecI[srcSide];
                srcJ += Layer.VecJ[srcSide];
                
                // test if src or dst at corner
                // if dst at corner check if corner is start, if yes then end
                // if at corner then increment side
                if (layer.CornerCheck(dstI, dstJ))
                { if (dstSide == 3)
                    { break; 
                    }
                    dstSide++;
                }

                if (layer.CornerCheck(srcI, srcJ))
                {
                    srcSide++;
                    srcSide %= 4;
                }
                //continue
            }
            
        }
    }

    public class Layer
    {
        public int startI;
        public int startJ;
        public int lengthI;
        public int lengthJ;

        public static readonly int[] VecI = {0, 1, 0, -1};
        public static readonly int[] VecJ = {1, 0, -1, 0};

        // test if at corner
        public int[] TestIs;
        public int[] TestJs;

        public Layer(int startI, int startJ, int lengthI, int lengthJ)
        {
            this.startI = startI;
            this.startJ = startJ;
            this.lengthI = lengthI;
            this.lengthJ = lengthJ;
            TestIs = new[] {-1, this.startI + lengthI - 1, -1, startI};
            TestJs = new[] {startJ + lengthJ, -1, startJ, -1};

        }

        public bool CornerCheck(int i, int j)
        {
            return (i == startI || i == startI + lengthI) && (j == startJ || j == startJ + lengthJ);
        }

        public (int, int) ReverseRotate(int r)
        {
            // note we are rotating backwards
            // therefore clockwise
                
            // if not already done then take:    r = r mod (lengthI * 2 + lengthJ * 2)
            // to remove cycles
            r = r % (lengthI * 2 + lengthJ * 2);
                
            // start at (startI, startJ)
            // move along each side subtracting side length from r
            // then testing if r - side length < 0 (MAYBE <=)
            // if true then (side length - r) = pos on side
            // calculate other coord using startCoord and length
            if (r - lengthJ <= 0)
            {
                return (startI, startJ + r);
            }

            if (r - (lengthJ + lengthI) <= 0)
            {
                return (startI + (r - lengthJ), lengthJ + startJ);
            }

            if (r - (lengthJ * 2 + lengthI) <= 0)
            {
                return (startI + lengthI, startJ + (lengthJ - (r - (lengthI + lengthJ))));
            }

            if (r - (lengthJ * 2 + lengthI * 2) <=0)
            {
                return (startI + (lengthI - (r - (lengthJ * 2 + lengthI))), startJ);
            }

            throw new ArithmeticException(
                $"{nameof(ReverseRotate)}: r was greater than one rotation -> error in mod function?");
        }
    }
}