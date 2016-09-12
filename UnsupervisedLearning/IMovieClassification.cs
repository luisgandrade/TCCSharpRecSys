using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  public interface IMovieClassification<T>
  {
    Movie movie { get; }

    T class_label { get; }
  }
}
