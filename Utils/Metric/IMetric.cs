using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Metric
{
  public interface IMetric
  {
    double applyMetric(IList<double> first, IList<double> second);

    string print { get; }
  }
}
