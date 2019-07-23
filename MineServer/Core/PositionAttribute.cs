using System;
using System.Runtime.CompilerServices;

namespace MineServer
{
    public class PositionAttribute : Attribute
    {
        private int pos;

        public PositionAttribute([CallerLineNumber]int cln = 0)
        {
            this.pos = cln;
        }

        public int Position { get => pos; }
    }
}