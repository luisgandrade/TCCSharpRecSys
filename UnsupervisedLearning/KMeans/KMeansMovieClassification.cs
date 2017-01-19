using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;

namespace UnsupervisedLearning.KMeans
{
  /// <summary>
  /// Implementação de <see cref="IMovieClassification"/> para o algoritmo <see cref="StandardKMeans"/>.
  /// </summary>
  public class KMeansMovieClassification : IMovieClassification
  {
    public Movie movie { get; private set; }

    public Cluster cluster { get; private set; }

    public IClassLabel label
    {
      get
      {
        return cluster;
      }
    }

    public KMeansMovieClassification(Movie movie, Cluster cluster)
    {
      if (movie == null)
        throw new ArgumentException("movie");
      if (cluster == null)
        throw new ArgumentException("cluster");

      this.movie = movie;
      this.cluster = cluster;
    }

    public string print()
    {
      return movie.id + "," + cluster.id;
    }
  }
}
