using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public interface IAlgorithm
  {
    IUnsupervisedLearning getAlgorithm(int attr_count);
  }
}
