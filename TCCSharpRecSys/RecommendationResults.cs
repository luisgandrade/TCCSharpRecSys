using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;

namespace TCCSharpRecSys
{
  public class RecommendationResults
  {

    public int user_id { get; private set; }

    public int number_of_ratings { get; private set; }

    public double precision { get; private set; }
    
    public RecommendationResults(int user_id, int number_of_ratings, double precision)
    {     

      this.user_id = user_id;
      this.number_of_ratings = number_of_ratings;
      this.precision = precision;
    }
  }
}
