using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Metric
{
  /// <summary>
  /// Distância entre dois pontos usando distância de Manhattan.
  /// </summary>
  public class ManhattanDistance : IMetric
  {
    public string print
    {
      get
      {
        return "manhattan";
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

      return first.Zip(second, (f, s) => Math.Abs(f - s)).Sum();
    }
  }
}
