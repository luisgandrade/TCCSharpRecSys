using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnsupervisedLearning;
using UnsupervisedLearning.SelfOrganizingMaps;

namespace TCCSharpRecSys.Persistence
{
  public class FileWritter
  {

    private string filesPath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory).Replace("\\TCCSharpRecSys\\bin\\Debug", "") + "\\Data\\";

    public void writeAttributes(IList<Attr> attributes)
    {
      var sb = new StringBuilder();
      using(var writer = new StreamWriter(filesPath + "attrs_" + attributes.Count + ".csv"))
      {
        foreach (var attribute in attributes)
          writer.WriteLine(attribute.attr_index + "," + attribute.attribute + "," + attribute.population_average + "," + attribute.population_standard_deviation);
      }
      
    }


    public void writeMovieAttributes(IList<MovieAttribute> movieAttrs, int attr_count)
    {

      using (var writer = new StreamWriter(filesPath + "movie_attr_" + attr_count + ".csv"))
      {
        foreach (var movieAttr in movieAttrs)
          writer.WriteLine(movieAttr.movie_id + "," + movieAttr.attribute_id + "," + movieAttr.value + "," +
                                (movieAttr.normalized_value.HasValue ? movieAttr.normalized_value.Value.ToString() : "null"));
      }
    }
    
    public void writeSOMClassification(SOMClassification som, IEnumerable<InstanceClassification> moviesAndNodes)
    {
      if (!Directory.Exists(filesPath + "movie_classification"))
        Directory.CreateDirectory(filesPath + "movie_classification");
      using (var writter = new StreamWriter(filesPath + "movie_classification\\" + som.attr_count + "_" + som.rows + "_" + som.columns + "_" + (som.instance) + ".csv"))
      {
        foreach (var movie in moviesAndNodes)
          writter.WriteLine(movie.instance_id + "," + movie.x + "," + movie.y);
      }
    }

    public void writeSOMNodes(SelfOrganizingMap somAlg)
    {
      if (!Directory.Exists(filesPath + "som_nodes"))
        Directory.CreateDirectory(filesPath + "som_nodes");
      var existingFilesInDirectory = Directory.GetFiles(filesPath + "som_nodes\\");
      var regex = new Regex(somAlg.attr_count + "_" + somAlg.rows + "_" + somAlg.columns + "_([0-9]*).csv");
      var maxInstance = existingFilesInDirectory.Where(f => regex.IsMatch(f)).Select(f => int.Parse(regex.Match(f).Groups[1].Value)).DefaultIfEmpty(0).Max();

      using (var writter = new StreamWriter(filesPath + "som_nodes\\" + somAlg.attr_count + "_" + somAlg.rows + "_" + somAlg.columns + "_" + (maxInstance + 1) + ".csv"))
      {
        foreach (var line in somAlg.printNetwork())
          writter.WriteLine(line);
      }
    }

    public void writeSOMLabels(SOMClassification som, IList<NodeLabel> nodeLabels)
    {
      var labelCount = nodeLabels[0].labels.Count;
      if (!Directory.Exists(filesPath + "som_labels"))
        Directory.CreateDirectory(filesPath + "som_labels");
      using (var writter = new StreamWriter(filesPath + "som_labels\\" + som.attr_count + "_" + som.rows + "_" + som.columns + "_" + (som.instance) + "_" + labelCount + ".csv"))
      {
        foreach (var node in nodeLabels)
          writter.WriteLine("[" + node.x + ";" + node.y + "],[" + node.labels.Aggregate((acc, l) => acc + ";" + l) +"]");
      }
    }
  }
}
