using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Particles
{
    public interface IEvaluable<T>
    {
        public T Evaluate(float t);
    }
    public interface ILerpable<T>
    {
        public T Lerp(T a, T b, float t);
    }

    public static class Utils
    {
        public static T[] Quantize<T>(this IEvaluable<T> evaluable, int samples)
        {
            var result = new T[samples];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = evaluable.Evaluate(((float)i) / ((float)samples));
            }

            return result;
        }
    }
}
