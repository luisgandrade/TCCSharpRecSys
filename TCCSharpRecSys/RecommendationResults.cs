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

    public UserProfile user { get; private set; }

    public int number_of_ratings { get; private set; }

    public int first_n_recommendations_precision { get; private set; }

    public int last_n_recommendations_precision { get; private set; }

    public RecommendationResults(UserProfile user, int number_of_ratings, int first_n_recommendations_precision, int last_n_recommendations_precision)
    {
      if (user == null)
        throw new ArgumentException("user");

      this.user = user;
      this.number_of_ratings = number_of_ratings;
      this.first_n_recommendations_precision = first_n_recommendations_precision;
      this.last_n_recommendations_precision = last_n_recommendations_precision;
    }
  }
}
