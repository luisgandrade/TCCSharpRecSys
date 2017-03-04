using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.UserProfiles.Normalization
{
  public class FeatureScaling : IRatingsNormalization
  {
    public string print
    {
      get
      {
        return "feature_scaling";
      }
    }

    public Func<double, double> setupNormalizationFunction(IList<double> ratings)
    {
      var min = ratings.Min();
      var max = ratings.Max();

      return r => (r - min) / (max - min);
    }
  }
}
