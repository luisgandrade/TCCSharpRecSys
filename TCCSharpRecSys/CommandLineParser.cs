using Lib;
using MachineLearning.UserProfiles;
using Npgsql;
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
using UnsupervisedLearning.UserProfiles.Decay;
using UnsupervisedLearning.UserProfiles.Normalization;
using Utils;
using Utils.Metric;

namespace TCCSharpRecSys
{
  public class CommandLineParser
  {

    private readonly int number_of_threads;

    public CommandLineParser(int number_of_threads)
    {
      this.number_of_threads = number_of_threads;
    }

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
      FileWriter.setDirPath(filePath);

      var regexProfiles = new Regex("profiles ct=([0-9]+\\.[0-9]+) (.+?) (.+?) tag_pop=([0-9]+)");

      var commands = new List<Action>();

      var matchBuildUserProfile = commandLines.Where(cl => regexProfiles.IsMatch(cl)).Select(cl => new { commandLine = cl, match = regexProfiles.Match(cl) } );
      if(matchBuildUserProfile.Any())
        commands.AddRange(matchBuildUserProfile.Select(mb => buildUserProfiles(double.Parse(mb.match.Groups[1].Value), mb.match.Groups[2].Value, mb.match.Groups[3].Value,
          int.Parse(mb.match.Groups[4].Value))));        

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

      var regex = new Regex("(.*?) instances=([0-9]+),([0-9]+) tag_pop=([0-9]+)");

      var match = regex.Match(args);

      if (!match.Success)
        throw new InvalidOperationException("Não foi possível parsear para treinamento.");
            
      var algorithmInfo = match.Groups[1].Value;
      var firstInstance = int.Parse(match.Groups[2].Value);
      var lastInstance = int.Parse(match.Groups[3].Value);
      var tagPopularity = int.Parse(match.Groups[4].Value);

      if (firstInstance > lastInstance)
        throw new ArgumentOutOfRangeException("O limite inferior do range é maior que o limite superior.");
      if (tagPopularity < 0)
        throw new ArgumentOutOfRangeException("A popularidade da tag deve ser maior ou igual a zero.");

      var instances = FileReader.getInstance().readTagRelevances(tagPopularity)
                                              .GroupBy(tr => tr.movie)
                                              .Select(tr => new Instance(tr.ToList()))
                                              .ToList();

      var algorithmGen = parseAlgorithm(algorithmInfo, instances.First().tag_relevances.Count);

      return () =>
      {
        
        var fileWriter = FileWriter.getInstance();
        for (int i = firstInstance; i <= lastInstance; i++)
        {
          var algorithm = algorithmGen();
          Console.WriteLine("Treinando algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i);
          fileWriter.log("Treinando algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i, true);

          var start = DateTime.Now;
          algorithm.train(instances);
          var end = DateTime.Now;

          fileWriter.writeTrainedAlgorithmInfo(algorithm.sub_dir, algorithm.file_prefix, i, algorithm.printClassifier());

          var execTime = end - start;

          Console.WriteLine("Finalizou treinamento do algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i + ". Tempo de execução: " +
            execTime.Hours + "h" + execTime.Minutes + "min");
          fileWriter.log("Finalizou treinamento do algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i + ". Tempo de execução: " +
            execTime.Hours + "h" + execTime.Minutes + "min", true);

        }
      };
    }

    private Action classify(string args)
    {
      if (args == null)
        throw new ArgumentException("args");

      var regex = new Regex("(.*?) instances=([1-9]+),([0-9]+) tag_pop=([0-9]+)");


      var match = regex.Match(args);

      if (!match.Success)
        throw new InvalidOperationException("Não foi possível parsear para classificação.");

      var algorithmInfo = match.Groups[1].Value;
      var firstInstance = int.Parse(match.Groups[2].Value);
      var lastInstance = int.Parse(match.Groups[3].Value);
      var tagPopularity = int.Parse(match.Groups[4].Value);

      if (firstInstance > lastInstance)
        throw new ArgumentOutOfRangeException("O limite inferior do range é maior que o limite superior.");


      var fileWriter = FileWriter.getInstance();
      var fileReader = FileReader.getInstance();

      var instances = fileReader.readTagRelevances(tagPopularity)
                                .GroupBy(tr => tr.movie)
                                .Select(tr => new Instance(tr.ToList()))
                                .ToList();

      var algorithmGen = parseAlgorithm(algorithmInfo, instances.First().tag_relevances.Count);
      
      return () =>
      {
        var dummyAlg = algorithmGen();
        var exisingTrainedModels = FileReader.getInstance().existingInstances(dummyAlg.sub_dir, dummyAlg.file_prefix);
        if(Enumerable.Range(firstInstance, lastInstance - firstInstance + 1).Except(exisingTrainedModels).Count() > 0)
          throw new InvalidOperationException("O comando deve classificar de acordo as instâncias de " + firstInstance + " até " + lastInstance + ". Porém, as " +
            "seguintes instâncias não foram encontradas em disco: " + 
            string.Join(",", Enumerable.Range(firstInstance, lastInstance - firstInstance + 1).Except(exisingTrainedModels).ToArray()));
        
        Func<int, int, Action> action = (lower, upper) =>
        {
          return () =>
          {
            for (int i = lower; i <= upper; i++)
            {
              var algorithm = algorithmGen();

              IList<string> algorithmConfig = null;
              while (!Monitor.TryEnter(fileReader))
                Thread.Sleep(100);
              try
              {
                algorithmConfig = fileReader.readAlgorithmConfig(algorithm.sub_dir, algorithm.file_prefix, i);
              }
              finally
              {
                Monitor.Exit(fileReader);
              }              
              Console.WriteLine("Classificando filmes com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i);

              algorithm.parse_classifier(algorithmConfig);
              
              var start = DateTime.Now;
              var movieClassifications = algorithm.classify_instances(instances, instances.First().tag_relevances.Count);
              var end = DateTime.Now;


              while (!Monitor.TryEnter(fileWriter))
                Thread.Sleep(100);
              try
              {
                fileWriter.writeMovieClassifications(algorithm.sub_dir, algorithm.file_prefix, i, movieClassifications);
              }
              finally
              {
                Monitor.Exit(fileWriter);
              }              
              var execTime = end - start;

              Console.WriteLine("Finalizou classificação com algoritmo " + algorithm.name + ". Config: " + algorithm.file_prefix + ". Instância: " + i + ". Tempo de execução: " +
                execTime.Hours + "h" + execTime.Minutes + "min");
            }
          };
        };

        var parallelizationThreshold = (int)((lastInstance - firstInstance + 1) / (decimal)number_of_threads);

        var tasks = Enumerable.Range(1, number_of_threads).Select(r => Task.Run(action(firstInstance + (r - 1) * parallelizationThreshold,
          lastInstance - (number_of_threads - r) * parallelizationThreshold - 1)));

        Task.WaitAll(tasks.ToArray());

      };
    }

    private Action recommend(string args)
    {

      if (args == null)
        throw new ArgumentException("args");

      var regex = new Regex("(.*?) instances=([0-9]+),([0-9]+) tag_pop=([0-9]+) ct=(0\\.[1-9]) (.*?) (.*?) p=([0-9]+)");

      var match = regex.Match(args);

      if (!match.Success)
        throw new InvalidOperationException("Não foi possível parsear para classificação.");

      var algorithmInfo = match.Groups[1].Value;
      var firstInstance = int.Parse(match.Groups[2].Value);
      var lastInstance = int.Parse(match.Groups[3].Value);
      var tagPopularity = int.Parse(match.Groups[4].Value);
      var userProfileCutoff = double.Parse(match.Groups[5].Value);
      var decay = match.Groups[6].Value;
      var normalization = match.Groups[7].Value;
      var recommendN = int.Parse(match.Groups[8].Value);

      var attrCount = FileReader.getInstance().readTags(tagPopularity).Count;

      if (firstInstance > lastInstance)
        throw new ArgumentOutOfRangeException("O limite inferior do range é maior que o limite superior.");

      var algorithmGen = parseAlgorithm(algorithmInfo, attrCount);

      return () =>
      {
        var fileReader = FileReader.getInstance();
        var fileWriter = FileWriter.getInstance();
        var dummyAlg = algorithmGen();
        var exisingTrainedModels = fileReader.existingInstances(dummyAlg.sub_dir, dummyAlg.file_prefix);
        if (!exisingTrainedModels.OrderBy(etm => etm).SequenceEqual(Enumerable.Range(firstInstance, lastInstance)))
          throw new InvalidOperationException("O comando deve classificar de acordo as instâncias de " + firstInstance + " até " + lastInstance + ". Porém, as " +
            "seguintes instâncias não foram encontradas em disco: " + string.Join(",", exisingTrainedModels.ToArray()));
        
        var profileParts = fileReader.getPartsOfProfiles(decay, userProfileCutoff, normalization, attrCount);

        Func<int, int, Action> act = (first, last) =>
        {

          return () =>
          {
            for (int i = first; i <= last; i++)
            {
              var newAlg = algorithmGen();
              
              Console.WriteLine("Recomendando filmes com algoritmo " + newAlg.name + ". Config: " + newAlg.file_prefix + ". Instância: " + i);

              var start = DateTime.Now;

              IList<string> trainedModel = new List<string>();
              IList<IMovieClassification> moviesClassifications = new List<IMovieClassification>();
              while (!Monitor.TryEnter(fileReader))
                Thread.Sleep(100);
              try
              {
                trainedModel = fileReader.readAlgorithmConfig(newAlg.sub_dir, newAlg.file_prefix, i);
                newAlg.parse_classifier(trainedModel);
                moviesClassifications = fileReader.readMovieClassification(newAlg.sub_dir, newAlg.file_prefix, i, newAlg.parse_movie_classification());
              }              
              finally
              {
                Monitor.Exit(fileReader);
              }

              newAlg.parse_classifier(trainedModel);
              
              var moviesByLabel = moviesClassifications.GroupBy(mc => mc.label).ToDictionary(mc => mc.Key, mc => mc.Select(mc1 => mc1.movie).ToList());
              var recommender = new Recommender(newAlg, moviesByLabel, recommendN);

              var recResults = new List<RecommendationResults>();
              foreach (var pt in profileParts)
              {
                IList<UserProfile> usersProfiles = new List<UserProfile>();
                IList<Rating> ratings = new List<Rating>();
                while (!Monitor.TryEnter(fileReader))
                  Thread.Sleep(100);
                try
                {
                  usersProfiles = fileReader.readUserProfiles(decay, normalization, userProfileCutoff, attrCount, pt);
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
                                                          ratingsNotIncluded = upr.ratings.Skip((int)(userProfileCutoff * upr.ratings.Count)),
                                                          ratingsAverage = upr.ratings.Average(r => r.rating)
                                                        });
                foreach (var profile in profilesWithRatings)
                  recResults.Add(recommender.recommend(profile.userProfile, profile.moviesAlreadyWatched.ToList(), profile.ratingsNotIncluded.ToList(), profile.ratingsAverage));
              }
              var end = DateTime.Now;

              var execTime = end - start;

              while (!Monitor.TryEnter(fileWriter))
                Thread.Sleep(100);
              try
              {
                fileWriter.writeRecommendationResults(newAlg.sub_dir, newAlg.file_prefix, userProfileCutoff, decay, normalization, recommendN, i, recResults);
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

        var parallelizationThreshold = (int)((lastInstance - firstInstance + 1) / (decimal) number_of_threads);

        var tasks = Enumerable.Range(1, number_of_threads).Select(r => Task.Run(act(firstInstance + (r - 1) * parallelizationThreshold,
          lastInstance - (number_of_threads - r) * parallelizationThreshold - 1)));

        Task.WaitAll(tasks.ToArray());        
      };
    }

    private Action buildUserProfiles(double cutoff, string decay, string normalizationFunction, int tagPopularity)
    {
      if(cutoff <= 0 || cutoff >= 1)
        throw new ArgumentOutOfRangeException("O cutoff deve ser um valor maior que zero e menor que um.");
      if (decay == null)
        throw new ArgumentException("decay");

      
      IDecayFormula decayFormula = null;
      IRatingsNormalization ratingsNormalization = null;
      switch (decay)
      {
        case "exponential":
          decayFormula = new ExponentialDecay();
          break;
        case "no":
          decayFormula = new NoDecay();
          break;
        default:
          throw new InvalidOperationException("Forma de decay das avaliações não existe.");
      }
      switch (normalizationFunction)
      {
        case "feature_scaling":
          ratingsNormalization = new FeatureScaling();
          break;
        default:
          throw new InvalidOperationException("Forma de normalização de avaliações não existe.");
      }

      var userProfileBuilder = new WeightedAverageUserProfileBuilder(decayFormula, ratingsNormalization);
      return () =>
      {
        var fileReader = FileReader.getInstance();
        IList<TagRelevance> tagRelevances = null;
        while (!Monitor.TryEnter(fileReader))
          Thread.Sleep(100);
        try
        {
          tagRelevances = fileReader.readTagRelevances(tagPopularity);
        }
        finally
        {
          Monitor.Exit(fileReader);
        }
        var fileWriter = FileWriter.getInstance();

        Console.WriteLine("Construindo perfis de usuários. Porcentagem de corte: " + cutoff + ". Decaimento: " + userProfileBuilder.label);

        
                
        var start = DateTime.Now;
        var pts = fileReader.getPartsOfRatings();
        foreach (var pt in pts)
        {          
          IList<Rating> ratings = null;
          while (!Monitor.TryEnter(fileReader))
            Thread.Sleep(100);
          try
          {
            ratings = fileReader.readUserRatings(pt);
          }
          finally
          {
            Monitor.Exit(fileReader);
          }
          var userProfiles =  userProfileBuilder.buildUserProfiles(ratings, tagRelevances, cutoff);
          Console.WriteLine("Escrevendo perfis dos usuários... [" + ((pt - 1) * 20000) + " a " + (pt * 20000 - 1) + "]");

          while (!Monitor.TryEnter(fileWriter))
            Thread.Sleep(100);
          try
          {
            fileWriter.appendUserProfiles(userProfileBuilder.label + "_" + cutoff + "_" + "pt" + pt + ".csv", userProfiles);
          }
          finally
          {
            Monitor.Exit(fileWriter);
          }
          userProfiles = null;
        }
        var end = DateTime.Now;
        var duration = end - start;
        Console.WriteLine("Construção de perfis de usuários finalizada. Porcentagem de corte: " + cutoff + ". Decaimento: " + userProfileBuilder.label + ". Tempo de execução: " + duration.TotalHours + "h" + duration.TotalMinutes + "min");
      };
      
    }

    private Func<IUnsupervisedLearning> parseAlgorithm(string algorithmInfo, int attrCount)
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
          return som(args, attrCount);
        case "kmeans":
          return kMeans(args, attrCount);
        default:
          throw new NotSupportedException("Algoritmo de treinamento \"" + algorithmName + "\" não encontrado");
      }
    }

    private Func<IUnsupervisedLearning> som(string args, int attrCount)
    {
      var regex = new Regex("r=([0-9]+) c=([0-9]+) mt=([a-z]+) n=([a-z]+) niw=([0-9]+)(?: ntc=([0-9]+))?(?: lri=([0-9]*\\.[0-9]+) lrtc=([0-9]*(?:\\.[0-9]+)?))?");
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

      var learningRateFunction = learningRateTimeConstant.HasValue && initialLearningRage.HasValue ? new LearningRateFunction(initialLearningRage.Value, learningRateTimeConstant.Value) :
                                                                                                     new LearningRateFunction();
      return () => new SelfOrganizingMap(rows, columns, attrCount, parseNeighborhoodFunction(neighborhood, neighborhoodInitialWidth, neighborhoodTimeConstant), learningRateFunction,
        parseMetric(metric));
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

    private Func<IUnsupervisedLearning> kMeans(string args, int attrCount)
    {
      var regex = new Regex("cl=([0-9]+)");

      var match = regex.Match(args);

      if(!match.Success)
        throw new InvalidOperationException("Não foi possível parsear os argumentos do K-Means padrão.");

      var clusterCount = int.Parse(match.Groups[1].Value);
      var useNormalizedValues = match.Groups[2].Value == "true";
      return () => new StandardKMeans(clusterCount, attrCount);
    }
    



  }
}
