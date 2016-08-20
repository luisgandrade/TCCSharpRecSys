using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
  public class EuclidianDistance
  {

    public static double euclidianDistance(IList<double> first, IList<double> second)
    {
      if (first == null)
        throw new ArgumentException("first");
      if (second == null)
        throw new ArgumentException("second");
      if (first.Count != second.Count)
        throw new InvalidOperationException("first != second");

      var squaresOfDifferences = first.Zip(second, (f, s) => (f - s) * (f - s));
      return Math.Sqrt(squaresOfDifferences.Sum());
    }
  }
}
