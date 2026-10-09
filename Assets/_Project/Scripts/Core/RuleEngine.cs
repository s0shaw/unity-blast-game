using System;

namespace GemBlast.Core
{
    public class RuleEngine
    {
        public int A { get; private set; }
        public int B { get; private set; }
        public int C { get; private set; }

        public RuleEngine(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
        }

        public IconType GetIconTypeForGroupSize(int count)
        {
            if (count < A) return IconType.Default;
            if (count < B) return IconType.IconA;
            if (count < C) return IconType.IconB;
            return IconType.IconC;
        }
    }
}
