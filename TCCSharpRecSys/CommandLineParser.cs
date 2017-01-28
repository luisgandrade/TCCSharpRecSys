using Lib;
using MachineLearning.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TCCSharpRecSys.Persistence;
using UnsupervisedLearning;
using UnsupervisedLearning.KMeans;
using UnsupervisedLearning.SelfOrganizingMaps;
using UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.Network;
using UnsupervisedLearning.UserProfiles;
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

      var commandLines = inputText.Split('\n').Where(cl => !string.IsNullOrWhiteSpace(cl)).Select(cl => cl.Trim());

      var firstLine = commandLines.First();
      var regex = new Regex("^set_dir (.*)$");

      var match = regex.Match(firstLine);
      if (!match.Success)
        throw new InvalidOperationException("A primeira linha deve necessariamente setar o diretório onde estão e serão salvos os arquivos que serão usados!");

      var filePath = match.Groups[1].Value.Trim();
      FileReader.setDirPath(filePath);
      FileWritter.setDirPath(filePath);

      var regexProfiles = new Regex("profiles ct=([0-9]+\\.[0-9]+) (.*)");

      var commands = new List<Action>();

      var matchBuildUserProfile = commandLines.Where(cl => regexProfiles.IsMatch(cl)).Select(cl => new { commandLine = cl, match = regexProfiles.Match(cl) } );
      if(matchBuildUserProfile.Any())
      {
        commands.AddRange(matchBuildUserProfile.Select(mb => buildUserProfiles(double.Parse(mb.match.Groups[1].Value), mb.match.Groups[2].Value)));
        
      }

      commands.AddRange(commandLines.Skip(1).Except(matchBuildUserProfile.Select(mb => mb.commandLine)).Select(cl => parseCommand(cl)).ToList());
      return commands;
    }

    public Action parseCommand(string line)
    {
      if (line == null)
        throw new ArgumentException("line");

      var regex = new Regex("([a-z]*) (.*)");

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
                                              .GroupBy(tr => tr.movie)
                                              .Select(tr => new Instance(tr.ToList()))
                                              .ToList();
        var fileWritter = FileWritter.getInstance();
        for (int i = firstInstance; i <= lastInstance; i++)
        {
          var algorithm = algorithmGen();
          Console.WriteLine("Treinando algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i);
          fileWritter.log("Treinando algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i, true);

          var start = DateTime.Now;
          algorithm.train(instances);
          var end = DateTime.Now;

          fileWritter.writeTrainedAlgorithmInfo(algorithm.sub_dir, algorithm.file_prefix, i, algorithm.printClassifier());

          var execTime = end - start;

          Console.WriteLine("Finalizou treinamento do algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i + ". Tempo de execução: " +
            execTime.Hours + "h" + execTime.Minutes + "min");
          fileWritter.log("Finalizou treinamento do algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i + ". Tempo de execução: " +
            execTime.Hours + "h" + execTime.Minutes + "min", true);

        }
      };
    }

    private Action classify(string args)
    {
      if (args == null)
        throw new ArgumentException("args");

      var regex = new Regex("(.*?) instances=([1-9]+),([0-9]+)");

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
        if(Enumerable.Range(firstInstance, lastInstance - firstInstance + 1).Except(exisingTrainedModels).Count() > 0)
          throw new InvalidOperationException("O comando deve classificar de acordo as instâncias de " + firstInstance + " até " + lastInstance + ". Porém, as " +
            "seguintes instâncias não foram encontradas em disco: " + 
            string.Join(",", Enumerable.Range(firstInstance, lastInstance - firstInstance + 1).Except(exisingTrainedModels).ToArray()));

        var fileWritter = FileWritter.getInstance();
        var fileReader = FileReader.getInstance();

        var instances = fileReader.readTagRelevances()
                                  .GroupBy(tr => tr.movie)
                                  .Select(tr => new Instance(tr.ToList()))
                                  .ToList();        
        
        for (int i = firstInstance; i <= lastInstance; i++)
        {
          var algorithm = algorithmGen();
          var algorithmConfig = fileReader.readAlgorithmConfig(algorithm.sub_dir, algorithm.file_prefix, i);
          algorithm.parse_classifier(algorithmConfig);

          Console.WriteLine("Classificando filmes com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i);
          fileWritter.log("Classificando filmes com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i, true);

          var start = DateTime.Now;
          var movieClassifications = algorithm.classify_instances(instances);
          var end = DateTime.Now;

          fileWritter.writeMovieClassifications(algorithm.sub_dir, algorithm.file_prefix, i, movieClassifications);
          
          var execTime = end - start;

          Console.WriteLine("Finalizou classificação com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i + ". Tempo de execução: " + 
            execTime.Hours + "h" + execTime.Minutes + "min");
          fileWritter.log("Finalizou classificação com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i + ". Tempo de execução: " +
            execTime.Hours + "h" + execTime.Minutes + "min", true);
        }
      };
    }

    private Action recommend(string args)
    {

      if (args == null)
        throw new ArgumentException("args");

      var regex = new Regex("(.*?) instances=([0-9]+),([0-9]+) ct=(0\\.[1-9]) p=([0-9]+)");

      var match = regex.Match(args);

      if (!match.Success)
        throw new InvalidOperationException("Não foi possível parsear para classificação.");

      var algorithmInfo = match.Groups[1].Value;
      var firstInstance = int.Parse(match.Groups[2].Value);
      var lastInstance = int.Parse(match.Groups[3].Value);
      var userProfileCutoff = double.Parse(match.Groups[4].Value);
      var predictNextN = int.Parse(match.Groups[5].Value);

      if (firstInstance > lastInstance)
        throw new ArgumentOutOfRangeException("O limite inferior do range é maior que o limite superior.");

      var algorithmGen = parseAlgorithm(algorithmInfo);

      return () =>
      {
        var fileReader = FileReader.getInstance();
        var fileWriter = FileWritter.getInstance();
        var dummyAlg = algorithmGen();
        var exisingTrainedModels = fileReader.existingInstances(dummyAlg.sub_dir, dummyAlg.file_prefix);
        if (!exisingTrainedModels.OrderBy(etm => etm).SequenceEqual(Enumerable.Range(firstInstance, lastInstance)))
          throw new InvalidOperationException("O comando deve classificar de acordo as instâncias de " + firstInstance + " até " + lastInstance + ". Porém, as " +
            "seguintes instâncias não foram encontradas em disco: " + string.Join(",", exisingTrainedModels.ToArray()));
        
        var profileParts = fileReader.getPartsOfProfiles(userProfileCutoff);

        Func<int, int, Action> act = (first, last) =>
        {

          return () =>
          {
            for (int i = first; i <= last; i++)
            {
              var newAlg = algorithmGen();

              while (!Monitor.TryEnter(fileWriter))
                Thread.Sleep(100);
              try
              {                
                fileWriter.log("Recomendando filmes com algoritmo " + newAlg.name + ". Config: " + newAlg.file_prefix + ". Instância: " + i, true);
              }
              finally
              {
                Monitor.Exit(fileWriter);
              }
              Console.WriteLine("Recomendando filmes com algoritmo " + newAlg.name + ". Config: " + newAlg.file_prefix + ". Instância: " + i);

              var start = DateTime.Now;

              IList<string> trainedModel = new List<string>();
              IList<IMovieClassification> moviesClassifications = new List<IMovieClassification>();
              while (!Monitor.TryEnter(fileReader))
                Thread.Sleep(100);
              try
              {
                trainedModel = fileReader.readAlgorithmConfig(newAlg.sub_dir, newAlg.file_prefix, i);
                moviesClassifications = fileReader.readMovieClassification(newAlg.sub_dir, newAlg.file_prefix, i, newAlg.parse_movie_classification());
              }              
              finally
              {
                Monitor.Exit(fileReader);
              }

              newAlg.parse_classifier(trainedModel);
              
              var moviesByLabel = moviesClassifications.GroupBy(mc => mc.label).ToDictionary(mc => mc.Key, mc => mc.Select(mc1 => mc1.movie).ToList());
              var recommender = new Recommender(newAlg, moviesByLabel, predictNextN);

              var recResults = new List<RecommendationResults>();
              foreach (var pt in profileParts)
              {
                IList<UserProfile> usersProfiles = new List<UserProfile>();
                IList<Rating> ratings = new List<Rating>();
                while (!Monitor.TryEnter(fileReader))
                  Thread.Sleep(100);
                try
                {
                  usersProfiles = fileReader.readUserProfiles(userProfileCutoff, pt);
                  ratings = fileReader.readUserRatings(pt);
                }
                finally
                {
                  Monitor.Exit(fileReader);
                }

                var profilesWithRatings = usersProfiles.GroupJoin(ratings, up => up.user_id, r => r.user_id, (up, r) => new { userProfile = up, ratings = r.OrderBy(r1 => r1.timestamp).ToList() })
                                                        .Select(upr => new
                                                        {
                                                          userProfile = upr.userProfile,
                                                          moviesAlreadyWatched = upr.ratings.Take((int)(userProfileCutoff * upr.ratings.Count)).Select(r => r.movie),
                                                          ratingsNotIncluded = upr.ratings.Skip((int)(userProfileCutoff * upr.ratings.Count))
                                                        });
                foreach (var profile in profilesWithRatings)
                  recResults.Add(recommender.recommend(profile.userProfile, profile.moviesAlreadyWatched.ToList(), profile.ratingsNotIncluded.ToList()));
              }
              var end = DateTime.Now;

              var execTime = end - start;
              while (!Monitor.TryEnter(fileWriter))
                Thread.Sleep(100);
              try
              {
                fileWriter.writeRecommendationResults(newAlg.sub_dir, newAlg.file_prefix, userProfileCutoff, predictNextN, i, recResults);
                fileWriter.log("Finalizou recomendações com algoritmo " + newAlg.name + ". Config: " + newAlg.file_prefix + ". Instância: " + i + ". Tempo de execução: " +
                execTime.Hours + "h" + execTime.Minutes + "min", true);
              }
              finally
              {
                Monitor.Exit(fileWriter);
              }
              Console.WriteLine("Finalizou recomendações com algoritmo " + newAlg.name + ". Config: " + newAlg.file_prefix + ". Instância: " + i + ". Tempo de execução: " +
                execTime.Hours + "h" + execTime.Minutes + "min");
              
            }
          };
        };

        var parallelizationThreshold = (int)((lastInstance - firstInstance) / 3.0);

        var firstTask = Task.Run(act(firstInstance, parallelizationThreshold));
        var secondTask = Task.Run(act(parallelizationThreshold + 1, 2 * parallelizationThreshold));
        var thirdTask = Task.Run(act(2 * parallelizationThreshold + 1, lastInstance));

        Task.WaitAll(firstTask, secondTask, thirdTask);
        
      };
    }

    private Action buildUserProfiles(double cutoff, string decay)
    {
      if(cutoff <= 0 || cutoff >= 1)
        throw new ArgumentOutOfRangeException("O cutoff deve ser um valor maior que zero e menor que um.");
      if (decay == null)
        throw new ArgumentException("decay");

      IUSerProfileBuilder userProfileBuilder = null;
      switch (decay)
      {
        case "exp":
          userProfileBuilder = new WeightedAverageUserProfileBuilder(new ExponentialDecay());
          break;
        case "linear":
          userProfileBuilder = new WeightedAverageUserProfileBuilder(new LinearDecay());          
          break;
        case "no":
          userProfileBuilder = new WeightedAverageUserProfileBuilder(new NoDecay());
          break;
        case "normal":
          userProfileBuilder = new NormalizedUserProfileBuilder();
          break;
        default:
          throw new InvalidOperationException("Forma de decay das avaliações não existe.");
      }
      return () =>
      {
        var fileReader = FileReader.getInstance();
        var tagRelevances = fileReader.readTagRelevances();        
        
        var fileWritter = FileWritter.getInstance();

        Console.WriteLine("Construindo perfis de usuários. Porcentagem de corte: " + cutoff + ". Decaimento: " + userProfileBuilder.label);
        fileWritter.log("Construindo perfis de usuários. Porcentagem de corte: " + cutoff + ". Decaimento: " + userProfileBuilder.label, true);
        var start = DateTime.Now;
        var pts = fileReader.getPartsOfRatings();
        foreach (var pt in pts)
        {          
          var ratings = fileReader.readUserRatings(pt);          
          var userProfiles =  userProfileBuilder.buildUserProfiles(ratings, tagRelevances, cutoff);
          Console.WriteLine("Escrevendo perfis dos usuários... [" + ((pt - 1) * 20000) + " a " + (pt * 20000 - 1) + "]");
          fileWritter.appendUserProfiles(userProfileBuilder.label + "_" + cutoff + "_" + "pt" + pt + ".csv", userProfiles);
          userProfiles = null;
        }
        var end = DateTime.Now;
        var duration = end - start;
        Console.WriteLine("Construção de perfis de usuários finalizada. Porcentagem de corte: " + cutoff + ". Decaimento: " + userProfileBuilder.label + ". Tempo de execução: " + duration.TotalHours + "h" + duration.TotalMinutes + "min");
        fileWritter.log("Construção de perfis de usuários finalizada. Porcentagem de corte: " + cutoff + ". Decaimento: " + userProfileBuilder.label + ". Tempo de execução: " + duration.TotalHours + "h" + duration.TotalMinutes + "min", true);
      };
      
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
        case "kmeans":
          return kMeans(args);
        default:
          throw new NotSupportedException("Algoritmo de treinamento \"" + algorithmName + "\" não encontrado");
      }
    }

    private Func<IUnsupervisedLearning> som(string args)
    {
      var regex = new Regex("r=([0-9]+) c=([0-9]+) mt=([a-z]+) n=([a-z]+) niw=([0-9]+)(?: ntc=([0-9]+))?(?: lri=([0-9]*\\.[0-9]+) lrtc=([0-9]*(?:\\.[0-9]+)?))? un=(true|false)");
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
      var regex = new Regex("cl=([0-9]+) un=(true|false)");

      var match = regex.Match(args);

      if(!match.Success)
        throw new InvalidOperationException("Não foi possível parsear os argumentos do K-Means padrão.");

      var clusterCount = int.Parse(match.Groups[1].Value);
      var useNormalizedValues = match.Groups[2].Value == "true";
      return () => new StandardKMeans(clusterCount, useNormalizedValues);
    }
  }
}
