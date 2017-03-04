using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.UserProfiles.Normalization
{
  public interface IRatingsNormalization
  {

    Func<double, double> setupNormalizationFunction(IList<double> ratings);

    string print { get; }

  }
}
