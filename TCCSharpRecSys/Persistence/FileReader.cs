using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TCCSharpRecSys.Persistence
{
  public class FileReader
  {

    private StreamReader reader;

    private string filesPath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory).Replace("\\TCCSharpRecSys\\bin\\Debug", "") + "\\Data\\";

    public IList<Movie> readMovies()
    {
      reader = new StreamReader(filesPath + "movies.csv");

      var movies = new List<Movie>();

      var regex = new Regex("^([0-9]+),\\s?\"(.+?)\",\\s?([0-9]*),\\s?([0-9]+),\\s?\"(.*?)\"$");

      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var match = regex.Match(line);

        var id = int.Parse(match.Groups[1].Value);
        var title = match.Groups[2].Value;
        var year = match.Groups[3].Value.Length > 0 ? int.Parse(match.Groups[3].Value): 0;
        var genre = int.Parse(match.Groups[4].Value);
        var keywords = match.Groups[5].Value;

        movies.Add(new Movie(id, title, year, (Genre) genre, keywords));
        Console.WriteLine(id);
      }

      reader.Close();
      reader = null;

      return movies;
    }

    public IList<Attr> readAttributes(int attr_count)
    {
      reader = new StreamReader(filesPath + "attrs_" + attr_count + ".csv");

      var attrs = new List<Attr>();

      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var properties = line.Split(',');

        var attr_index = int.Parse(properties[0]);
        var attribute = properties[1];
        var average = double.Parse(properties[2]);
        var std_dev = double.Parse(properties[3]);

        attrs.Add(new Attr(attr_index, attribute, average, std_dev));
      }

      reader.Close();
      reader = null;

      return attrs;
    }

    public IList<MovieAttribute> readMoviesAttributes(int attr_count)
    {
      reader = new StreamReader(filesPath + "movie_attr_" + attr_count + ".csv");

      var movieAttrs = new List<MovieAttribute>();

      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var properties = line.Split(',');

        var movie_id = int.Parse(properties[0]);
        var attr_index = int.Parse(properties[1]);
        var value = double.Parse(properties[2]);
        var normalized_value = properties[3];

        var movieAttr = new MovieAttribute(movie_id, attr_index, value);

        double parsedValue = 0;
        if (double.TryParse(normalized_value, out parsedValue))
          movieAttr.normalized_value = parsedValue;
        movieAttrs.Add(movieAttr);
      }

      reader.Close();
      reader = null;

      return movieAttrs;
    }

    public IList<Tuple<int, int, List<double>>> existingNeurons(int attr_count, int rows, int columns, int instance)
    {
      reader = new StreamReader(filesPath + "som_nodes\\" + attr_count + "_" + rows + "_" + columns + "_" + instance + ".csv"); ;

      var nodes = new List<Tuple<int, int, List<double>>>();

      reader.ReadLine();
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var split = line.Split(',');

        var coordinates = split[0].Trim('[', ']').Split(';');

        var nodesWeightsString = split[1].Trim('[', ']').Split(';');

        var nodesWeights = nodesWeightsString.Select(nws => double.Parse(nws)).ToList();

        nodes.Add(new Tuple<int, int, List<double>>(int.Parse(coordinates[0]), int.Parse(coordinates[1]), nodesWeights));
      }

      return nodes;
    }

  }
}
