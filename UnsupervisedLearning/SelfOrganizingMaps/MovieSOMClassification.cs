using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;
using UnsupervisedLearning.SelfOrganizingMaps.Network;

namespace UnsupervisedLearning.SelfOrganizingMaps
{
  public class MovieSOMClassification : IMovieClassification<Neuron>
  {
    public Movie movie { get; set; }

    public Neuron neuron { get; set; }

    public Neuron class_label
    {
      get
      {
        return neuron;
      }
    }


    public MovieSOMClassification(Movie movie, Neuron neuron)
    {
      if (movie == null)
        throw new ArgumentException("movie");
      if (neuron == null)
        throw new ArgumentException("neuron");
      this.movie = movie;
      this.neuron = neuron;
    }    
  }
}
