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

    public int recommend_n_movies { get; private set; }


    public RecommendationResults recommend(UserProfile userProfile, IList<Movie> moviesAlreadyWatched, IList<Rating> ratingsNotIncluded, double ratingAverage)
    {
      if (userProfile == null)
        throw new ArgumentException("userProfile");
      if (moviesAlreadyWatched == null)
        throw new ArgumentException("moviesAlreadyWatched");
      if (ratingsNotIncluded == null)
        throw new ArgumentException("ratingsNotIncluded");

      var bestMatchingClasses = algorithm.best_matching_units(userProfile, 30).ToList();

      var moviesByLabel = new List<Movie>();
      foreach (var bmc in bestMatchingClasses)
      {
        foreach (var m in movies_by_label.Keys)
        {
          if (bmc.Equals(m))
            moviesByLabel.AddRange(movies_by_label[m]);
        }
      }

      //var moviesByLabel = bestMatchingClasses.Join(movies_by_label, bmc => bmc, mbl => mbl.Key, (bmc, mbl) => mbl.Value).SelectMany(mbl => mbl).ToList();

      var moviesRecommended = 0;      
      var numberOfCorrectPredictions = 0;
      var moviesByLabelEnumerator = moviesByLabel.GetEnumerator();
           

      while (moviesRecommended < recommend_n_movies && numberOfCorrectPredictions < ratingsNotIncluded.Count)
      {
        moviesByLabelEnumerator.MoveNext();
        var movie = moviesByLabelEnumerator.Current;
        if (!moviesAlreadyWatched.Contains(movie))
        {
          if (ratingsNotIncluded.Any(r => r.movie == movie && r.rating > ratingAverage))
            numberOfCorrectPredictions++;
          moviesRecommended++;
        }
      }

      return new RecommendationResults(userProfile, moviesAlreadyWatched.Count + ratingsNotIncluded.Count, numberOfCorrectPredictions);
    }


    public Recommender(IUnsupervisedLearning algorithm, IDictionary<IClassLabel, List<Movie>> movies_by_label, int recommend_next_n_movies)
    {
      if (algorithm == null)
        throw new ArgumentException("algorithm");
      if (movies_by_label == null)
        throw new ArgumentException("movies_by_label");

      this.algorithm = algorithm;
      this.movies_by_label = movies_by_label;
      this.recommend_n_movies = recommend_next_n_movies;
    }
  }
}
