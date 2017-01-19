using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  /// <summary>
  /// Fornece uma interface para filmes classificados.
  /// </summary>
  public interface IMovieClassification
  {
    Movie movie { get; }   

    IClassLabel label { get; }

    string print();
  }
}
