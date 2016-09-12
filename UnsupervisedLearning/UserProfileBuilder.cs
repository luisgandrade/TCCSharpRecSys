using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace UnsupervisedLearning
{
  public class UserProfileBuilder
  {
    public IEnumerable<UserProfile> buildUserProfiles(IList<Rating> ratings, IList<TagRelevance> tagRelevances, double trainingCutoff)
    {
      if (ratings == null)
        throw new ArgumentException("ratings");
      if (tagRelevances == null)
        throw new ArgumentException("tagRelevances");
      if (trainingCutoff <= 0 || trainingCutoff > 1)
        throw new ArgumentOutOfRangeException("Trainig cutoff precisa ser um valor maior que zero e menor ou igual a um.");


      var ratingsByUser = ratings.GroupBy(rat => rat.user_id);
      foreach (var userRatings in ratingsByUser)
      {
        var trainingRatings = userRatings.OrderBy(ur => ur.timestamp).Take((int)(trainingCutoff * userRatings.Count()));

        var ratingAverage = trainingRatings.Average(rat => rat.rating);
        var minRating = trainingRatings.Min(tr => tr.rating - ratingAverage);
        var maxMinusMinRating = trainingRatings.Max(rat => rat.rating - ratingAverage) - minRating;

        var lastTimestamp = trainingRatings.Max(tr => tr.timestamp);

        var profile = trainingRatings.Join(tagRelevances, trat => trat.movie, trel => trel.movie, (trat, trel) => new
        {
          normalizedRating = ((trat.rating - ratingAverage) - minRating) / maxMinusMinRating * 2 - 1, //normaliza no intervalo 0 a 1 e translada e escala para o intervalo -1 a 1
          timestamp = trat.timestamp,
          tag = trel.tag,
          relevance = trel.relevance
        }).GroupBy(tr => tr.tag)
          .Select(tr => new
          {
            tag = tr.Key,
            //Média pondera das avaliações normalizadas atenuadas por um fator proporcional ao número de semanas da avaliação
            value = tr.Sum(r => r.normalizedRating * r.relevance / ((lastTimestamp - r.timestamp).Duration().TotalDays / 7))
                    /
                  tr.Sum(r => r.normalizedRating / ((lastTimestamp - r.timestamp).Duration().TotalDays / 7))
          });

        yield return new UserProfile(userRatings.Key, profile.OrderBy(pr => pr.tag.id).Select(pr => pr.value).ToList());
      }
    }

    //public UserProfile buildUserProfile(IList<Rating> ratings, IList<TagRelevance> movieAttributes, DateTime momentoAtual)
    //{
    //  var userId = ratings.Select(rat => rat.user_id).Distinct();
    //  if (userId.Count() > 1)
    //    throw new InvalidOperationException("Nem todas as avaliações são do mesmo usuário.");
      
    //  var userAverageRating = ratings.Average(rat => rat.rating);

    //  var min = ratings.Min(rat => rat.rating - userAverageRating);
    //  var maxMinusMin = ratings.Max(rat => rat.rating - userAverageRating) - min;
      

    //  var ratingsAndMovieAttributes = ratings.Join(movieAttributes, rat => rat.movie, ma => ma.movie, (rat, ma) => new
    //  {
    //    normalizedRating = ((rat.rating - userAverageRating) - min) / maxMinusMin * 2 - 1 , //normaliza no intervalo 0 a 1 e translada e escala para o intervalo -1 a 1
    //    timestamp = rat.timestamp,
    //    tag = ma.tag,
    //    relevance = ma.relevance
    //  }).GroupBy(rta => rta.tag)
    //    .Select(rta => new
    //    {
    //      tag = rta.Key,
    //      //Média pondera das avaliações normalizadas atenuadas por um fator proporcional ao número de semanas da avaliação
    //      value = rta.Sum(r => r.normalizedRating * r.relevance / ((momentoAtual - r.timestamp).Duration().TotalDays / 7)) 
    //                /
    //              rta.Sum(r => r.normalizedRating / ((momentoAtual- r.timestamp).Duration().TotalDays / 7))
    //    });
    //  return new UserProfile(userId.Single(), ratingsAndMovieAttributes.OrderBy(rma => rma.tag.id).Select(rma => rma.value).ToList());
    //}
  }
}
