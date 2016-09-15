using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;

namespace TCCSharpRecSys
{
  public class Recommender<T>
    where T : IEquatable<T>
  {
    
    //public IClassification<T> labeler { get; private set; }

    //public IDictionary<IClassLabel<T>, List<Movie>> movies_by_label { get; private set; }

    //public int result_count { get; private set; }


    //public List<Movie> recommend(IList<double> tagsRelevances, IList<Movie> moviesToExclude)
    //{

    //  var bestMatchingClasses = labeler.classify(tagsRelevances);

    //  var moviesByLabel = bestMatchingClasses.Join(movies_by_label, bmc => bmc, mbl => mbl.Key, (bmc, mbl) => new
    //  {
    //    movies = mbl.Value,
    //    count = mbl.Value.Count
    //  });

    //  var recommendedMovies = new List<Movie>();

    //  foreach (var movieLabel in moviesByLabel)
    //  {        
    //    recommendedMovies.AddRange(movieLabel.movies.Except(moviesToExclude));
    //    if (recommendedMovies.Count > result_count)
    //      break;
    //  }

    //  return recommendedMovies;
    //}
    

    //public Recommender(IClassification<T> labeler, IDictionary<IClassLabel<T>, List<Movie>> movies_by_label, int result_count)
    //{
    //  if (labeler == null)
    //    throw new ArgumentException("labeler");
    //  if (movies_by_label == null)
    //    throw new ArgumentException("movies_by_label");
      
    //  this.labeler = labeler;
    //  this.movies_by_label = movies_by_label;
    //  this.result_count = result_count;
    //}
  }
}
