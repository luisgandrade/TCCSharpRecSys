using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.ExpectationMaximization.MixtureModel
{
  public interface IMixtureModel
  {

    double prior(Instance instance, IList<double> mean_vector, IList<IList<double>> covariance_matrix);
  }
}
