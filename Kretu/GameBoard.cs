//
// GameLabel.cs
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
using System.Text;
using Gtk;
using System.Collections.Generic;

namespace Kretu
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class GameBoard : Gtk.EventBox, IGameInterface
	{
		private class FigureColorNSymbol
		{
			public string Color { get; set; }
			public char Symbol { get; set; }
		}

		public static readonly Gdk.Color[] Colors = {
			TangoColors.Butter2,
			TangoColors.Chameleon1,
			TangoColors.Chocolate2,
			TangoColors.Orange2,
			TangoColors.Plum2,
			TangoColors.ScarletRed2,
			TangoColors.SkyBlue1,
			TangoColors.Aluminium4,

			TangoColors.Butter3,
			TangoColors.Chameleon3,
			TangoColors.Chocolate3,
			TangoColors.Orange3,
			TangoColors.Plum3,
			TangoColors.ScarletRed3,
			TangoColors.SkyBlue3,
			TangoColors.Aluminium6
			
		};
//		string[] symbols = {
//			"#",
//			"&amp;",
//			"@",
//			"$",
//			"%",
//			"X",
//			"B",
//			"8"
//		};
		string symbols = "█░▒▓";

		Random rnd = new Random ();
		Dictionary<Figure, FigureColorNSymbol> figColsSymbols = new Dictionary<Figure, FigureColorNSymbol> ();
		Label lbl = new Label ();

		public GameBoard () : base() {
			lbl.Show ();
			Add (lbl);

			CanDefault = true;
			CanFocus = true;
			Events = Gdk.EventMask.KeyPressMask;

			KeyPressEvent += HandleKeyPressEvent;
		}

		public void DrawBoard (List<Figure> figures, Block[,] board) {
			

			foreach (var f in figures) {
				if (!figColsSymbols.ContainsKey (f))
					figColsSymbols.Add (f, new FigureColorNSymbol {
						Color = Colors[rnd.Next (Colors.Length)].ToWebString (),
						Symbol = symbols[rnd.Next (symbols.Length)]
					});
			}

			var sb = new StringBuilder ("<span weight=\"bold\" size=\"xx-large\" font=\"Monospace\" bgcolor=\"");
			sb.Append (TangoColors.Aluminium3.ToWebString ());
			sb.Append ("\" fgcolor=\"");
			sb.Append (TangoColors.Aluminium3.ToWebString ());
			sb.Append ("\">");

			for (int y = 5; y < board.GetLength (1); y++) {
				for (int x = 0; x < board.GetLength (0); x++) {
					if (board[x, y] == null) {
						sb.Append (" ");
					} else if (board[x, y] == Block.Kretu) {
						sb.Append ("<span color=\"black\" weight=\"bold\">⍨</span>");
					} else {
						var idx = figures.IndexOf (board[x, y].Figure);
						sb.Append ("<span color=\"");
						sb.Append (figColsSymbols[board[x, y].Figure].Color);
						sb.Append ("\">");
						sb.Append (figColsSymbols[board[x, y].Figure].Symbol);
						sb.Append ("</span>");
					}
				}

				sb.AppendLine ();
			}
			sb.Append ("</span>");

			lbl.Markup = sb.ToString ();


		}

		void HandleKeyPressEvent (object o, KeyPressEventArgs args) {
			if (KeyPressed != null) {
				var e = new KeyPressedEventArgs ();

				switch (args.Event.Key) {
					case Gdk.Key.Escape:
						e.Key = KeyInfo.Escape;
						break;

					case Gdk.Key.Left:
						e.Key = KeyInfo.Left;
						break;

					case Gdk.Key.Right:
						e.Key = KeyInfo.Right;
						break;

					case Gdk.Key.Up:
						e.Key = KeyInfo.Up;
						break;

					case Gdk.Key.Return:
					case Gdk.Key.KP_Enter:
						e.Key = KeyInfo.Enter;
						break;

				default:
						e.Key = KeyInfo.Other;
						break;
				}

				KeyPressed (this, e);
			}

			args.RetVal = true;
		}

		public event EventHandler<KeyPressedEventArgs> KeyPressed;

	}

	public static class TangoColors
	{
		public static string ToWebString (this Gdk.Color col) {
			return string.Format ("#{0:x2}{1:x2}{2:x2}", col.Red * byte.MaxValue / ushort.MaxValue, col.Green * byte.MaxValue / ushort.MaxValue, col.Blue * byte.MaxValue / ushort.MaxValue);
		}


		public static readonly Gdk.Color Butter1 = new Gdk.Color (252, 233, 79);
		public static readonly Gdk.Color Butter2 = new Gdk.Color (237, 212, 0);
		public static readonly Gdk.Color Butter3 = new Gdk.Color (196, 160, 0);
		public static readonly Gdk.Color Chameleon1 = new Gdk.Color (138, 226, 52);
		public static readonly Gdk.Color Chameleon2 = new Gdk.Color (115, 210, 22);
		public static readonly Gdk.Color Chameleon3 = new Gdk.Color (78, 154, 6);
		public static readonly Gdk.Color Orange1 = new Gdk.Color (252, 175, 62);
		public static readonly Gdk.Color Orange2 = new Gdk.Color (245, 121, 0);
		public static readonly Gdk.Color Orange3 = new Gdk.Color (206, 92, 0);
		public static readonly Gdk.Color SkyBlue1 = new Gdk.Color (114, 159, 207);
		public static readonly Gdk.Color SkyBlue2 = new Gdk.Color (52, 101, 164);
		public static readonly Gdk.Color SkyBlue3 = new Gdk.Color (32, 74, 135);
		public static readonly Gdk.Color Plum1 = new Gdk.Color (173, 127, 168);
		public static readonly Gdk.Color Plum2 = new Gdk.Color (117, 80, 123);
		public static readonly Gdk.Color Plum3 = new Gdk.Color (92, 53, 102);
		public static readonly Gdk.Color Chocolate1 = new Gdk.Color (233, 185, 110);
		public static readonly Gdk.Color Chocolate2 = new Gdk.Color (193, 125, 17);
		public static readonly Gdk.Color Chocolate3 = new Gdk.Color (143, 89, 2);
		public static readonly Gdk.Color ScarletRed1 = new Gdk.Color (239, 41, 41);
		public static readonly Gdk.Color ScarletRed2 = new Gdk.Color (204, 0, 0);
		public static readonly Gdk.Color ScarletRed3 = new Gdk.Color (164, 0, 0);
		public static readonly Gdk.Color Aluminium1 = new Gdk.Color (238, 238, 236);
		public static readonly Gdk.Color Aluminium2 = new Gdk.Color (211, 215, 207);
		public static readonly Gdk.Color Aluminium3 = new Gdk.Color (186, 189, 182);
		public static readonly Gdk.Color Aluminium4 = new Gdk.Color (136, 138, 133);
		public static readonly Gdk.Color Aluminium5 = new Gdk.Color (85, 87, 83);
		public static readonly Gdk.Color Aluminium6 = new Gdk.Color (46, 52, 54);
		
	}
}
