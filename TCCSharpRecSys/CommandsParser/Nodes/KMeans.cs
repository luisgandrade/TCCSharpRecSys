using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;
using UnsupervisedLearning.KMeans;

namespace TCCSharpRecSys.CommandsParser.Nodes
{
  public class KMeans : IAlgorithm
  {

    public int cluster_count { get; private set; }
        
    public KMeans(int clusterCount)
    {
      if (clusterCount <= 1)
        throw new InvalidOperationException("O número de clusters deve ser maior que um.");
      this.cluster_count = clusterCount;
    }

    public IUnsupervisedLearning getAlgorithm(int attr_count)
    {
      return new StandardKMeans(cluster_count);
    }
  }
}
