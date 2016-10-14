using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Metric
{
  /// <summary>
  /// Distância entre dois pontos usando similaridade de cossenos.
  /// </summary>
  public class CosineSimilarity : IMetric
  {
    public string print
    {
      get
      {
        return "cosine";
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

      var magFirst = Math.Sqrt(first.Sum(f => Math.Pow(f, 2)));
      var magSecond = Math.Sqrt(second.Sum(s => Math.Pow(s, 2)));

      var dotProduct = first.Zip(second, (f, s) => f * s).Sum();

      return Math.Acos(dotProduct / (magFirst * magSecond)) / Math.PI;
    }
  }
}
