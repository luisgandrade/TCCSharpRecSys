using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnsupervisedLearning;
using UnsupervisedLearning.UserProfiles;

namespace MachineLearning.UserProfiles
{
  public class WeightedAverageUserProfileBuilder : IUSerProfileBuilder
  {
    /// <summary>
    /// Fórmula de decaímento dos pesos ao decorrer do tempo.
    /// </summary>
    private IDecayFormula decay_formula;

    public string label
    {
      get
      {
        return decay_formula.decay_display;
      }
    }

    /// <summary>
    /// Constróis os <see cref="UserProfile"/> dos usuários que possuem um número suficiente de <see cref="Rating"/>s.  Os <see cref="Rating"/>s
    /// do usuário são ordernados e separados em dois grupos: um de treinamento e outro de teste (o último não é usado aqui). A porcentagem de split é 
    /// definida por <paramref name="trainingCutoff"/>
    /// </summary>
    /// <param name="ratings">avaliações dos usuários</param>
    /// <param name="tagRelevances">todas as relevâncias de tags e filmes</param>
    /// <param name="trainingCutoff">porcentagem de split</param>
    /// <returns></returns>
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

        var profile = trainingRatings.Join(tagRelevances, trat => trat.movie, trel => trel.movie, (trat, trel) => new
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
            value = tr.Sum(r => r.normalizedRating * r.relevance * decay_formula.decay(r.relativeRatingAge))
                    /
                  tr.Sum(r => r.normalizedRating * decay_formula.decay(r.relativeRatingAge))
          });

        userProfiles.Add(new UserProfile(userRatings.Key, profile.OrderBy(pr => pr.tag.id).Select(pr => pr.value).ToList()));        
      }
      return userProfiles;
    }

    public WeightedAverageUserProfileBuilder(IDecayFormula decayFormula)
    {
      if (decayFormula == null)
        throw new ArgumentException("decayFormula");

      decay_formula = decayFormula;
    }
  }
}
