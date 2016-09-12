using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  public interface IClassLabel<T>
    where T: IEquatable<T>
  {
    //string class_label { get; }
  }
}
