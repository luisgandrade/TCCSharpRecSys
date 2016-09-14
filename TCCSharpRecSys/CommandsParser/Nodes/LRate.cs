using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class LRate
  {
    public ILearningRateFunction learning_rate_function { get; set; }

    public LRate(double initial_learning_rate, double time_constant)
    {
      learning_rate_function = new LearningRateFunction(initial_learning_rate, time_constant);
    }

    public LRate()
    {
      learning_rate_function = new LearningRateFunction();
    }
  }
}
