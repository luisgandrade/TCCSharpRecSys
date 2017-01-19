using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.GaussianMixtureModel
{
  public class GaussianDistribution : IClassLabel
  {

    public double[] average { get; private set; }

    public double[][] covariance_matrix { get; private set; }

    public GaussianDistribution(double[] average, double[][] covariance_matrix)
    {
      this.average = average;
      this.covariance_matrix = covariance_matrix;
    }
  }
}
