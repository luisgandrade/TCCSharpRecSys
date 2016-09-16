﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;
using Utils.Metric;
using Utils;

namespace UnsupervisedLearning.KMeans
{
  /// <summary>
  /// Algoritmo K-Means padrão. Essa implementação usa o método de Forgy para inicialização dos clusters, onde
  /// são escolhidas k observações aleatórias do dataset e essas são usadas como os centróides iniciais dos clusters.
  /// </summary>
  public class StandardKMeans : IUnsupervisedLearning
  {

    public int cluster_count { get; private set; }
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
        throw new NotImplementedException();
      }
    }

    public string sub_dir
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string file_prefix
    {
      get
      {
        throw new NotImplementedException();
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
      if (instances[0].tag_relevances.Count != clusters[0].centroid.Count)
        throw new InvalidOperationException("A quantidade de atributos das instâncias não bate com a quantidade de atributos dos clusters já definidos.");

      instances.Shuffle();
      if (!clusters.Any())
        clusters = instances.Take(cluster_count).Select((im, index) => new Cluster(index, im.tag_relevances.Select(tr => tr.relevance).ToList())).ToList();

      var stop = false;
      var iteration = 1;
      while (!stop)
      {
        Console.WriteLine("Iteration " + iteration);
        //assignment step
        var newInstancesClustered = instances.GroupJoin(clusters, ti => true, c => true,
          (ti, c) => new InstanceClustered(ti, c.WhereMin(cl => EuclidianDistance.distance(ti.tag_relevances.Select(tr => tr.relevance).ToList(), cl.centroid)))).ToList();

        //update step
        clusters = clusters.Join(instances_clustered.GroupBy(ic => ic.cluster), cl => cl, ic => ic.Key, (cl, ic) => new Cluster(cl.id,
          ic.SelectMany(ic1 => ic1.instance.tag_relevances).GroupBy(ic1 => ic1.tag).OrderBy(ic1 => ic1.Key.id).Select(ic1 => ic1.Average(ic2 => ic2.relevance)).ToList())).ToList();

        //retorna verdadeiro se nenhuma instância mudou de cluster na iteração atual
        stop = instances_clustered != null && newInstancesClustered.Join(instances_clustered, nic => nic.instance, ic => ic.instance, (nic, ic) => nic.cluster == ic.cluster).All(ic => ic);
        instances_clustered = newInstancesClustered;
          
        iteration++;
      }
    }

    public IList<IMovieClassification> classify_instances(IList<Instance> tagRelevances)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<string> printClassifier()
    {
      throw new NotImplementedException();
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
    public StandardKMeans(int clusterCount)
    {
      clusters = new List<Cluster>();
      cluster_count = clusterCount;
    }
  }
}
