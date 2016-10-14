using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnsupervisedLearning;
using UnsupervisedLearning.KMeans;
using UnsupervisedLearning.SelfOrganizingMaps;

namespace TCCSharpRecSys.Persistence
{
  public class FileWritter
  {

    private static string dir_path;

    private static FileWritter file_writter;

    private FileWritter()
    {      
    }

    public static FileWritter getInstance()
    {
      if(file_writter == null)
      {
        if (dir_path == null)
          throw new InvalidOperationException("Antes de instanciar o escritor de arquivos é necessário setar o caminho absolutos dos arquivos.");
        file_writter = new FileWritter();
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
      using(var writter = new StreamWriter(dir_path + "log.txt", append))
      {
        writter.WriteLine("@" + DateTime.Now.ToString("dd/MM/yyyy,HH:mm") + "- " + line);
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
      if(!Directory.Exists(dir_path + sub_dir + "\\train"))
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
      using (var writter = new StreamWriter(dir_path + filename, true))
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
          writter.WriteLine(profile.user_id + profile.profile.Aggregate("",(acc, n) => acc + "," + n));
      }
    }
    
    //public void writeClusters(IList<Cluster> clusters)
    //{
    //  if (!Directory.Exists(file_path + "standard_k_means"))
    //    Directory.CreateDirectory(file_path + "standard_k_means");
    //  var existingFilesInDirectory = Directory.GetFiles(file_path + "standard_k_means\\");
    //  var regex = new Regex(clusters.Count + "_([0-9]*).csv");
    //  var maxInstance = existingFilesInDirectory.Where(f => regex.IsMatch(f)).Select(f => int.Parse(regex.Match(f).Groups[1].Value)).DefaultIfEmpty(0).Max();

    //  using (var writter = new StreamWriter(file_path + "standard_k_means\\" + clusters.Count + "_" + (maxInstance + 1) + ".csv"))
    //  {
    //    foreach (var cluster in clusters)
    //      writter.WriteLine(cluster.id + "," + cluster.centroid.Aggregate("", (acc, n) => acc + "," + n));
    //  }
    //}

    // user_id, filmes recomendados com match, numero de filmes para treino
    public void writePrecision(IList<Tuple<int, int, int>> recomendacoesParUsuario)
    {
      using (var writter = new StreamWriter(dir_path + "recommendations.csv"))
      {
        foreach (var recommendation in recomendacoesParUsuario)
            writter.WriteLine(recommendation.Item1 + "," + recommendation.Item2 + "," + recommendation.Item3);
      }
    }


  }
}
