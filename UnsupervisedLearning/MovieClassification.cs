using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.Network;
using Utils;

namespace UnsupervisedLearning
{
  public class MovieClassification : IMovieClassification<Coordinate>
  {

    public Movie movie { get; private set; }

    public Coordinate coordinate { get; private set; }

    public Coordinate class_label
    {
      get
      {
        return coordinate;
      }
    }

    public MovieClassification(Movie movie, int x, int y)
    {
      this.movie = movie;
      this.coordinate = new Coordinate(x, y);
    }    
  }
}
