//
// Figure.cs
//
// Author:
//       Paweł "X4lldux" Drygas <x4lldux@jabster.pl>
//
// Copyright (c) 2009 Paweł "X4lldux" Drygas
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

namespace Kretu
{
	public class Figure
	{
		static int[,,] figureShape = {
			{{0,0}, {0,1}, {0,2}, {0,3}},
			{{0,0}, {1,0}, {2,0}, {3,0}}, // ----
			{{0,0}, {0,1}, {1,1}, {1,0}}, // box
			{{0,0}, {0,1}, {1,1}, {0,2}}, // faket
			{{0,0}, {1,0}, {1,1}, {2,0}}, // faket
			{{1,0}, {1,1}, {0,1}, {1,2}}, // faket
			{{0,1}, {1,1}, {1,0}, {2,1}}, // faket
			{{0,0}, {0,1}, {1,1}, {2,1}}, // L
			{{0,0}, {1,0}, {0,1}, {0,2}}, // L
			{{0,0}, {1,0}, {2,0}, {2,1}}, // L
			{{1,0}, {1,1}, {1,2}, {0,2}}, // L
			{{0,0}, {0,1}, {1,0}, {2,0}}, // L
			{{0,0}, {1,0}, {1,1}, {1,2}}, // L
			{{0,1}, {1,1}, {2,1}, {2,0}}, // L
			{{0,0}, {0,1}, {0,2}, {1,2}}, // L
			{{0,0}, {1,0}, {1,1}, {2,1}}, // Z
			{{1,0}, {1,1}, {0,1}, {0,2}}, // Z
			{{0,0}, {0,1}, {1,1}, {1,2}}, // Z
			{{1,0}, {2,0}, {0,1}, {1,1}}, // Z
		};
		static public int[,,] FigureShape {
			get { return figureShape; }
		}
		static Random rnd = new Random ();
		Block[] blocks;

		public Block[] Blocks {
			get { return this.blocks; }
		}

		public bool Moved {get; set;}

		private Figure () {
			blocks = new Block[4];

			for (int i = 0; i < blocks.Length; i++) {
				blocks[i] = new Block (this);
			}
		}

		public static Figure Generate () {
			var f = new Figure ();

			var shapeNum = rnd.Next (figureShape.GetLength (0));
			for (int i = 0; i < f.Blocks.Length; i++) {
				f.Blocks[i].X = figureShape[shapeNum, i, 0];
				f.Blocks[i].Y = figureShape[shapeNum, i, 1];
			}

			return f;
		}
	}
}
