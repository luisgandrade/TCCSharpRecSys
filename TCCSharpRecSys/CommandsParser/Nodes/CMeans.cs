using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class CMeans : IAlgorithm
  {

    public int cluster_count { get; private set; }

    public double fuzzyness_coefficient { get; private set; }

    public CMeans(int cluster_count, double fuzzyness_coefficient)
    {
      throw new NotImplementedException();
    }

    public IUnsupervisedLearning getAlgorithm(int attr_count)
    {
      throw new NotImplementedException();
    }
  }
}
