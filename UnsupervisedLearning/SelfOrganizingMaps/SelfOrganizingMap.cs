using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions;

namespace UnsupervisedLearning.SelfOrganizingMaps
{
  /// <summary>
  /// Implementação do algoritmo de mapas auto-organizáveis.
  /// </summary>
  public class SelfOrganizingMap : IClustering
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

    public SelfOrganizingMap(int rows, int columns, int attr_count, INeighborhoodFunction neighborhoodFunction, ILearningRateFunction learningRateFunction, bool useNormalizedValues = true)
    {
      if (neighborhoodFunction == null)
        throw new ArgumentException("neighborhoodFunction");
      if (learningRateFunction == null)
        throw new ArgumentException("learningRateFunction");

      network = new Network.Network(rows, columns, attr_count);
      neighborhood_function = neighborhoodFunction;
      learning_rate_function = learningRateFunction;
      maxIterations = 1000 + rows * columns * 500;
      this.useNormalizedValues = useNormalizedValues;
    }

    public SelfOrganizingMap(int rows, int columns, int attr_count, int instance, IEnumerable<Tuple<int, int, List<double>>> existingNeurons, bool useNormalizedValues = true)
    {
      this.network = new Network.Network(rows, columns, attr_count, instance, existingNeurons);
      this.useNormalizedValues = useNormalizedValues;
    }

    public bool iterate(IList<MovieAttribute> instanceAttributes, int iteration)
    {      
      if (useNormalizedValues && instanceAttributes.Any(ia => !ia.normalized_value.HasValue))
        throw new InvalidOperationException("Algoritmo está setado para usar valores normalizados, porém. algum valor normalizado não foi informado.");
      IList<double> values = null;
      if (useNormalizedValues)
        values = instanceAttributes.Select(ia => ia.normalized_value.Value).ToList();
      else
        values = instanceAttributes.Select(ia => ia.value).ToList();

      var bmu = network.getBMU(values);
      network.updateNetwork(bmu, neighborhood_function, learning_rate_function, values, iteration);

      return iteration >= maxIterations;
    }   
    
    public SOMClassification getClassifier()
    {
      return new SOMClassification(new Network.Network(network), useNormalizedValues);
    }    

    public IEnumerable<string> printNetwork()
    {
      var header = network.neurons.Length + "," + network.neurons[0].Length + "," + (useNormalizedValues ? "t" : "f");
      return new[] { header }.Concat(network.printNetwork());
    }
    
  }
}
