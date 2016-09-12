using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.KMeans
{
  public class InstanceClustered
  {    
    public Instance instance { get; private set; }

    public Cluster cluster { get; private set; }

    public InstanceClustered(Instance instance, Cluster cluster)
    {
      if (instance == null)
        throw new ArgumentException("instance");
      if (cluster == null)
        throw new ArgumentException("cluster");

      this.instance = instance;
      this.cluster = cluster;
    }

  }
}
