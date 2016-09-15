using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.Network;
using Utils;
using Utils.Metric;

namespace UnsupervisedLearning.SelfOrganizingMaps
{
  /// <summary>
  /// Implementação do algoritmo de mapas auto-organizáveis.
  /// </summary>
  public class SelfOrganizingMap : IUnsupervisedLearning
  {
    private Network.Network network;
    private INeighborhoodFunction neighborhood_function;
    private ILearningRateFunction learning_rate_function;
    public int maxIterations;
    private bool useNormalizedValues;

    public int rows
    {
      get
      {
        return network.rows;
      }
    }

    public int columns
    {
      get
      {
        return network.columns;
      }
    }

    public int attr_count
    {
      get
      {
        return network.attr_count;
      }
    }

    public int instance
    {
      get
      {
        return network.instance;
      }
    }

    public SelfOrganizingMap(int rows, int columns, int attr_count, INeighborhoodFunction neighborhoodFunction, ILearningRateFunction learningRateFunction, IMetric metric,
      bool useNormalizedValues = false)
    {
      if (neighborhoodFunction == null)
        throw new ArgumentException("neighborhoodFunction");
      if (learningRateFunction == null)
        throw new ArgumentException("learningRateFunction");

      network = new Network.Network(rows, columns, attr_count, metric);
      neighborhood_function = neighborhoodFunction;
      learning_rate_function = learningRateFunction;
      maxIterations = 1000 + rows * columns * 500;
      this.useNormalizedValues = useNormalizedValues;
    }

    public SelfOrganizingMap(int rows, int columns, int attr_count, int instance, IMetric metric, IEnumerable<Tuple<int, int, List<double>>> existingNeurons, 
      bool useNormalizedValues = true)
    {
      this.network = new Network.Network(rows, columns, attr_count, instance, metric, existingNeurons);
      this.useNormalizedValues = useNormalizedValues;
    }

    private bool iterate(IList<TagRelevance> instanceAttributes, int iteration)
    {      
      if (useNormalizedValues && instanceAttributes.Any(ia => !ia.normalized_relevance.HasValue))
        throw new InvalidOperationException("Algoritmo está setado para usar valores normalizados, porém. algum valor normalizado não foi informado.");
      IList<double> values = null;
      if (useNormalizedValues)
        values = instanceAttributes.Select(ia => ia.normalized_relevance.Value).ToList();
      else
        values = instanceAttributes.Select(ia => ia.relevance).ToList();

      var bmu = network.getBMU(values);
      network.updateNetwork(bmu, neighborhood_function, learning_rate_function, values, iteration);

      return iteration >= maxIterations;
    }   
    
    public Neuron getNeuron(int x, int y)
    {
      return network.neurons[x][y];
    } 

    public IEnumerable<string> printNetwork()
    {
      var header = network.neurons.Length + "," + network.neurons[0].Length + "," + (useNormalizedValues ? "t" : "f");
      return new[] { header }.Concat(network.printNetwork());
    }

    public IList<IMovieClassification> classify_instances<Neuron>(IList<Instance> instances)
    {
      return instances.Select(i => new MovieSOMClassification(i.movie,
        network.classifyInstance(i.tag_relevances.OrderBy(tr => tr.tag.id).Select(tr => useNormalizedValues ? tr.normalized_relevance.Value : tr.relevance).ToList()).First()))
                      .Cast<IMovieClassification>().ToList();
    }

    public void train(IList<Instance> instances)
    {
      if (instances == null)
        throw new ArgumentException("instances");

      var iteration = 0;
      var stop = false;
      while (!stop)
      {
        if (iteration % instances.Count == 0)
          instances.Shuffle();

        stop = iterate(instances[iteration % instances.Count].tag_relevances, iteration);
      }      
    }

    public IEnumerable<string> printClassifier()
    {
      return network.printNetwork();
    }

    public IList<IMovieClassification> classify_instances(IList<Instance> tagRelevances)
    {
      throw new NotImplementedException();
    }
  }
}
