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

    private static string file_path;

    private static FileWritter file_writter;

    private FileWritter()
    {      
    }

    public static FileWritter getInstance()
    {
      if(file_writter == null)
      {
        if (file_path == null)
          throw new InvalidOperationException("Antes de instanciar o escritor de arquivos é necessário setar o caminho absolutos dos arquivos.");
        file_writter = new FileWritter();
      }
      return file_writter;        
    }

    public static void setFilePath(string filePath)
    {
      if (filePath == null)
        throw new ArgumentException("filePath");
      if (!Directory.Exists(filePath))
        throw new InvalidOperationException("Diretório não existe.");
      file_path = filePath;
    }

    public void log(string line)
    {
      using(var writter = new StreamWriter(file_path + "log.txt", true))
      {
        writter.WriteLine("@ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "- " + line);
      }
    }

    public void writeAttributes(IList<Tag> attributes)
    {
      var sb = new StringBuilder();
      using(var writer = new StreamWriter(file_path + "attrs_" + attributes.Count + ".csv"))
      {
        foreach (var attribute in attributes)
          writer.WriteLine(attribute.id + "," + attribute.tag + "," + attribute.population_average + "," + attribute.population_standard_deviation);
      }
      
    }


    public void writeMovieAttributes(IList<TagRelevance> movieAttrs, int attr_count)
    {

      using (var writer = new StreamWriter(file_path + "movie_attr_" + attr_count + ".csv"))
      {
        foreach (var movieAttr in movieAttrs)
          writer.WriteLine(movieAttr.movie.id + "," + movieAttr.tag.id + "," + movieAttr.relevance + "," +
                                (movieAttr.normalized_relevance.HasValue ? movieAttr.normalized_relevance.Value.ToString() : "null"));
      }
    }
    
    public void writeSOMClassification(SOMClassification som, IEnumerable<InstanceClassification> moviesAndNodes)
    {
      if (!Directory.Exists(file_path + "movie_classification"))
        Directory.CreateDirectory(file_path + "movie_classification");
      using (var writter = new StreamWriter(file_path + "movie_classification\\" + som.attr_count + "_" + som.rows + "_" + som.columns + "_" + (som.instance) + ".csv"))
      {
        foreach (var movie in moviesAndNodes)
          writter.WriteLine(movie.instance_id + "," + movie.x + "," + movie.y);
      }
    }

    public string writeSOMNodes(SelfOrganizingMap somAlg, string filePrefix)
    {
      if (!Directory.Exists(file_path + "som_nodes"))
        Directory.CreateDirectory(file_path + "som_nodes");
      var existingFilesInDirectory = Directory.GetFiles(file_path + "som_nodes\\");
      var regex = new Regex(filePrefix + "_([0-9]*).csv");
      var maxInstance = existingFilesInDirectory.Where(f => regex.IsMatch(f)).Select(f => int.Parse(regex.Match(f).Groups[1].Value)).DefaultIfEmpty(0).Max();

      using (var writter = new StreamWriter(file_path + "som_nodes\\" + filePrefix + (maxInstance + 1) + ".csv"))
      {
        foreach (var line in somAlg.printNetwork())
          writter.WriteLine(line);
      }

      return filePrefix + (maxInstance + 1) + ".csv";
    }

    public void writeSOMLabels(SOMClassification som, IList<NodeLabel> nodeLabels)
    {
      var labelCount = nodeLabels[0].labels.Count;
      if (!Directory.Exists(file_path + "som_labels"))
        Directory.CreateDirectory(file_path + "som_labels");
      using (var writter = new StreamWriter(file_path + "som_labels\\" + som.attr_count + "_" + som.rows + "_" + som.columns + "_" + (som.instance) + "_" + labelCount + ".csv"))
      {
        foreach (var node in nodeLabels)
          writter.WriteLine("[" + node.x + ";" + node.y + "],[" + node.labels.Aggregate((acc, l) => acc + ";" + l) +"]");
      }
    }

    public void writeUserRatings(IList<KeyValuePair<int, IList<KeyValuePair<int, double>>>> userProfile, int attr_count)
    {
      using (var writter = new StreamWriter(file_path + "user_profile_" + attr_count + ".csv"))
      {
        foreach (var user in userProfile)
          foreach (var attribute in user.Value)
            writter.WriteLine(user.Key + "," + attribute.Key + "," + attribute.Value);
      }
    }

    public void appendUserProfiles(string filename, IList<UserProfile> userProfile)
    {
      using (var writter = new StreamWriter(file_path + filename, true))
      {
        foreach (var profile in userProfile)
          writter.WriteLine(profile.user_id + profile.profile.Aggregate("", (acc, n) => acc + "," + n));
        Console.WriteLine("Perfis de usuários escritos.");
      }
      
    }

    public void writeUserProfiles(IList<UserProfile> userProfile)
    {
      using (var writter = new StreamWriter(file_path + "user_profile.csv"))
      {
        foreach (var profile in userProfile)
          writter.WriteLine(profile.user_id + profile.profile.Aggregate("",(acc, n) => acc + "," + n));
      }
    }
    
    public void writeClusters(IList<Cluster> clusters)
    {
      if (!Directory.Exists(file_path + "standard_k_means"))
        Directory.CreateDirectory(file_path + "standard_k_means");
      var existingFilesInDirectory = Directory.GetFiles(file_path + "standard_k_means\\");
      var regex = new Regex(clusters.Count + "_([0-9]*).csv");
      var maxInstance = existingFilesInDirectory.Where(f => regex.IsMatch(f)).Select(f => int.Parse(regex.Match(f).Groups[1].Value)).DefaultIfEmpty(0).Max();

      using (var writter = new StreamWriter(file_path + "standard_k_means\\" + clusters.Count + "_" + (maxInstance + 1) + ".csv"))
      {
        foreach (var cluster in clusters)
          writter.WriteLine(cluster.id + "," + cluster.centroid.Aggregate("", (acc, n) => acc + "," + n));
      }
    }

    // user_id, filmes recomendados com match, numero de filmes para treino
    public void writePrecision(IList<Tuple<int, int, int>> recomendacoesParUsuario)
    {
      using (var writter = new StreamWriter(file_path + "recommendations.csv"))
      {
        foreach (var recommendation in recomendacoesParUsuario)
            writter.WriteLine(recommendation.Item1 + "," + recommendation.Item2 + "," + recommendation.Item3);
      }
    }


  }
}
