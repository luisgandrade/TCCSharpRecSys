using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;

namespace UnsupervisedLearning.KMeans
{
  public class Cluster : IClassLabel
  {
    public int id { get; private set; }

    public IList<double> centroid { get; private set; }
    

    public void updateCentroid(IList<double> newCentroid)
    {
      if (newCentroid == null)
        throw new ArgumentException("newCentroid");

      centroid = newCentroid;
    }

    public Cluster(int id, IList<double> means)
    {
      if (means == null)
        throw new ArgumentException("means");

      this.id = id;
      this.centroid = means;
    }

    public Cluster(int id)
    {      
      this.id = id;
    }

    public bool Equals(Cluster other)
    {
      if (other == null)
        return false;
      return this.id == other.id;
    }
  }
}
