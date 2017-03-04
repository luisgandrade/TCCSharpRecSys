using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnsupervisedLearning;
using UnsupervisedLearning.UserProfiles;
using UnsupervisedLearning.UserProfiles.Decay;
using UnsupervisedLearning.UserProfiles.Normalization;

namespace MachineLearning.UserProfiles
{
  public class WeightedAverageUserProfileBuilder : IUSerProfileBuilder
  {
    /// <summary>
    /// Fórmula de decaímento dos pesos ao decorrer do tempo.
    /// </summary>
    private IDecayFormula decay_formula;

    private IRatingsNormalization ratings_normalization;

    public string label
    {
      get
      {
        return decay_formula.decay_display + "_" + ratings_normalization.print;
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
        var trainingRatings = userRatings.OrderBy(ur => ur.timestamp).Take((int)(trainingCutoff * userRatings.Count())).ToList();        
        var ratingsNormalizationFunction = ratings_normalization.setupNormalizationFunction(trainingRatings.Select(tr => tr.rating).ToList());
        var lastTimestamp = trainingRatings.Max(tr => tr.timestamp);

        var profile = trainingRatings.Join(tagRelevances, trat => trat.movie, trel => trel.movie, (trat, trel) => new
        {
          normalizedRating = ratingsNormalizationFunction(trat.rating),
          ratingFreshness = decay_formula.decay((lastTimestamp - trat.timestamp).Duration()), //idade da avaliação relativa
          tag = trel.tag,
          relevance = trel.relevance
        }).GroupBy(tr => tr.tag)
          .Select(tr => new
          {
            tag = tr.Key,
            //Média ponderada das avaliações normalizadas atenuadas por um fator proporcional ao número de semanas da avaliação
            value = tr.Sum(r => r.normalizedRating * r.relevance * r.ratingFreshness)
                    /
                  tr.Sum(r => r.normalizedRating * r.ratingFreshness)
          });

        userProfiles.Add(new UserProfile(userRatings.Key, profile.OrderBy(pr => pr.tag.id).Select(pr => pr.value).ToList()));        
      }
      return userProfiles;
    }

    public WeightedAverageUserProfileBuilder(IDecayFormula decayFormula, IRatingsNormalization ratingsNormalization)
    {
      if (decayFormula == null)
        throw new ArgumentException("decayFormula");
      if (ratingsNormalization == null)
        throw new ArgumentException("ratingsNormalization");

      decay_formula = decayFormula;
      ratings_normalization = ratingsNormalization;
    }
  }
}
