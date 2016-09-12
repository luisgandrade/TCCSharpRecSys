using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCCSharpRecSys.Persistence;
using UnsupervisedLearning;
using Utils;

namespace TCCSharpRecSys
{
  public class UserProfileBuilder
  {
    public void buildUserProfiles(IList<Rating> ratings, IList<TagRelevance> tagRelevances, double trainingCutoff, string filePreffix, FileWritter fileWritter)
    {
      if (ratings == null)
        throw new ArgumentException("ratings");
      if (tagRelevances == null)
        throw new ArgumentException("tagRelevances");
      if (trainingCutoff <= 0 || trainingCutoff > 1)
        throw new ArgumentOutOfRangeException("Trainig cutoff precisa ser um valor maior que zero e menor ou igual a um.");


      var ratingsByUser = ratings.GroupBy(rat => rat.user_id);
      var userProfiles = new List<UserProfile>();
      var chunks = 1;
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

        userProfiles.Add(new UserProfile(userRatings.Key, profile.OrderBy(pr => pr.tag.id).Select(pr => pr.value).ToList()));

        if(userProfiles.Count % 50000 != 0 && userProfiles.Count != 0)
        {
          var profilesToBeWritten = userProfiles;
          userProfiles = new List<UserProfile>();
          Console.WriteLine("Escrevendo perfis dos usuários... [" + ((chunks - 1) * 50000) + " a " + (chunks * 50000 - 1) + "]");
          Task.Run(() => fileWritter.appendUserProfiles(filePreffix + "pt" + chunks + ".csv", profilesToBeWritten));
          chunks++;
        }
      }
      Console.WriteLine("Escrevendo perfis dos usuários... [restantes]");
      fileWritter.appendUserProfiles(filePreffix, userProfiles);
    }   
  }
}
