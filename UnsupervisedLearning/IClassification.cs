using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  public interface IClassification<T>
    where T : IEquatable<T>
  {
    IEnumerable<IClassLabel<T>> classify(IList<double> instance_attributes);
  }
}
