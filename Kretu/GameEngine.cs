//
// GameEngine.cs
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
using System.Collections.Generic;

namespace Kretu
{
	public class DrawEventArgs : EventArgs
	{
		public Block[,] Blocks { get; private set; }

		List<Figure> figures = new List<Figure> ();
		public List<Figure> Figures {
			get { return this.figures; }
		}


		public DrawEventArgs (List<Figure> figures, Block[,] blocks) {
			this.figures = figures;
			this.Blocks = blocks;
		}

	}

	public class GameEngine
	{
		int boardWidth = 40, boardHeight = 40;
		public int BoardWidth {
			get { return this.boardWidth; }
		}

		public int BoardHeight {
			get { return this.boardHeight; }
		}

		int bufforHeight = 4;
		Block[,] board;
		Block[,] lastBoard;
		List<Figure> figures = new List<Figure> ();
		Random rnd = new Random();
		bool kretuMoved;
		bool kretuSquashed;
		bool firstRun;
		bool firstFull;
		bool eaten;
		int kretuMoveDownTicksCounter = 0;
		int resetCounter = 0;
		uint gameToclTimeoutID;

		public int Points { get; set; }
		public bool GameLost { get; set; }


		public GameEngine () {
			Reset ();

			gameToclTimeoutID = GLib.Timeout.Add (800, GameTick);
		}

		public void Reset () {
			kretuMoved = false;
			kretuSquashed = false;
			firstRun = true;
			firstFull = true;
			eaten = false;
			kretuMoveDownTicksCounter = 0;
			resetCounter = 0;
			GameLost = false;
			Points = 0;

			figures.Clear ();
			board = new Block[boardWidth, boardHeight];
			for (int i = 0; i < 50 && AddNewFigure (); i++) {
			}
			do {
				for (int i = 0; i < 20 && AddNewFigure (); i++) {
				}
			} while (MoveFigures () != 0);
		}

		public void Undo () {
			var tmp = (Block[,]) board.Clone ();
			board = lastBoard;
			lastBoard = tmp;
			
			GameTick ();
		}

		bool GameTick () {
			for (int i = 0; i < 20 && AddNewFigure (); i++) {
			}

			if (!firstRun) {
				lastBoard = (Block[,]) board.Clone ();

				if (MoveFigures () == 0 && firstFull) {
					List<int> freePos = new List<int> ();
					for (int i = 0; i < BoardWidth; i++) {
						if (board[i, BoardHeight - 1] == null)
							freePos.Add (i);
					}

					int x = rnd.Next (freePos.Count);
					Block.Kretu.X = freePos[x];
					Block.Kretu.Y = BoardHeight - 1;
					board[freePos[x], BoardHeight - 1] = Block.Kretu;

					firstFull = false;
				}

			}
			if (kretuSquashed) {
				GameLost = true;
				resetCounter++;

				if (resetCounter == 7)
					Reset ();
			} else {
				if (kretuMoved)
					kretuMoveDownTicksCounter = 0;
				if (!firstFull && Block.Kretu.Y != BoardHeight - 1) {
					if (board[Block.Kretu.X, Block.Kretu.Y + 1] == null)
						kretuMoveDownTicksCounter++;

				if (kretuMoveDownTicksCounter == 7) {
						kretuMoveDownTicksCounter = 0;

					board[Block.Kretu.X, Block.Kretu.Y] = null;
						Block.Kretu.Y++;
						board[Block.Kretu.X, Block.Kretu.Y] = Block.Kretu;
					}
				}
			}

			if (Draw != null)
				Draw (this, new DrawEventArgs (figures, board));

			eaten = false;
			firstRun = false;
			kretuMoved = false;
			return true;
		}

		int MoveFigures () {
			int movesNum = 0;
			foreach (var f in figures) {
				f.Moved = false;
			}
			for (int y = BoardHeight - 1; y >= 0; y--) {
				for (int x = 0; x < BoardWidth; x++) {
					if (board[x, y] != null && board[x, y] != Block.Kretu) {
						var f = board[x, y].Figure;
						if (f.Moved)
							continue;

						bool kretuCanBeSquashed = false;
						foreach (var b in f.Blocks) {
							if (b.Y + 1 == BoardHeight) {
								f.Moved = true;
								break;
							} else if (board[b.X, b.Y + 1] != null) {
								if (board[b.X, b.Y + 1] == Block.Kretu) {
									kretuCanBeSquashed = true;
								} else if (board[b.X, b.Y + 1].Figure != f) {
									f.Moved = true;
									break;
								}
							}
						}
						if (f.Moved)
							continue;

						if (kretuCanBeSquashed)
							kretuSquashed = true;

						foreach (var b in f.Blocks) {
							board[b.X, b.Y] = null;
						}
						foreach (var b in f.Blocks) {
							b.Y++;
							board[b.X, b.Y] = b;
						}
						f.Moved = true;
						y = BoardHeight - 1;
						x = 0;
						movesNum++;
					}
				}
			}
			
			return movesNum;
		}

		bool AddNewFigure () {
			Figure f = Figure.Generate ();
			return AddFigure (f);
		}

		bool AddFigure (Figure fig) {
			var rnd = new Random ();
			
			bool isFigurePlaced;
			int x, y, tries = 0;
			do {
				tries++;
				x = rnd.Next (boardWidth);
				y = rnd.Next (bufforHeight + 1);

				isFigurePlaced = true;
				foreach (var b in fig.Blocks) {
					var X = x + b.X;
					var Y = y + b.Y;

					if (X >= 0 && X < BoardWidth && Y >= 0 && Y <= bufforHeight) {
						isFigurePlaced &= (board[X, Y] == null);
						
					} else {
						isFigurePlaced = false;
						break;
					}
				}
			} while (!isFigurePlaced && tries < 100);
			
			if (!isFigurePlaced) {
				return false;
			}
			
			if (isFigurePlaced) {
				foreach (var b in fig.Blocks) {
					b.X += x;
					b.Y += y;
					board[b.X, b.Y] = b;
				}
				
				figures.Add (fig);
				return true;
			}
			
			return false;
		}

		public void KeyPress (KeyInfo key) {
			if (firstFull == false && kretuSquashed == false) {
				switch (key) {
					case KeyInfo.Left:
						if (Block.Kretu.X == 0)
							break;

						board[Block.Kretu.X, Block.Kretu.Y] = null;
						Block.Kretu.X--;
						
						kretuMoved = true;
						break;

				case KeyInfo.Right:
						if (Block.Kretu.X == BoardWidth - 1)
							break;

						board[Block.Kretu.X, Block.Kretu.Y] = null;
						Block.Kretu.X++;

						kretuMoved = true;
						break;

				case KeyInfo.Up:
						if (Block.Kretu.Y == 0)
							break;

						board[Block.Kretu.X, Block.Kretu.Y] = null;
						Block.Kretu.Y--;

						kretuMoved = true;
						break;

				case KeyInfo.Escape:
					case KeyInfo.Enter:
						if (GameLost == true) {
							Reset ();
							return;
						}
						break;
					default:
						break;
				}

				if (board[Block.Kretu.X, Block.Kretu.Y] != null && board[Block.Kretu.X, Block.Kretu.Y] != Block.Kretu) {
					var fig = board[Block.Kretu.X, Block.Kretu.Y].Figure;
					for (int i = 0; i < fig.Blocks.Length; i++) {
						board[fig.Blocks[i].X, fig.Blocks[i].Y] = null;
						fig.Blocks[i] = null;
					}
					eaten = true;
					figures.Remove (fig);

					Points++;
				}
				board[Block.Kretu.X, Block.Kretu.Y] = Block.Kretu;
			}

//			if (Draw != null)
			//				Draw (this, new DrawEventArgs (figures, board));
			GLib.Source.Remove (gameToclTimeoutID);
			gameToclTimeoutID = GLib.Timeout.Add (800, GameTick);
			GameTick ();
		}

		public event EventHandler<DrawEventArgs> Draw;
	}
}
