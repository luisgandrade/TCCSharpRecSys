using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace TCCSharpRecSys
{
  public class AttributeExtractor
  {



    public IList<Attr> chooseAttributesFromMovies(IList<Movie> movies, decimal minimal_occurence_of_keyword)
    {
           

      var keywords = movies.SelectMany(mv => mv.keywords.Split('|'))
                           .GroupBy(k => k)
                           .Select(k => new { keyword = k.Key, occurences = k.Count() });

      var keywordsChosen = keywords.Where(k => k.occurences >= movies.Count * minimal_occurence_of_keyword)
                                   .Select(k => k.keyword)
                                   .Where(k => k.Length > 0);

      var attributes = Enum.GetValues(typeof(Genre)).Cast<Genre>().Where(g => g != Genre.unknown).Select(g => g.Description())
                                                                  .Concat(keywordsChosen)
                                                                  .Select((attr, index) => new Attr(index + 1, attr));

      return attributes.ToList();
    }

    public IList<MovieAttribute> extractAttributesFromMovies(IList<Movie> movies, IList<Attr> attributes, bool normalize)
    {

      var genreAttributes = attributes.Take(18).Join(Enum.GetValues(typeof(Genre)).Cast<Genre>(), attr => attr.attribute, g => g.Description(), 
                                                 (attr, g) => new { attribute = attr, genre = g });
      var movieGenreAttributes = movies.SelectMany(mv => genreAttributes.Select(ga => new MovieAttribute(mv.id, ga.attribute.attr_index, mv.genres.HasFlag(ga.genre) ? 1 : 0)));

      var keywordsAttributes = attributes.Skip(18);
      var movieKeywordsAttributes = movies.SelectMany(mv => keywordsAttributes.Select(ka => new MovieAttribute(mv.id, ka.attr_index, mv.keywords.Contains(ka.attribute) ? 1 : 0)));

      var moviesAttributes = movieGenreAttributes.Concat(movieKeywordsAttributes).ToList();

      if (normalize)
      {
        var attrAvgAndStd = moviesAttributes.GroupBy(ma => ma.attribute_id)
                                          .Select(ma => new
                                          {
                                            attribute_id = ma.Key,
                                            average = ma.Select(m => m.value).Average(),
                                            average_of_the_squares = ma.Select(m => m.value).Average(m => m * m)
                                          })
                                          .Select(ma => new { attribute_id = ma.attribute_id, average = ma.average, std_dev = Math.Sqrt(ma.average_of_the_squares - Math.Pow(ma.average, 2)) });
        
        var moviesWithAttrStats = moviesAttributes.Join(attrAvgAndStd, ma => ma.attribute_id, aas => aas.attribute_id, (ma, aas) => new { movieAttribute = ma, stats = aas });

        foreach (var movieAttribute in moviesWithAttrStats)
          movieAttribute.movieAttribute.normalized_value = (movieAttribute.movieAttribute.value - movieAttribute.stats.average) / (movieAttribute.stats.std_dev != 0 ? movieAttribute.stats.std_dev : 1);

        var attrsWithStats = attributes.Join(attrAvgAndStd, attr => attr.attr_index, aas => aas.attribute_id, (attr, aas) => new
        {
          attribute = attr,
          average = aas.average,
          std_dev = aas.std_dev
        });

        foreach (var attrStats in attrsWithStats)
        {
          attrStats.attribute.population_average = attrStats.average;
          attrStats.attribute.population_standard_deviation = attrStats.std_dev;
        }
      }

      return moviesAttributes.ToList();
    }

  }
}
