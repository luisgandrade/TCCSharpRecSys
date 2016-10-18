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

    public IUnsupervisedLearning algorithm { get; private set; }

    public IDictionary<IClassLabel, List<Movie>> movies_by_label { get; private set; }

    public int predict_next_n_movies { get; private set; }


    public RecommendationResults recommend(UserProfile userProfile, IList<Movie> moviesAlreadyWatched, IList<Rating> ratingsNotIncluded)
    {

      var bestMatchingClasses = algorithm.best_matching_units(userProfile);

      var moviesByLabel = bestMatchingClasses.Join(movies_by_label, bmc => bmc, mbl => mbl.Key, (bmc, mbl) => new
      {
        movies = mbl.Value,
        count = mbl.Value.Count
      });

      var moviesRecommended = 0;      
      var firstNRecommendationsPrecision = 0;
      var lastNRecommendationsPrecision = 0;
      var moviesByLabelEnumerator = moviesByLabel.GetEnumerator();

      var nextNMovies = ratingsNotIncluded.OrderBy(rni => rni.timestamp).Take(predict_next_n_movies).Select(rni => rni.movie).ToList();

      while (moviesRecommended < 2 * predict_next_n_movies || (firstNRecommendationsPrecision + lastNRecommendationsPrecision) >= nextNMovies.Count)
      {
        var moviesLabel = moviesByLabelEnumerator.Current;
        foreach (var movie in moviesLabel.movies.Except(moviesAlreadyWatched))
        {
          if (nextNMovies.Contains(movie))
          {
            if (moviesRecommended < predict_next_n_movies)
              firstNRecommendationsPrecision++;
            else
              lastNRecommendationsPrecision++;
          }
          moviesRecommended++;
        }
        moviesByLabelEnumerator.MoveNext();
      }

      return new RecommendationResults(userProfile, moviesAlreadyWatched.Count + ratingsNotIncluded.Count, firstNRecommendationsPrecision, lastNRecommendationsPrecision);
    }


    public Recommender(IUnsupervisedLearning algorithm, IDictionary<IClassLabel, List<Movie>> movies_by_label, int predict_next_n_movies)
    {
      if (algorithm == null)
        throw new ArgumentException("algorithm");
      if (movies_by_label == null)
        throw new ArgumentException("movies_by_label");

      this.algorithm = algorithm;
      this.movies_by_label = movies_by_label;
      this.predict_next_n_movies = predict_next_n_movies;
    }
  }
}
