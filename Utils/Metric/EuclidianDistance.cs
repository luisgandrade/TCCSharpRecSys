using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Metric
{
  /// <summary>
  /// Distância entre dois pontos usando distância euclidiana.
  /// </summary>
  public class EuclidianDistance : IMetric
  {
    public string print
    {
      get
      {
        return "euclidian";
      }
    }

    public double applyMetric(IList<double> first, IList<double> second)
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

    public static double distance(IList<double> first, IList<double> second)
    {
      return new EuclidianDistance().applyMetric(first, second);
    }
  }
}
