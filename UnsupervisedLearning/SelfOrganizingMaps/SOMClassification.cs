using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.SelfOrganizingMaps
{
  public class SOMClassification
  {

    internal Network.Network network { get; private set; }

    private bool normalized_values;

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

    public SOMClassification(int rows, int columns, int attr_count, int instance, IList<Tuple<int, int, List<double>>> neurons, bool normalizedValues)
    {
      if (neurons == null)
        throw new ArgumentException("neurons");
      if (rows <= 0 || columns <= 0 || attr_count <= 0 || instance <= 0)
        throw new InvalidOperationException("rows <= 0 || columns <= 0 || attr_count <= 0 || instance <= 0");
      if (neurons.Any(n => n.Item3.Count != attr_count))
        throw new InvalidOperationException("Pelo menos um dos nós tem lista de pesos diferente do número de atributos informados.");
      if (neurons.Count != rows * columns)
        throw new InvalidOperationException("Número de neurônios informados não bate com o número de linhas e colunas.");

      this.network = new Network.Network(rows, columns, attr_count, instance, neurons);
      this.normalized_values = normalizedValues;
    }

    internal SOMClassification(Network.Network network, bool normalizedValues)
    {
      this.network = network;
      this.normalized_values = normalizedValues;
    }

    public IList<InstanceClassification> classifyInstances(IList<KeyValuePair<int, List<MovieAttribute>>> instancesAttributes)
    {
      if (normalized_values && instancesAttributes.Any(ia => ia.Value.Any(iia => !iia.normalized_value.HasValue)))
        throw new InvalidOperationException("Normalized values setada mas algum dos atributos tem valor normalizado nulo.");
      return instancesAttributes.Select(ia => new
      {
        movie_id = ia.Key,
        node = network.classifyInstance(ia.Value.OrderBy(iia => iia.attribute_id).Select(iia => normalized_values ? iia.normalized_value.Value : iia.value).ToList())
      }).Select(ia => new InstanceClassification(ia.movie_id, ia.node.Item1, ia.node.Item2)).ToList();
    }


    public IList<NodeLabel> bestFitAttributes(IList<KeyValuePair<Attr, double>> referenceValues, int returnCount)
    {
      if (referenceValues == null)
        throw new ArgumentException("referenceValues");
      if (returnCount <= 0)
        throw new InvalidOperationException("Deve ser escolhido retornar pelo menos um atributo!");
      if (referenceValues.Count != network.attr_count)
        throw new InvalidOperationException("Número de atributos não bate!");
      var nodeLabels = new List<NodeLabel>();

      foreach (var neuronArray in network.neurons)
      {
        foreach (var neuron in neuronArray)
        {
          var attributes = neuron.weights.Select((w, index) => new { index = index + 1, weight = w }).Join(referenceValues, w => w.index, rv => rv.Key.attr_index,
            (w, rv) => new { distance = w.weight * rv.Key.population_standard_deviation + rv.Key.population_average, attr = rv.Key }).OrderByDescending(da => da.distance).Select(da => da.attr.attribute).Take(returnCount);

          nodeLabels.Add(new NodeLabel(neuron.x, neuron.y, attributes.ToList()));
        }
      }
      return nodeLabels;
    }
  }
}
