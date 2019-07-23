using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MineServer.MCFormat
{
    public sealed class MineColor
    {
        public char ID { get; }
        public string Name { get; }
        public Color Color { get; }
        public Color BackgroundColor { get; }
        public ConsoleColor ConsoleColor { get; }

        public static char[] ColorChars { get; } = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
        private static readonly MineColor[] colors;

        static MineColor()
        {
            colors = new MineColor[]
            {
                Black, DarkBlue, DarkGreen, DarkCyan, DarkRed,
                Purple, Gold, Gray, DarkGray, Blue, BrightGreen,
                Cyan, Red, Pink, Yellow, White
            };
        }

        public override string ToString() => $"{ID}: {Name}";

        private MineColor(char id, string name, Color color, Color backgroundColor, ConsoleColor consoleColor)
        {
            ID = id;
            Name = name;
            Color = color;
            BackgroundColor = backgroundColor;
            ConsoleColor = consoleColor;
        }

        public static MineColor Find(char c)
        {
            return colors.Where(x => x.ID == c).FirstOrDefault();
        }

        public static MineColor Find(string name)
        {
            return colors.Where(x => x.Name == name).FirstOrDefault();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }

        public override bool Equals(object obj)
        {
            return obj is MineColor color && ID == color.ID;
        }

        public static bool operator ==(MineColor a, MineColor b)
        {
            if (a is null)
                return b is null;

            return a.Equals(b);
        }

        public static bool operator !=(MineColor a, MineColor b)
        {
            return !(a == b);
        }

        public static MineColor Black { get; }
            = new MineColor('0', "black", Color.FromArgb(0x000000),
                Color.FromArgb(0x000000), ConsoleColor.Black);

        public static MineColor DarkBlue { get; }
            = new MineColor('1', "dark_blue", Color.FromArgb(0x0000aa),
                Color.FromArgb(0x00002a), ConsoleColor.DarkBlue);

        public static MineColor DarkGreen { get; }
            = new MineColor('2', "dark_green", Color.FromArgb(0x00aa00),
                Color.FromArgb(0x002a00), ConsoleColor.DarkGreen);

        public static MineColor DarkCyan { get; }
            = new MineColor('3', "dark_cyan", Color.FromArgb(0x00aaaa),
                Color.FromArgb(0x002a2a), ConsoleColor.DarkCyan);

        public static MineColor DarkRed { get; }
            = new MineColor('4', "dark_red", Color.FromArgb(0xaa0000),
                Color.FromArgb(0x2a0000), ConsoleColor.DarkRed);

        public static MineColor Purple { get; }
            = new MineColor('5', "dark_purple", Color.FromArgb(0xaa00aa),
                Color.FromArgb(0x2a002a), ConsoleColor.DarkMagenta);

        public static MineColor Gold { get; }
            = new MineColor('6', "gold", Color.FromArgb(0xffaa00),
                Color.FromArgb(0x2a2a00), ConsoleColor.DarkYellow);

        public static MineColor Gray { get; }
            = new MineColor('7', "gray", Color.FromArgb(0xaaaaaa),
                Color.FromArgb(0x2a2a2a), ConsoleColor.Gray);

        public static MineColor DarkGray { get; }
            = new MineColor('8', "dark_gray", Color.FromArgb(0x555555),
                Color.FromArgb(0x151515), ConsoleColor.DarkGray);

        public static MineColor Blue { get; }
            = new MineColor('9', "blue", Color.FromArgb(0x5555ff),
                Color.FromArgb(0x15153f), ConsoleColor.Blue);

        public static MineColor BrightGreen { get; }
            = new MineColor('a', "green", Color.FromArgb(0x55ff55),
                Color.FromArgb(0x153f15), ConsoleColor.Green);

        public static MineColor Cyan { get; }
            = new MineColor('b', "cyan", Color.FromArgb(0x55ffff),
                Color.FromArgb(0x153f3f), ConsoleColor.Cyan);

        public static MineColor Red { get; }
            = new MineColor('c', "red", Color.FromArgb(0xff5555),
                Color.FromArgb(0x3f1515), ConsoleColor.Red);

        public static MineColor Pink { get; }
            = new MineColor('d', "light_purple", Color.FromArgb(0xff55ff),
                Color.FromArgb(0x3f153f), ConsoleColor.Magenta);

        public static MineColor Yellow { get; }
            = new MineColor('e', "yellow", Color.FromArgb(0xffff55),
                Color.FromArgb(0x3f3f15), ConsoleColor.Yellow);

        public static MineColor White { get; }
            = new MineColor('f', "white", Color.FromArgb(0xffffff),
                Color.FromArgb(0x3f3f3f), ConsoleColor.White);
    }
}