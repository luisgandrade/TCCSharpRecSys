using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  public interface IMovieClassification
  {
    Movie movie { get; }

    IClassLabel class_label { get; }
  }
}
