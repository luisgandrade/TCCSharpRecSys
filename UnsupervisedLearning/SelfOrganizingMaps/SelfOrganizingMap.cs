using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    private IMetric metric;
    public int maxIterations;

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

    public string name
    {
      get
      {
        return "SOM";
      }
    }

    public string sub_dir
    {
      get
      {
        return "som";
      }
    }

    public string file_prefix
    {
      get
      {
        return rows + "_" + columns + "_" + metric.print + "_" + neighborhood_function.print + "_" + learning_rate_function.print + "_" + attr_count;
      }
    }
    
    private bool iterate(IList<TagRelevance> instanceAttributes, int iteration)
    { 
      var values = instanceAttributes.Select(ia => ia.relevance).ToList();

      var bmu = network.getBMU(values);
      network.updateNetwork(bmu, neighborhood_function, learning_rate_function, values, iteration);

      return iteration >= maxIterations;
    }   
    
    public Neuron getNeuron(int x, int y)
    {
      return network.neurons[x][y];
    } 

    public IEnumerable<IMovieClassification> classify_instances(IList<Instance> instances, int number_of_attributes)
    {
      return instances.Select(i => new SOMMovieClassification(i.movie,
        network.classifyInstance(i.tag_relevances.Select(tr => tr.relevance).Select((tr, index) => new KeyValuePair<int, double>(index, tr))
                                                                     .OrderByDescending(tr => tr.Value).Take(number_of_attributes).ToList()).First()))
                      .Cast<IMovieClassification>().ToList();
    }

    public IEnumerable<IMovieClassification> classify_instances(IList<Instance> instances)
    {
      return instances.Select(ins => new SOMMovieClassification(ins.movie, network.classifyInstance(ins.tag_relevances.Select(tr => tr.relevance).ToList()).First()));
    }

    public IEnumerable<IClassLabel> best_matching_units(UserProfile userProfile, int number_of_attributes)
    {
      return network.classifyInstance(userProfile.profile.Select((p, index) => new KeyValuePair<int, double>(index, p)).OrderByDescending(p => p.Value).Take(number_of_attributes).ToList())
                    .Cast<IClassLabel>().ToList();
    }

    public IEnumerable<IClassLabel> best_matching_units(UserProfile userProfile)
    {
      return network.classifyInstance(userProfile.profile).ToList();
    }

    public void train(IList<Instance> instances)
    {
      if (instances == null)
        throw new ArgumentException("instances");

      var random = new Random((int)DateTime.Now.Ticks);
      var iteration = 0;
      var stop = false;
      while (!stop)
      {
        var generatedNumber = (int)(random.NextDouble() * 10 * instances.Count); //10 vezes a quantidade de filmes
        stop = iterate(instances[generatedNumber % instances.Count].tag_relevances, iteration);
        iteration++;
      }      
    }

    public IEnumerable<string> printClassifier()
    {
      return network.printNetwork();
    }

    public Func<Movie, string, IMovieClassification> parse_movie_classification()
    {
      return (movie, str) =>
      {
        var coordinates = str.Split(',');
        var x = int.Parse(coordinates[0]);
        var y = int.Parse(coordinates[1]);

        return new SOMMovieClassification(movie, network.neurons[x][y]);
      };
    }

    public void parse_classifier(IList<string> classLabelConfig)
    {
      var configPattern = new Regex("\\[(.*)\\],\\[(.*)\\]");
      foreach (var neuron in classLabelConfig)
      {
        var match = configPattern.Match(neuron);
        if (!match.Success)
          throw new InvalidOperationException("Não deu match.");

        var coordinates = match.Groups[1].Value.Split(';');
        var weights = match.Groups[2].Value.Split(';');

        var x = int.Parse(coordinates[0]);
        var y = int.Parse(coordinates[1]);
        var parsedWeights = weights.Select(w => double.Parse(w));
        network.neurons[x][y] = new Neuron(x, y, parsedWeights.ToList());
      }
    }
    
    public SelfOrganizingMap(int rows, int columns, int attr_count, INeighborhoodFunction neighborhoodFunction, ILearningRateFunction learningRateFunction, IMetric metric)
    {
      if (neighborhoodFunction == null)
        throw new ArgumentException("neighborhoodFunction");
      if (learningRateFunction == null)
        throw new ArgumentException("learningRateFunction");

      network = new Network.Network(rows, columns, attr_count, metric);
      neighborhood_function = neighborhoodFunction;
      learning_rate_function = learningRateFunction;
      this.metric = metric;
      maxIterations = 1000 + rows * columns * 500;
    }
  }
}
