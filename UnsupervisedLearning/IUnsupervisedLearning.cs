using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;

namespace UnsupervisedLearning
{
  public interface IUnsupervisedLearning
  {

    void train(IList<Instance> instances);

    IList<IMovieClassification> classify_instances(IList<Instance> tagRelevances);

    IEnumerable<string> printClassifier();


  }
}
