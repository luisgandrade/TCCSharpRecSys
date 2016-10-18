using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;

namespace UnsupervisedLearning.KMeans
{
  /// <summary>
  /// Unidade de agrumanento do algoritmo K-Means.
  /// </summary>
  public class Cluster : IClassLabel
  {
    /// <summary>
    /// Identificador da unidade.
    /// </summary>
    public int id { get; private set; }
    /// <summary>
    /// Centróide do cluster. É a média de todas as instâncias classificadas neste cluster.
    /// </summary>
    public IList<double> centroid { get; private set; }
    
    /// <summary>
    /// Atualiza o centróide desse cluster com um novo centróide.
    /// </summary>
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
