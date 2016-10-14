using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnsupervisedLearning;
using UnsupervisedLearning.SelfOrganizingMaps;

namespace TCCSharpRecSys.Persistence
{
  public class FileReader
  {

    private StreamReader reader;

    private static FileReader file_reader;

    private static string dir_path;
    
    private FileReader()
    {
      movies_read = new List<Movie>();
      ratings_read = new List<Rating>();
      tags_read = new List<Tag>();
      movie_classification_read = new List<IMovieClassification>();
      tag_relevances_read = new List<TagRelevance>();
    }

    public static FileReader getInstance()
    {
      if (file_reader == null)
        file_reader = new FileReader();
      return file_reader;
    }

    public static void setDirPath(string dirPath)
    {
      if (dirPath == null)
        throw new ArgumentException("filePath");

      if (!Directory.Exists(dirPath))
        Directory.CreateDirectory(dirPath);

      dir_path = dirPath;
    }

    private IList<Movie> movies_read;
    private IList<Rating> ratings_read;
    private IList<Tag> tags_read;
    private IList<TagRelevance> tag_relevances_read;
    private IList<IMovieClassification> movie_classification_read;


    public IList<int> existingInstances(string sub_dir, string file_prefix)
    {
      if (sub_dir == null)
        throw new ArgumentException("sub_dir");
      if (file_prefix == null)
        throw new ArgumentException("file_prefix");

      var regex = new Regex(file_prefix + "_([0-9]+)\\.csv");      
      var filesInSubDir = Directory.GetFiles(dir_path + sub_dir + "\\train\\");
      var instances = new List<int>();
      foreach (var file in filesInSubDir)
      {
        var match = regex.Match(file);
        if (match.Success)
          instances.Add(int.Parse(match.Groups[1].Value));
      }
      return instances;
    }

    public IList<Movie> readMovies()
    {
      if (movies_read.Any())
        return movies_read;
      reader = new StreamReader(dir_path + "movies.csv");

      var movies = new List<Movie>();

      var regex = new Regex("^([0-9]+),\\s?\"(.+?)\",\\s?([0-9]{4}),\\s?([0-9]+)$");

      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var match = regex.Match(line);

        var id = int.Parse(match.Groups[1].Value);
        var title = match.Groups[2].Value;
        var year = match.Groups[3].Value.Length > 0 ? int.Parse(match.Groups[3].Value): 0;

        movies.Add(new Movie(id, title, year));
        Console.WriteLine(id);
      }

      reader.Close();
      reader = null;

      movies_read = movies;
      return movies;
    }

    public IList<int> getPartsOfRatings()
    {
      var files = Directory.GetFiles(dir_path + "profiles");
      var regex = new Regex("ratings_pt([0-9])+\\.csv");
      var pts = new List<int>();
      foreach (var file in files)
      {
        var match = regex.Match(file);
        if (match.Success)
          pts.Add(int.Parse(match.Groups[1].Value));
      }
      return pts;
    }

    public IList<Rating> readUserRatings(int chunk)
    {      
      if (!movies_read.Any())
        readMovies();
      if (!File.Exists(dir_path + "profiles\\ratings_pt" + chunk + ".csv"))
        throw new FileNotFoundException("Arquivo de ratings não existe.");
      reader = new StreamReader(dir_path + "profiles\\ratings_pt" + chunk + ".csv");

      
      var ratingsInfo = new List<Tuple<int, int, double, DateTime>>();
      reader.ReadLine();
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var properties = line.Split(',');

        var movieId = int.Parse(properties[0]);
        var userId = int.Parse(properties[1]);        
        var rating = double.Parse(properties[2]);
        var timestampSec = int.Parse(properties[3]);

        ratingsInfo.Add(new Tuple<int, int, double,DateTime>(movieId, userId, rating, new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestampSec)));
      }

      var ratings = ratingsInfo.Join(movies_read, ri => ri.Item1, mr => mr.id, (ri, mr) => new Rating(mr, ri.Item2, ri.Item3, ri.Item4)).ToList();

      ratings_read = ratings;

      return ratings;
    }

    public IList<Tag> readTags()
    {
      if (tags_read.Any())
        return tags_read;
      reader = new StreamReader(dir_path + "tags.csv");

      var tags = new List<Tag>();

      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var properties = line.Split(',').ToList();

        var id = int.Parse(properties[0]);
        var tag = properties[1];
        var occurences = int.Parse(properties[2]);
        var average = double.Parse(properties[3]);
        var std_dev = double.Parse(properties[4]);
        
        tags.Add(new Tag(id, tag, average, std_dev));
      }

      reader.Close();
      reader = null;

      tags_read = tags;

      return tags;
    }

    public IList<TagRelevance> readTagRelevances()
    {
      if (tag_relevances_read.Any())
        return tag_relevances_read;
      if (!tags_read.Any())
        readTags();
      if (!movies_read.Any())
        readMovies();
      reader = new StreamReader(dir_path + "tag_relevance.csv");

      
      var tagRelInfo = new List<Tuple<int, int, double, double>>();
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();
        var properties = line.Split(',');

        var movie_id = int.Parse(properties[0]);
        var tag_id = int.Parse(properties[1]);
        var relevance = double.Parse(properties[2]);
        var normalized_relevance = properties[3];
        double parsedValue = 0;
        double.TryParse(normalized_relevance, out parsedValue);

        tagRelInfo.Add(new Tuple<int, int, double, double>(movie_id, tag_id, relevance, parsedValue));
      }

      var tagRelevances = tagRelInfo.Join(movies_read, tri => tri.Item1, mr => mr.id, (tri, mr) => new { movie = mr, tagRel = tri })
                                    .Join(tags_read, trim => trim.tagRel.Item2, tr => tr.id, (trim, tr) => new TagRelevance(trim.movie, tr, trim.tagRel.Item3)
                                    {
                                      normalized_relevance = trim.tagRel.Item4
                                    }).ToList();

      reader.Close();
      reader = null;


      tag_relevances_read = tagRelevances;

      return tagRelevances;
    }

    public IList<IMovieClassification> readMovieClassification(string algorithmDir, string filename, Func<Movie, string, IMovieClassification> classLabelParser)
    {
      if (movie_classification_read.Any())
        return movie_classification_read;
      if (!movies_read.Any())
        readMovies();
      reader = new StreamReader(dir_path + algorithmDir + "\\" + "movie_classification\\" + filename +".csv");

      
      var moviesClassification = new List<IMovieClassification>();
      var regex = new Regex("([0-9]+)\\s?,\\s?(.*)");
      while (!reader.EndOfStream)
      {
        var line = reader.ReadLine();

        var match = regex.Match(line);
        if (!match.Success)
          throw new InvalidOperationException("Não deu match.");
        

        var movie_id = int.Parse(match.Groups[1].Value);
        var classlabel = match.Groups[2].Value;

        var movie = movies_read.Single(mr => mr.id == movie_id);

        moviesClassification.Add(classLabelParser(movie, classlabel));
      }

      reader.Close();
      reader = null;
      
      movie_classification_read = moviesClassification;
      

      return moviesClassification;
    }

    public IList<string> readAlgorithmConfig(string algorithmDir, string filename, int instance)
    {

      reader = new StreamReader(dir_path + algorithmDir + "\\train_" + filename + "_" + instance + ".csv");
      var classLabelsStr = new List<string>();

      while (!reader.EndOfStream)
        classLabelsStr.Add(reader.ReadLine());
      reader.Close();
      reader = null;

      return classLabelsStr;
    }

    //public IList<Tuple<int, int, List<double>>> existingNeurons(int rows, int columns, string metric, string neighborhood, int instance)

    //{
    //  reader = new StreamReader(dir_path + "som_nodes\\" + rows + "_" + columns + "_" + metric + "_" + neighborhood + "_" + instance + ".csv"); ;

    //  var nodes = new List<Tuple<int, int, List<double>>>();

    //  reader.ReadLine();
    //  while (!reader.EndOfStream)
    //  {
    //    var line = reader.ReadLine();
    //    var split = line.Split(',');

    //    var coordinates = split[0].Trim('[', ']').Split(';');

    //    var nodesWeightsString = split[1].Trim('[', ']').Split(';');

    //    var nodesWeights = nodesWeightsString.Select(nws => double.Parse(nws)).ToList();

    //    nodes.Add(new Tuple<int, int, List<double>>(int.Parse(coordinates[0]), int.Parse(coordinates[1]), nodesWeights));
    //  }

    //  return nodes;
    //}

  }
}
