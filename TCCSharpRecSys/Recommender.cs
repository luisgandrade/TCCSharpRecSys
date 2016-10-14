using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;

namespace TCCSharpRecSys
{
  public class Recommender
  {

    public IUnsupervisedLearning labeler { get; private set; }

    public IDictionary<IClassLabel, List<Movie>> movies_by_label { get; private set; }

    public int result_count { get; private set; }


    public List<Movie> recommend(UserProfile userProfile, IList<Movie> moviesToExclude)
    {

      var bestMatchingClasses = labeler.best_matching_units(userProfile);

      var moviesByLabel = bestMatchingClasses.Join(movies_by_label, bmc => bmc, mbl => mbl.Key, (bmc, mbl) => new
      {
        movies = mbl.Value,
        count = mbl.Value.Count
      });

      var recommendedMovies = new List<Movie>();

      foreach (var movieLabel in moviesByLabel)
      {
        recommendedMovies.AddRange(movieLabel.movies.Except(moviesToExclude));
        if (recommendedMovies.Count > result_count)
          break;
      }

      return recommendedMovies;
    }


    public Recommender(IUnsupervisedLearning labeler, IDictionary<IClassLabel, List<Movie>> movies_by_label, int result_count)
    {
      if (labeler == null)
        throw new ArgumentException("labeler");
      if (movies_by_label == null)
        throw new ArgumentException("movies_by_label");

      this.labeler = labeler;
      this.movies_by_label = movies_by_label;
      this.result_count = result_count;
    }
  }
}
