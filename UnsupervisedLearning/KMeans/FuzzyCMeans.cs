using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lib;
using Utils.Metric;
using Utils;

namespace UnsupervisedLearning.KMeans
{
  //public class FuzzyCMeans : IUnsupervisedLearning
  //{
  //  /// <summary>
  //  /// Clusters gerados.
  //  /// </summary>
  //  public IList<Cluster> clusters { get; private set; }
  //  /// <summary>
  //  /// Instâncias clusterizadas.
  //  /// </summary>
  //  internal IList<InstanceFuzzyClustered> instances_fuzzy_clustered { get; private set; }
  //  /// <summary>
  //  /// Número de clusters
  //  /// </summary>
  //  internal int n_clusters { get; private set; }
  //  /// <summary>
  //  /// Coeficiente de fuzzyness. Indica o quanto clusters podem se sobrepor um ao outro. Quando maior essa propriedade for,
  //  /// mais sobreposição entre os clusters acontecerá.
  //  /// </summary>
  //  internal double fuzzyness { get; private set; }
  //  /// <summary>
  //  /// Critério de parada.
  //  /// </summary>
  //  internal double stop_criterion { get; private set; }

  //  internal bool use_normalized_values { get; private set; }

  //  public string sub_dir
  //  {
  //    get
  //    {
  //      return "cmeans";
  //    }
  //  }

  //  public string file_prefix
  //  {
  //    get
  //    {
  //      return n_clusters + "_" + fuzzyness + "_" + stop_criterion + (use_normalized_values ? "t" : "f");
  //    }
  //  }

  //  public string name
  //  {
  //    get
  //    {
  //      return "Fuzzy C-Means";
  //    }
  //  }

  //  public void train(IList<Instance> instances)
  //  {
  //    if (instances == null)
  //      throw new ArgumentException("instances");

  //    var random = new Random(DateTime.Now.Millisecond);

  //    clusters = Enumerable.Range(1, n_clusters).Select(r => new Cluster(r)).ToList();

  //    instances_fuzzy_clustered = instances.GroupJoin(clusters, i => true, c => true, (i, c) => c.Select(c1 => new InstanceFuzzyClustered(i, c1, random.NextDouble())))
  //                                         .SelectMany(x => x)
  //                                         .ToList();
  //    var stop = false;
  //    var iteration = 1;
  //    while (!stop)
  //    {
  //      Console.WriteLine("Iteration " + iteration);
  //      iteration++;
  //      clusters = instances_fuzzy_clustered.GroupBy(ifc => ifc.cluster)
  //                                          .Select(ifc => new Cluster(ifc.Key.id, ifc.SelectMany(i => i.instance.tag_relevances.Select(tr => new
  //                                          {
  //                                            degree = i.degree_of_pertinence,
  //                                            tagRel = tr
  //                                          }))
  //                                                                                    .GroupBy(dtr => dtr.tagRel.tag)
  //                                                                                    .OrderBy(dtr => dtr.Key.id)
  //                                                                                    .Select(dtr => dtr.Sum(dt => dt.degree * dt.tagRel.relevance) / dtr.Sum(dt => dt.degree))
  //                                                                                    .ToList()))
  //                                          .ToList();

  //      var newInstancesClustered = instances_fuzzy_clustered.Select(ifc => new
  //      {
  //        instance = ifc.instance,
  //        relevances = ifc.instance.tag_relevances.OrderBy(tr => tr.tag.id).Select(tr => tr.relevance).ToList()
  //      }).GroupJoin(clusters, i => true, cl => true, (i, cl) => cl.Select(cl1 => new
  //        {
  //          instance = i.instance,
  //          relevances = i.relevances,
  //          cluster = cl1,
  //          similarity = EuclidianDistance.distance(i.relevances, cl1.centroid)
  //        }))
  //        .SelectMany(ics => ics.Select(ics1 => new
  //        {
  //          ics = ics1,
  //          degree = 1 / Math.Pow(
  //                        (clusters.Sum(cl => ics1.similarity / (EuclidianDistance.distance(ics1.relevances, cl.centroid)))),
  //                        2 / (fuzzyness - 1))
  //        }))
  //        .Select(icsd => new InstanceFuzzyClustered(icsd.ics.instance, icsd.ics.cluster, icsd.degree))
  //        .ToList();

  //      stop = instances_fuzzy_clustered.Join(newInstancesClustered, ifc => new { instance = ifc.instance, cluster = ifc.cluster },
  //                                                                   nic => new { instance = nic.instance, cluster = nic.cluster },
  //                                                                   (ifc, nic) => nic.degree_of_pertinence *
  //                                                                      EuclidianDistance.distance(nic.instance.tag_relevances.OrderBy(tr => tr.tag.id)
  //                                                                                                                            .Select(tr => tr.relevance).ToList(),
  //                                                                                                 nic.cluster.centroid))
  //                                      .Max() < stop_criterion;
  //      instances_fuzzy_clustered = newInstancesClustered;
  //    }
  //  }

  //  public IList<IMovieClassification> classify_instances(IList<Instance> tagRelevances)
  //  {
  //    if (tagRelevances == null)
  //      throw new ArgumentException("tagRelevances");

  //    var moviesClassification = new List<IMovieClassification>();

  //    foreach (var instance in tagRelevances)
  //    {
  //      var bestFitCluster = clusters.WhereMin(cl => EuclidianDistance.distance(instance.getRelevances(use_normalized_values), cl.centroid));
  //      moviesClassification.Add(new KMeansMovieClassification(instance.movie, bestFitCluster));
  //    }

  //    return moviesClassification;
  //  }

  //  public IEnumerable<string> printClassifier()
  //  {
  //    return Enumerable.Empty<string>();
  //  }

  //  IEnumerable<IMovieClassification> IUnsupervisedLearning.classify_instances(IList<Instance> instances)
  //  {
  //    throw new NotImplementedException();
  //  }

  //  public IEnumerable<IClassLabel> best_matching_units(UserProfile userProfile)
  //  {
  //    throw new NotImplementedException();
  //  }

  //  public Func<Movie, string, IMovieClassification> parse_movie_classification()
  //  {
  //    throw new NotImplementedException();
  //  }

  //  public void parse_classifier(IList<string> classLabelConfig)
  //  {
  //    throw new NotImplementedException();
  //  }

  //  public FuzzyCMeans(int n_clusters, double fuzzyness, double stop_criterion, bool useNormalizedValues)
  //  {
  //    if (n_clusters < 2)
  //      throw new ArgumentOutOfRangeException("Número de clusters deve ser maior que 2.");
  //    if (fuzzyness <= 1)
  //      throw new ArgumentOutOfRangeException("Coeficiente de fuzzyness deve ser maior que 1.");
  //    if (stop_criterion < 0 || stop_criterion > 1)
  //      throw new ArgumentOutOfRangeException("Critério de parada de estar entre zero e um.");

  //    this.n_clusters = n_clusters;
  //    this.fuzzyness = fuzzyness;
  //    this.stop_criterion = stop_criterion;
  //  }

  //}
}
