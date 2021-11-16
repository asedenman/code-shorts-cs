// ----------------------------------------------------------------------------------------------------------------
// ---------------------------- CodeShorts ----------------------------
// ---------------------------- CodeShortsApp.Test ----------------------------
// ----------------------------------------------------------------------------------------------------------------
// 
// File: MatrixRotationTests.cs
// Created: 16/11/2021
// By: ASED
// 
// ----------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Metadata;
using Xunit;

namespace CodeShortsApp
{
    public class MatrixRotationTests
    {
        public class ReverseRotate
        {
            public Layer Layer;

            public ReverseRotate()
            {
            }
            
            [Fact]
            public void LengthOne()
            {
                Layer = new Layer(0, 0, 1, 1);
                Assert.Equal((0,1), Layer.ReverseRotate(1));
            }

            [Theory]
            [InlineData(2, 0, 2)]
            [InlineData(5, 1, 4)]
            [InlineData(10, 4, 2)]
            [InlineData(13, 3, 0)]
            public void EachSide(int r, int expI, int expJ)
            {
                Layer = new Layer(0, 0, 4, 4);
                Assert.Equal((expI, expJ), Layer.ReverseRotate(r));
            }
            
            [Theory]
            [InlineData(4, 3, 7)]
            [InlineData(10, 4, 12)]
            [InlineData(21, 12, 9)]
            [InlineData(30, 9, 3)]
            public void Offset(int r, int expI, int expJ)
            {
                Layer = new Layer(3, 3, 9, 9);
                Assert.Equal((expI, expJ), Layer.ReverseRotate(r));
            }
            
            [Theory]
            [InlineData(4, 4, 8)]
            [InlineData(14, 9, 13)]
            [InlineData(16, 10, 12)]
            [InlineData(25, 9, 4)]
            public void OffsetUnevenSides(int r, int expI, int expJ)
            {
                Layer = new Layer(4, 4, 6, 9);
                Assert.Equal((expI, expJ), Layer.ReverseRotate(r));
            }
            
            [Theory]
            [InlineData(0, 4, 4)]
            [InlineData(9, 4, 13)]
            [InlineData(15, 10, 13)]
            [InlineData(24, 10, 4)]
            public void OffsetUnevenSidesCorners(int r, int expI, int expJ)
            {
                Layer = new Layer(4, 4, 6, 9);
                Assert.Equal((expI, expJ), Layer.ReverseRotate(r));
            }
            
            [Theory]
            [InlineData(30, 4, 4)]
            [InlineData(44, 9, 13)]
            [InlineData(46, 10, 12)]
            [InlineData(55, 9, 4)]
            public void OffsetUnevenSidesOverflow(int r, int expI, int expJ)
            {
                Layer = new Layer(4, 4, 6, 9);
                Assert.Equal((expI, expJ), Layer.ReverseRotate(r));
            }
            
            
            
        }

        public class CopyRotateLayer
        {
            public List<List<int>> src;
            public int[,] dst;
            
            public void SetupDstSrc(int lengthI, int lengthJ)
            {
                var list = new List<List<int>>(lengthI);
                var count = 0;
                dst = new int[lengthI, lengthJ];
                for (int i = 0; i < lengthI; i++)
                {
                    list.Add(new List<int>(lengthJ));
                    for (int j = 0; j < lengthJ; j++)
                    {
                        list[i].Add(count);
                        dst[i, j] = count;
                        count++;
                    }
                }

                src = list;

                
                

            }
            
            
            [Fact]
            public void EvenInnerLayer()
            {
                Layer layer = new Layer(1, 1, 1, 1);
                SetupDstSrc(4, 4);
                MatrixRotation matRot = new MatrixRotation();
                var exp = new int[,]
                {
                    {0, 1, 2, 3},
                    {4, 6, 10, 7},
                    {8, 5, 9, 11},
                    {12, 13, 14, 15}
                };
                MatrixRotation.CopyRotateLayer(src, dst, 1, layer);
                Assert.Equal(exp, dst);
            }
            
            [Fact]
            public void EvenOuterLayer()
            {
                // var exp = new int[,]
                // {
                //     {0, 1, 2, 3},
                //     {4, 5, 6, 7},
                //     {8, 9, 10, 11},
                //     {12, 13, 14, 15}
                // };
                var lengthI = 4;
                var lengthJ = 4;
                Layer layer = new Layer(0, 0, lengthI -1, lengthJ - 1);
                SetupDstSrc(lengthI, lengthJ);
                MatrixRotation matRot = new MatrixRotation();
                var exp = new int[,]
                {
                    {1, 2, 3, 7},
                    {0, 5, 6, 11},
                    {4, 9, 10, 15},
                    {8, 12, 13, 14}
                };
                MatrixRotation.CopyRotateLayer(src, dst, 1, layer);
                Assert.Equal(exp, dst);
            }

            [Fact]
            public void OddInnerLayer()
            {
                /* Initial:
                 * 0  1  2  3  4
                 * 5  6  7  8  9
                 * 10 11 12 13 14
                 * 15 16 17 18 19
                 * Exp:
                 * 0  1  2  3  4
                 * 5  13 12 11  9
                 * 10  8  7  6 14
                 * 15 16 17 18 19
                 */
                Layer layer = new Layer(1, 1, 1, 2);
                SetupDstSrc(4, 5);
                MatrixRotation matRot = new MatrixRotation();
                var exp = new int[,]
                {
                    {0, 1, 2, 3, 4},
                    {5, 13, 12, 11, 9},
                    {10, 8, 7, 6, 14},
                    {15, 16, 17, 18, 19}
                };
                MatrixRotation.CopyRotateLayer(src, dst, 3, layer);
                Assert.Equal(exp, dst);
            }
            
            [Fact]
            public void OddOuterLayer()
            {
                /* Initial:
                 * 0  1  2  3  4
                 * 5  6  7  8  9
                 * 10 11 12 13 14
                 * 15 16 17 18 19
                 * Exp:
                 * 15 10 5  0  1
                 * 16 6  7  8  2
                 * 17 11 12 13 3
                 * 18 19 14 9 4
                 */
                Layer layer = new Layer(0, 0, 3, 4);
                SetupDstSrc(4, 5);
                MatrixRotation matRot = new MatrixRotation();
                var exp = new int[,]
                {
                    {15, 10, 5, 0, 1},
                    {16, 6, 7, 8, 2},
                    {17, 11, 12, 13, 3},
                    {18, 19, 14, 9, 4}
                };
                MatrixRotation.CopyRotateLayer(src, dst, 11, layer);
                Assert.Equal(exp, dst);
            }
            
        }
    }
}