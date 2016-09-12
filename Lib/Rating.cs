using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
  public class Rating
  {

    public Movie movie { get; private set; }

    public int user_id { get; private set; }

    public double rating { get; private set; }

    public DateTime timestamp { get; private set; }

    public Rating(Movie movie, int user_id, double rating, DateTime timestamp)
    {
      this.movie = movie;
      this.user_id = user_id;
      this.rating = rating;
      this.timestamp = timestamp;
    }
  }
}
