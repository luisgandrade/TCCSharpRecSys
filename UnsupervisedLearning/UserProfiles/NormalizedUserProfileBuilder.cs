using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;

namespace UnsupervisedLearning.UserProfiles
{
  public class NormalizedUserProfileBuilder : IUSerProfileBuilder
  {
    public string label
    {
      get
      {
        return "normalized";
      }
    }

    public IList<UserProfile> buildUserProfiles(IList<Rating> ratings, IList<TagRelevance> tagRelevances, double trainingCutoff)
    {
      if (ratings == null)
        throw new ArgumentException("ratings");
      if (tagRelevances == null)
        throw new ArgumentException("tagRelevances");
      if (trainingCutoff <= 0 || trainingCutoff > 1)
        throw new ArgumentOutOfRangeException("Trainig cutoff precisa ser um valor maior que zero e menor ou igual a um.");

      var ratingsByUser = ratings.GroupBy(rat => rat.user_id);
      var userProfiles = new List<UserProfile>();
      foreach (var userRatings in ratingsByUser)
      {
        var trainingRatings = userRatings.OrderBy(ur => ur.timestamp).Take((int)(trainingCutoff * userRatings.Count()));

        var ratingAverage = trainingRatings.Average(rat => rat.rating);
        var minRating = trainingRatings.Min(tr => tr.rating);
        var maxMinusMinRating = trainingRatings.Max(rat => rat.rating) - minRating;

        var lastTimestamp = trainingRatings.Max(tr => tr.timestamp);

        var notNormalizedProfile = trainingRatings.Join(tagRelevances, trat => trat.movie, trel => trel.movie, (trat, trel) => new
        {
          normalizedRating = (trat.rating - minRating) / maxMinusMinRating,
          relativeRatingAge = (lastTimestamp - trat.timestamp).Duration(), //idade da avaliação relativa
          tag = trel.tag,
          relevance = trel.relevance
        }).GroupBy(tr => tr.tag)
          .Select(tr => new
          {
            tag = tr.Key,
            //Média pondera das avaliações normalizadas atenuadas por um fator proporcional ao número de semanas da avaliação
            value = tr.Sum(r => r.normalizedRating * r.relevance )
          });
        var max = notNormalizedProfile.Max(nnp => nnp.value);

        userProfiles.Add(new UserProfile(userRatings.Key, notNormalizedProfile.OrderBy(pr => pr.tag.id).Select(pr => pr.value / max).ToList()));
      }
      return userProfiles;
    }
  }
}
