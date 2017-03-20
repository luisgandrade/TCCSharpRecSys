using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnsupervisedLearning;
using UnsupervisedLearning.KMeans;
using UnsupervisedLearning.SelfOrganizingMaps;

namespace TCCSharpRecSys.Persistence
{
  public class FileWriter
  {

    private static string dir_path;

    private static FileWriter file_writter;        

    private FileWriter()
    {
    }

    public static FileWriter getInstance()
    {
      if (file_writter == null)
      {
        if (dir_path == null)
          throw new InvalidOperationException("Antes de instanciar o escritor de arquivos é necessário setar o caminho absolutos dos arquivos.");
        file_writter = new FileWriter();
      }
      return file_writter;
    }

    public static void setDirPath(string dirPath)
    {
      if (dirPath == null)
        throw new ArgumentException("filePath");

      if (!Directory.Exists(dirPath))
        Directory.CreateDirectory(dirPath);

      dir_path = dirPath;
    }

    public void log(string line, bool append)
    {      
      var writter = new StreamWriter(dir_path + "log.txt", append);
      writter.WriteLine("@" + DateTime.Now.ToString("dd/MM/yyyy,HH:mm") + "- " + line);
      writter.Close();

    }

    public void writeTagRel(IList<TagRelevance> tagRelevances)
    {
      using (var writter = new StreamWriter(dir_path + "tag_relevance.csv"))
      {
        foreach (var tagRel in tagRelevances)
          writter.WriteLine(string.Join(",", tagRel.movie.id, tagRel.tag.id, tagRel.relevance));
      }
    }

    public void writeMovieClassifications(string sub_dir, string file_prefix, int instance, IEnumerable<IMovieClassification> moviesClassifications)
    {
      if (sub_dir == null)
        throw new ArgumentException("sub_dir");
      if (file_prefix == null)
        throw new ArgumentException("file_prefix");
      if (moviesClassifications == null)
        throw new ArgumentException("moviesClassifications");

      if (!Directory.Exists(dir_path + sub_dir))
        Directory.CreateDirectory(dir_path + sub_dir);
      if (!Directory.Exists(dir_path + sub_dir + "\\classify"))
        Directory.CreateDirectory(dir_path + sub_dir + "\\classify");
      
      using (var writter = new StreamWriter(dir_path + sub_dir + "\\classify\\" + file_prefix + "_" + instance + ".csv"))
      {
        foreach (var movieClassification in moviesClassifications)
          writter.WriteLine(movieClassification.print());
      }
    }

    public void writeTrainedAlgorithmInfo(string sub_dir, string file_prefix, int instance, IEnumerable<string> linesToWrite)
    {
      if (sub_dir == null)
        throw new ArgumentException("sub_dir");
      if (file_prefix == null)
        throw new ArgumentException("file_prefix");
      if (linesToWrite == null)
        throw new ArgumentException("linesToWrite");

      if (!Directory.Exists(dir_path + sub_dir))
        Directory.CreateDirectory(dir_path + sub_dir);
      if (!Directory.Exists(dir_path + sub_dir + "\\train"))
        Directory.CreateDirectory(dir_path + sub_dir + "\\train");

      using (var writter = new StreamWriter(dir_path + sub_dir + "\\train\\" + file_prefix + "_" + instance + ".csv"))
      {
        foreach (var line in linesToWrite)
          writter.WriteLine(line);
      }
    }

    public void writeUserRatings(IList<Rating> ratings, int chunk)
    {
      var baseDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      if (!Directory.Exists(dir_path + "profiles"))
        Directory.CreateDirectory(dir_path + "profiles");

      using (var writter = new StreamWriter(dir_path + "profiles\\ratings_pt" + chunk + ".csv"))
      {
        foreach (var rating in ratings)
          writter.WriteLine(rating.movie.id + "," + rating.user_id + "," + rating.rating + "," + (rating.timestamp - baseDate).Duration().TotalSeconds);

      }
    }

    public void appendUserProfiles(string filename, IList<UserProfile> userProfile)
    {
      if (!Directory.Exists(dir_path + "\\profiles"))
        Directory.CreateDirectory(dir_path + "\\profiles");

      using (var writter = new StreamWriter(dir_path + "\\profiles\\" + filename, true))
      {
        foreach (var profile in userProfile)
          writter.WriteLine(profile.user_id + profile.profile.Aggregate("", (acc, n) => acc + "," + n));
        Console.WriteLine("Perfis de usuários escritos.");
      }
    }

    public void writeUserProfiles(IList<UserProfile> userProfile)
    {

      using (var writter = new StreamWriter(dir_path + "user_profile.csv"))
      {
        foreach (var profile in userProfile)
          writter.WriteLine(profile.user_id + profile.profile.Aggregate("", (acc, n) => acc + "," + n));
      }
    }
  

    public void writeRecommendationResults(string sub_dir, string file_prefix, double cutoff, string decay, string normalization, int predictNextN, int instance, 
      IList<RecommendationResults> recResults)
    {
      if (recResults == null)
        throw new ArgumentException("recResults");
      if (sub_dir == null)
        throw new ArgumentException("sub_dir");
      if (file_prefix == null)
        throw new ArgumentException("file_prefix");

      if (!Directory.Exists(dir_path + sub_dir))
        Directory.CreateDirectory(dir_path + sub_dir);
      if (!Directory.Exists(dir_path + sub_dir + "\\recommend"))
        Directory.CreateDirectory(dir_path + sub_dir + "\\recommend");
      
      using (var writter = new StreamWriter(dir_path + sub_dir + "\\recommend\\" + file_prefix + "_" + cutoff + "_" + decay + "_" + normalization + "_" + predictNextN + "_" + instance + ".csv"))
      {
        foreach (var result in recResults)
          writter.WriteLine(result.user_id + "," + result.number_of_ratings + "," + result.precision);
      }
    }


    public void writeAggregatedResults(string sub_dir, string filenameWithoutExtension, int instance, IList<AggregatedResults> aggregatedResultsQt, IList<AggregatedResults> aggregatedResultsPt,
      AggregatedResults aggregatedResultsAll)
    {
      
      if(!Directory.Exists(dir_path + "evaluate"))
        Directory.CreateDirectory(dir_path + "evaluate");
      if (!Directory.Exists(dir_path + sub_dir + "\\evaluate"))
        Directory.CreateDirectory(dir_path + sub_dir + "\\evaluate");

      foreach (var result in aggregatedResultsQt)
      {
        using (var writter = new StreamWriter(dir_path + "evaluate\\qt_" + result.min + "_" + result.max +".csv", true))
        {
          writter.WriteLine(filenameWithoutExtension + ","  + result.average + "," + result.std_dev);
        }
      }

      foreach (var result in aggregatedResultsPt)
      {
        using (var writter = new StreamWriter(dir_path + "evaluate\\pt_" + result.min + "_" + result.max + ".csv", true))
        {
          writter.WriteLine(filenameWithoutExtension + "," + result.average + "," + result.std_dev);
        }
      }

      using (var writter = new StreamWriter(dir_path + "evaluate\\all.csv", true))
      {
        writter.WriteLine(filenameWithoutExtension + "," + aggregatedResultsAll.average + "," + aggregatedResultsAll.std_dev);
      }


      using (var writter = new StreamWriter(dir_path + sub_dir + "\\evaluate\\" + filenameWithoutExtension + "_" + instance + ".csv"))
      {

        foreach (var aggRes in aggregatedResultsQt.OrderBy(ar => ar.min))
          writter.WriteLine(filenameWithoutExtension + "," + aggRes.min + "," + aggRes.max + "," + aggRes.average + "," + aggRes.std_dev);
        
        foreach (var aggRes in aggregatedResultsPt.OrderBy(ar => ar.min))
          writter.WriteLine(filenameWithoutExtension + "," + aggRes.min + "," + aggRes.max + "," + aggRes.average + "," + aggRes.std_dev);


      }

    }

    //public void writeRatings(IList<Rating> ratings, int chunk)
    //{
    //  using (var writter = new StreamWriter(dir_path + "ratings_pt" + chunk + ".csv"))
    //  {
    //    foreach (var rating in ratings)
    //      writter.WriteLine(rating.movie.id + "," + rating.user_id + "," + rating.rating + "," + (rating.timestamp - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
    //  }
    //}

  }
}
