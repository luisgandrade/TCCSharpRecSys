using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;
using UnsupervisedLearning.SelfOrganizingMaps.Network;
using UnsupervisedLearning.KMeans;

namespace UnsupervisedLearning.SelfOrganizingMaps
{
  /// <summary>
  /// Implementação de <see cref="IMovieClassification"/> para o algoritmo <see cref="SelfOrganizingMap"/>.
  /// </summary>
  public class SOMMovieClassification : IMovieClassification
  {
    public Movie movie { get; set; }

    public Neuron neuron { get; set; }

    public IClassLabel label
    {
      get
      {
        return neuron;
      }
    }

    public SOMMovieClassification(Movie movie, Neuron neuron)
    {
      if (movie == null)
        throw new ArgumentException("movie");
      if (neuron == null)
        throw new ArgumentException("neuron");
      this.movie = movie;
      this.neuron = neuron;
    }

    public string print()
    {
      return movie.id + "," + neuron.x + "," + neuron.y;
    }
  }
}
