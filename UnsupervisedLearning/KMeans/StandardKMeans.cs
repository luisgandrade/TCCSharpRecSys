using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;
using Utils.Metric;
using Utils;
using UnsupervisedLearning;

namespace UnsupervisedLearning.KMeans
{
  /// <summary>
  /// Algoritmo K-Means padrão. Essa implementação usa o método de Forgy para inicialização dos clusters, onde
  /// são escolhidas k observações aleatórias do dataset e essas são usadas como os centróides iniciais dos clusters.
  /// </summary>
  public class StandardKMeans : IUnsupervisedLearning
  {

    public int cluster_count { get; private set; }

    public bool use_normalized_values { get; private set; }
    /// <summary>
    /// Clusters gerados.
    /// </summary>
    public IList<Cluster> clusters { get; private set; }


    /// <summary>
    /// Instâncias associadas a um dos clusters existentes.
    /// </summary>
    internal IList<InstanceClustered> instances_clustered { get; private set; }

    public string name
    {
      get
      {
        return "K-Means";
      }
    }

    public string sub_dir
    {
      get
      {
        return "kmeans";
      }
    }

    public string file_prefix
    {
      get
      {
        return cluster_count + "_" + (use_normalized_values ? "t" : "f");
      }
    }

    public void train(IList<Instance> instances)
    {
      if (instances == null)
        throw new ArgumentException("trainingInstances");
      if (!instances.Any())
        throw new InvalidOperationException("Não há nenhuma instância para ser treinada.");
      if (instances.Select(ti => ti.tag_relevances.Count).Distinct().Count() > 1)
        throw new InvalidOperationException("Existem instâncias com vetores de atributos de tamanhos diferentes.");
      if (clusters.Any() && instances[0].tag_relevances.Count != clusters[0].centroid.Count)
        throw new InvalidOperationException("A quantidade de atributos das instâncias não bate com a quantidade de atributos dos clusters já definidos.");

      instances.Shuffle();
      if (!clusters.Any())
        clusters = instances.Take(cluster_count).Select((im, index) => new Cluster(index, im.getRelevances(use_normalized_values))).ToList();
      
      var continueTraining = true;
      var iteration = 1;
      do
      {
        Console.WriteLine("Iteration " + iteration);
        iteration++;

        //assignment step
        var newInstancesClustered = instances.GroupJoin(clusters, ti => true, c => true,
          (ti, c) => new InstanceClustered(ti, c.WhereMin(cl => EuclidianDistance.distance(ti.tag_relevances.Select(tr => tr.relevance).ToList(), cl.centroid)))).ToList();

        //update step
        clusters = clusters.Join(newInstancesClustered.GroupBy(ic => ic.cluster), cl => cl, ic => ic.Key, (cl, ic) => new Cluster(cl.id,
          ic.SelectMany(ic1 => ic1.instance.tag_relevances).GroupBy(ic1 => ic1.tag).OrderBy(ic1 => ic1.Key.id).Select(ic1 => ic1.Average(ic2 => ic2.relevance)).ToList())).ToList();

        if (instances_clustered != null)
        {
          var instancesThatChanged = newInstancesClustered.Join(instances_clustered, nic => nic.instance, ic => ic.instance, (nic, ic) => nic.cluster.id == ic.cluster.id).Count(c => !c);
          Console.WriteLine("Quantidade de instâncias que mudaram de cluster: " + instancesThatChanged);
          continueTraining = instancesThatChanged > 0;
        }

        instances_clustered = newInstancesClustered;

      } while (continueTraining);      
    }

    public IEnumerable<IMovieClassification> classify_instances(IList<Instance> tagRelevances)
    {
      if (tagRelevances == null)
        throw new ArgumentException("tagRelevances");
      
      var moviesClassification = new List<IMovieClassification>();

      foreach (var instance in tagRelevances)
      {
        var bestFitCluster = clusters.WhereMin(cl => EuclidianDistance.distance(instance.getRelevances(use_normalized_values), cl.centroid));
        moviesClassification.Add(new KMeansMovieClassification(instance.movie, bestFitCluster));
      }

      return moviesClassification;
    }

    public IEnumerable<string> printClassifier()
    {
      return clusters.Select(cl => cl.id + "," + string.Join(",", cl.centroid.Select(c => c.ToString()).ToList()));
    }

    public IEnumerable<IClassLabel> best_matching_units(UserProfile userProfile)
    {
      return clusters.OrderBy(cl => EuclidianDistance.distance(userProfile.profile, cl.centroid)).Cast<IClassLabel>().ToList();
    }

    public Func<Movie, string, IMovieClassification> parse_movie_classification()
    {
      return (movie, str) =>
      {
        var clusterId = int.Parse(str);
        return new KMeansMovieClassification(movie, clusters.Single(cl => cl.id == clusterId));
      };
      
    }

    public void parse_classifier(IList<string> classLabelConfig)
    {
      var clusters = new List<Cluster>();
      foreach (var clusterConfig in classLabelConfig)
      {
        var split = clusterConfig.Split(',');
        var clusterId = int.Parse(split[0]);
        var means = split.Skip(1).Select(s => double.Parse(s)).ToList();
        clusters.Add(new Cluster(clusterId, means));
      }
      this.clusters = clusters;
    }

    /// <param name="initialMeans">instâncias do dataset serão usadas para criação dos clusters</param>
    public StandardKMeans(IList<Instance> initialMeans)
    {
      if (initialMeans == null)
        throw new ArgumentException("initialMeans");

      clusters = initialMeans.Select((im, index) => new Cluster(index, im.tag_relevances.Select(tr => tr.relevance).ToList())).ToList();
      cluster_count = initialMeans.Count;
    }

    /// <param name="initialMeans">instâncias do dataset serão usadas para criação dos clusters</param>
    public StandardKMeans(int clusterCount, bool useNormalizedValues)
    {
      clusters = new List<Cluster>();
      cluster_count = clusterCount;
      use_normalized_values = useNormalizedValues;
    }
  }
}
