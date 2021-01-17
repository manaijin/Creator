using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Framework.InputType;

namespace Framework
{
    public class InputCombination<T> where T : struct
    {
        public int[] InputIndexes;
        public KeyState[] states;

        public InputCombination(T input, KeyState states)
        {
            this.InputIndexes = new[] { input.GetHashCode() };
            this.states = new[] { states };
        }

        public InputCombination(T[] inputs, KeyState[] states)
        {
            this.InputIndexes = inputs as int[];
            this.states = states;
        }

        public InputCombination(T[] inputs, KeyState state)
        {
            this.InputIndexes = inputs as int[];
            var length = inputs.Length;
            this.states = new KeyState[length];
            for (int i = 0; i < length; i++)
            {
                states[i] = state;
            }
        }

        public override bool Equals(object combine)
        {
            if (combine == null) return false;

            if (combine.GetType() != this.GetType()) return false;

            return CompareIntArrayEqual(this.InputIndexes, ((InputCombination<T>)combine).InputIndexes);
        }

        public override int GetHashCode()
        {
            return -1532116648 + EqualityComparer<int[]>.Default.GetHashCode(InputIndexes);
        }

        private bool CompareIntArrayEqual(int[] a, int[] b)
        {
            if (a == null || b == null)
                return false;

            var lengthA = a.Length;
            var lengthB = b.Length;

            if (lengthA != lengthB)
                return false;

            for (var i = 0; i < lengthA; i++)
            {
                var flag = false;
                for (var j = 0; j < lengthB; j++)
                {
                    if (a[i] == b[j])
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
