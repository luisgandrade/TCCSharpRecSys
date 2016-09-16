using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCCSharpRecSys.Persistence;
using UnsupervisedLearning;
using UnsupervisedLearning.KMeans;
using UnsupervisedLearning.SelfOrganizingMaps;
using UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions;
using Utils;
using Utils.Metric;

namespace TCCSharpRecSys
{
  public class CommandLineParser
  {

    public IList<Action> parseCommands(string inputText)
    {
      if (inputText == null)
        throw new ArgumentException("inputText");

      var commandLines = inputText.Split('\n');

      var firstLine = commandLines.First();
      var regex = new Regex("^set_dir (.*)$");

      var match = regex.Match(firstLine);
      if (!match.Success)
        throw new InvalidOperationException("A primeira linha deve necessariamente setar o diretório onde estão e serão salvos os arquivos que serão usados!");

      var filePath = match.Groups[1].Value;
      FileReader.setFilePath(filePath);
      FileWritter.setFilePath(filePath);

      var regexProfiles = new Regex("profiles ct=([0-9]+\\.[0-9]+)");

      var commands = new List<Action>();

      var matchBuildUserProfile = commandLines.Where(cl => regexProfiles.IsMatch(cl)).Select(cl => regexProfiles.Match(cl)).SingleOrDefault();
      if(matchBuildUserProfile != null)
      {
        commands.Add(buildUserProfiles(double.Parse(matchBuildUserProfile.Groups[1].Value)));
      }

      commands.AddRange(commandLines.Select(cl => parseCommand(cl)).ToList());
      return commands;
    }

    public Action parseCommand(string line)
    {
      if (line == null)
        throw new ArgumentException("line");

      var regex = new Regex("([a-z]*) (.*?) instances=([0-9]+),([0-9]+)");

      var match = regex.Match(line);

      if (!match.Success)
        throw new InvalidOperationException("Commando desconhecido.");


      switch (match.Groups[1].Value)
      {        
        case "train":
          return train(match.Groups[2].Value);
        case "classify":
          return classify(match.Groups[2].Value);
        case "recommend":
          return recommend(match.Groups[2].Value);
        default:
          throw new NotSupportedException("Opção \"" + train(match.Groups[1].Value) + "\" não encontrada.");
      }
    }

    private Action train(string args)
    {
      if (args == null)
        throw new ArgumentException("args");

      var regex = new Regex("(.*?) instances=([0-9]+),([0-9]+)");

      var match = regex.Match(args);

      if (!match.Success)
        throw new InvalidOperationException("Não foi possível parsear para treinamento.");
            
      var algorithmInfo = match.Groups[1].Value;
      var firstInstance = int.Parse(match.Groups[2].Value);
      var lastInstance = int.Parse(match.Groups[3].Value);

      if (firstInstance > lastInstance)
        throw new ArgumentOutOfRangeException("O limite inferior do range é maior que o limite superior.");
            
      var algorithmGen = parseAlgorithm(algorithmInfo);

      return () =>
      {
        var instances = FileReader.getInstance().readTagRelevances()
                                              .GroupBy(tr => tr.tag)
                                              .Select(tr => new Instance(tr.ToList()))
                                              .ToList();
        var fileWritter = FileWritter.getInstance();
        for (int i = firstInstance; i < lastInstance; i++)
        {
          var algorithm = algorithmGen();
          Console.WriteLine("Treinando algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i);
          fileWritter.log("Treinando algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i, true);

          algorithm.train(instances);
          fileWritter.writeTrainedAlgorithmInfo(algorithm.sub_dir, algorithm.file_prefix, i, algorithm.printClassifier());

          Console.WriteLine("Finalizou treinamento do algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i);
          fileWritter.log("Finalizou treinamento do algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i, true);

        }
      };
    }

    private Action classify(string args)
    {
      if (args == null)
        throw new ArgumentException("args");

      var regex = new Regex("(.*?) instances=([1-9]+),([1-9]+)");

      var match = regex.Match(args);

      if (!match.Success)
        throw new InvalidOperationException("Não foi possível parsear para classificação.");

      var algorithmInfo = match.Groups[1].Value;
      var firstInstance = int.Parse(match.Groups[2].Value);
      var lastInstance = int.Parse(match.Groups[3].Value);

      if (firstInstance > lastInstance)
        throw new ArgumentOutOfRangeException("O limite inferior do range é maior que o limite superior.");

      var algorithmGen = parseAlgorithm(algorithmInfo);
      
      return () =>
      {
        var dummyAlg = algorithmGen();
        var exisingTrainedModels = FileReader.getInstance().existingInstances(dummyAlg.sub_dir, dummyAlg.file_prefix);
        if (!exisingTrainedModels.OrderBy(etm => etm).SequenceEqual(Enumerable.Range(firstInstance, lastInstance - firstInstance)))
          throw new InvalidOperationException("O comando deve classificar de acordo as instâncias de " + firstInstance + " até " + lastInstance + ". Porém, as " +
            "seguintes instâncias não foram encontradas em disco: " + string.Join(",", exisingTrainedModels.ToArray()));
        
        var instances = FileReader.getInstance().readTagRelevances()
                                              .GroupBy(tr => tr.tag)
                                              .Select(tr => new Instance(tr.ToList()))
                                              .ToList();
        var fileWritter = FileWritter.getInstance();
        for (int i = firstInstance; i < lastInstance; i++)
        {
          var algorithm = algorithmGen();
          Console.WriteLine("Classificando filmes com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i);
          fileWritter.log("Classificando filmes com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i, true);

          var movieClassifications = algorithm.classify_instances(instances);
          fileWritter.writeMovieClassifications(algorithm.sub_dir, algorithm.file_prefix, i, movieClassifications);

          Console.WriteLine("Finalizou classificação com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i);
          fileWritter.log("Finalizou classificação com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i, true);

        }
      };
    }

    private Action recommend(string args)
    {

      throw new NotImplementedException();
    }

    private Action buildUserProfiles(double cutoff)
    {
      throw new NotImplementedException();
    }

    private Func<IUnsupervisedLearning> parseAlgorithm(string algorithmInfo)
    {
      var regex = new Regex("([a-z]+) (.*)");

      var match = regex.Match(algorithmInfo);

      if (!match.Success)
        throw new InvalidOperationException("Não foi possível parsear para treinamento.");

      var algorithmName = match.Groups[1].Value;
      var args = match.Groups[2].Value;

      switch (algorithmName)
      {
        case "som":
          return som(args);
        case "k-means":
          return kMeans(args);
        case "c-means":
          return cMeans(args);
        case "boltzman-machine":
          return boltzman(args);
        default:
          throw new NotSupportedException("Algoritmo de treinamento \"" + algorithmName + "\" não encontrado");
      }
    }

    private Func<IUnsupervisedLearning> som(string args)
    {
      var regex = new Regex("r=([0-9]+) c=([0-9]+) mt=([a-z]+) n=([a-z]+) niw=([0-9]+)(?: ntc=([0-9]+))?(?: lri=([0-9]*\\.[0-9]+) lrtc=([0-9]*\\.[0-9]+))? un=(true|false)");
      var match = regex.Match(args);

      if (!match.Success)
        throw new InvalidOperationException("Não foi possível parsear os argumentos do SOM.");

      var rows = int.Parse(match.Groups[1].Value);
      var columns = int.Parse(match.Groups[2].Value);
      var metric = match.Groups[3].Value;
      var neighborhood = match.Groups[4].Value;
      var neighborhoodInitialWidth = string.IsNullOrWhiteSpace(match.Groups[5].Value) ? (int?)null : int.Parse(match.Groups[5].Value);
      var neighborhoodTimeConstant = string.IsNullOrWhiteSpace(match.Groups[6].Value) ? (int?)null : int.Parse(match.Groups[6].Value);
      var initialLearningRage = string.IsNullOrWhiteSpace(match.Groups[7].Value) ? (double?)null : double.Parse(match.Groups[7].Value);
      var learningRateTimeConstant = string.IsNullOrWhiteSpace(match.Groups[8].Value) ? (double?)null : double.Parse(match.Groups[8].Value);
      var useNormalizedValues = match.Groups[9].Value == "true";

      var learningRateFunction = learningRateTimeConstant.HasValue && initialLearningRage.HasValue ? new LearningRateFunction(initialLearningRage.Value, learningRateTimeConstant.Value) :
                                                                                                     new LearningRateFunction();
      return () => new SelfOrganizingMap(rows, columns, 1128, parseNeighborhoodFunction(neighborhood, neighborhoodInitialWidth, neighborhoodTimeConstant), learningRateFunction,
        parseMetric(metric), useNormalizedValues);
    }


    private IMetric parseMetric(string metric)
    {
      if (metric == null)
        throw new ArgumentException("metric");

      switch (metric)
      {
        case "euclidian":
          return new EuclidianDistance();
        case "manhattan":
          return new ManhattanDistance();
        case "cosine":
          return new CosineSimilarity();
        default:
          throw new NotSupportedException("Métrica desconhecida.");
      }
    }

    private INeighborhoodFunction parseNeighborhoodFunction(string name, int? initialWidth, int? timeConstant)
    {
      if (name == null)
        throw new ArgumentException("name");

      switch (name)
      {
        case "gaussian":
          return initialWidth.HasValue && timeConstant.HasValue ? new GaussianNeighborhood(initialWidth.Value, timeConstant.Value) : new GaussianNeighborhood(initialWidth.Value);
        default:
          throw new NotSupportedException("Função de vizinhança desconhecida.");
      }
    }

    private Func<IUnsupervisedLearning> kMeans(string args)
    {
      var regex = new Regex("cl=([0-9]+)");

      var match = regex.Match(args);

      if(!match.Success)
        throw new InvalidOperationException("Não foi possível parsear os argumentos do K-Means padrão.");

      var clusterCount = int.Parse(match.Groups[1].Value);
      var kMeans = new StandardKMeans(clusterCount);
      return () => kMeans;
    }

    private Func<IUnsupervisedLearning> cMeans(string args)
    {
      var regex = new Regex("cl=([0-9]+) fz=([0-9]+\\.[0-9]+) st=([0-9]+\\.[0-9]+)");

      var match = regex.Match(args);

      if (!match.Success)
        throw new InvalidOperationException("Não foi possível parsear os argumentos do Fuzzy C-Means.");

      var clusterCount = int.Parse(match.Groups[1].Value);
      var fuzzynessCoefficient = double.Parse(match.Groups[2].Value);
      var stopCriterion = double.Parse(match.Groups[3].Value);

      var cMeans = new FuzzyCMeans(clusterCount, fuzzynessCoefficient, stopCriterion);

      return () => cMeans;
    }

    private Func<IUnsupervisedLearning> boltzman(string args)
    {
      throw new NotImplementedException();
    }    
  }
}
