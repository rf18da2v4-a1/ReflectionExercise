using System;
using System.Collections.Generic;
using System.Text;

namespace ReflectionLib.model
{
    public class TestVarClass
    {
        public int Ivar { get; set; }
        public short Svar { get; set; }
        public long Lvar { get; set; }
        public byte Bvar { get; set; }
        public char Cvar { get; set; }
        public bool Boolvar { get; set; }
        public string  Strvar { get; set; }

        public TestVarClass()
        {
        }

        public TestVarClass(int ivar, short svar, long lvar, byte bvar, char cvar, bool boolvar, string strvar)
        {
            Ivar = ivar;
            Svar = svar;
            Lvar = lvar;
            Bvar = bvar;
            Cvar = cvar;
            Boolvar = boolvar;
            Strvar = strvar;
        }

        public override string ToString()
        {
            return $"{nameof(Ivar)}: {Ivar}, {nameof(Svar)}: {Svar}, {nameof(Lvar)}: {Lvar}, {nameof(Bvar)}: {Bvar}, {nameof(Cvar)}: {Cvar}, {nameof(Boolvar)}: {Boolvar}, {nameof(Strvar)}: {Strvar}";
        }
    }
}
