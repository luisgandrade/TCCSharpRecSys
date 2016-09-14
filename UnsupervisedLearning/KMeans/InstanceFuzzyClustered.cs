using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.KMeans
{
  internal class InstanceFuzzyClustered : InstanceClustered
  {

    /// <summary>
    /// Grau de pertinência da instância ao cluster.
    /// </summary>
    public double degree_of_pertinence { get; private set; }

    public InstanceFuzzyClustered(Instance instance, Cluster cluster, double degree_of_pertinence)
      : base(instance, cluster)
    {
      if (degree_of_pertinence < 0 || degree_of_pertinence > 1)
        throw new ArgumentOutOfRangeException("O grau de pertinência deve estar entre zero e um.");

      this.degree_of_pertinence = degree_of_pertinence;      
    }
  }
}
