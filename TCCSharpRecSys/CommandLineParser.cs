using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCCSharpRecSys.Persistence;
using UnsupervisedLearning.SelfOrganizingMaps;
using UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions;
using UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions;
using Utils;
using Utils.Metric;

namespace TCCSharpRecSys
{
  public class CommandLineParser
  {
    
    public IList<Action> parseCommands(string commands)
    {
      var commandLines = commands.Split('\n');

      return commandLines.Select(cl => parseLine(cl)).ToList();
    }

    public Action parseLine(string line)
    {

      var tokens = line.Split(' ');

      var op = tokens[0];

      switch (op)
      {
        case "filepath":
          return () => FileReader.setFilePath(line.Substring(2, line.Length - 2));
        case "train":
          return null;
        case "classify":
          return null;
        case "profiles":
          return null;
        default:
          throw new NotSupportedException("Opção \"" + op + "\" não encontrada.");
      }
    }
    
    private Action train(string[] tokens)
    {
      var algorithm = tokens[0];
      switch (algorithm)
      {
        case "som":
          return trainSom(tokens.Skip(1).ToList());
        case "k-means":
          return trainkMeans(tokens.Skip(1).ToList());
        case "c-means":
          return traincMeans(tokens.Skip(1).ToList());
        case "boltzman-machine":
          return trainBoltzman(tokens.Skip(1).ToList());
        default:
          throw new NotSupportedException("Algoritmo de treinamento \"" + algorithm + "\" não encontrado");
      }
    }

    private Action classify(string[] tokens)
    {
      var algorithm = tokens[0];
      switch (algorithm)
      {
        case "som":
          return classifySom(tokens.Skip(1).ToList());
        case "k-means":
          return classifykMeans(tokens.Skip(1).ToList());
        case "c-means":
          return classifycMeans(tokens.Skip(1).ToList());
        case "boltzman-machine":
          return classifyBoltzman(tokens.Skip(1).ToList());
        default:
          throw new NotSupportedException("Algoritmo de treinamento \"" + algorithm + "\" não encontrado");
      }
    }

    private Action trainSom(IList<string> args)
    {
      int rows = 0;
      int columns = 0;
      IMetric distanceMetric = null;
      INeighborhoodFunction neighborhoodFunction = null;
      ILearningRateFunction learningRateFunction = new LearningRateFunction();
      if (!int.TryParse(args[0], out rows))
        throw new ArgumentException("O primeiro argumento para treinamento de um SOM deve ser um inteiro correspondente ao número de linhas do SOM");
      if (!int.TryParse(args[1], out columns))
        throw new ArgumentException("O segunddo argumento para treinamento de um SOM deve ser um inteiro correspondente ao número de colunas do SOM");
      switch (args[2])
      {
        case "euclidian":
          distanceMetric = new EuclidianDistance();
          break;
        case "manhattan":
          distanceMetric = new ManhattanDistance();
          break;
        case "cosine":
          distanceMetric = new CosineSimilarity();
          break;
        default:
          throw new ArgumentException("O terceiro argumento para treinamento de um SOM deve ser uma string representando a métrica de distância adotada. Valores permitidos: \"euclidian\", \"manhattan\" e \"cosine\"");
      }
      switch (args[3])
      {
        case "gaussian":
          neighborhoodFunction = new GaussianNeighborhood(Math.Round(Math.Max(rows, columns) / 2 + .5));
          break;
        default:
          throw new ArgumentException("O quarto argumento para treinamento de um SOM, que deve ser uma string que indica o método de vizinhança a ser utilizada, não bate com os métodos existentes.");
      }
      var som = new SelfOrganizingMap(rows, columns, 1128, neighborhoodFunction, learningRateFunction, distanceMetric);

      return () =>
      {
        var fileWritter = FileWritter.getInstance();
        Console.WriteLine("Treinando SOM. linhas = " + rows + "; colunas = " + columns + "; métrica = " + args[2] + "; função de vizinhança = " + args[3]);
        fileWritter.log("Treinando SOM. linhas = " + rows + "; colunas = " + columns + "; métrica = " + args[2] + "; função de vizinhança = " + args[3]);
        

        var tagRelevances = FileReader.getInstance().readTagRelevances()
                                                    .GroupBy(ma => ma.movie)
                                                    .Select(ma => ma.OrderBy(m => m.tag.id).ToList())
                                                    .ToList();        
        var stop = false;
        var iteration = 0;

        while (!stop)
        {
          if (iteration % tagRelevances.Count == 0)
            tagRelevances.Shuffle();
          stop = som.iterate(tagRelevances[iteration % tagRelevances.Count], iteration);
          iteration++;
        }
        var fileName = fileWritter.writeSOMNodes(som, rows + "_" + columns + "_" + args[2] + "_" + args[3]);
        Console.WriteLine("Treinamento finalizado. Escrito em " + fileName);
        fileWritter.log("Treinamento finalizado. Escrito em " + fileName);
      };
    }

    private Action classifySom(IList<string> args)
    {
      int rows = 0;
      int columns = 0;
      IMetric distanceMetric = null;
      INeighborhoodFunction neighborhoodFunction = null;
      ILearningRateFunction learningRateFunction = new LearningRateFunction();
      if (!int.TryParse(args[0], out rows))
        throw new ArgumentException("O primeiro argumento para treinamento de um SOM deve ser um inteiro correspondente ao número de linhas do SOM");
      if (!int.TryParse(args[1], out columns))
        throw new ArgumentException("O segunddo argumento para treinamento de um SOM deve ser um inteiro correspondente ao número de colunas do SOM");
      switch (args[2])
      {
        case "euclidian":
          distanceMetric = new EuclidianDistance();
          break;
        case "manhattan":
          distanceMetric = new ManhattanDistance();
          break;
        case "cosine":
          distanceMetric = new CosineSimilarity();
          break;
        default:
          throw new ArgumentException("O terceiro argumento para treinamento de um SOM deve ser uma string representando a métrica de distância adotada. Valores permitidos: \"euclidian\", \"manhattan\" e \"cosine\"");
      }
      switch (args[3])
      {
        case "gaussian":
          neighborhoodFunction = new GaussianNeighborhood(Math.Round(Math.Max(rows, columns) / 2 + .5));
          break;
        default:
          throw new ArgumentException("O quarto argumento para treinamento de um SOM, que deve ser uma string que indica o método de vizinhança a ser utilizada, não bate com os métodos existentes.");
      }

      int instance = 0;
      if (!int.TryParse(args[4], out instance))
        throw new ArgumentException("O quarto argumento para treinamento de um SOM, que representa o número da instância com a configuração apresentada, não é um inteiro.");
      if (instance <= 0)
        throw new ArgumentOutOfRangeException("(Treinamento SOM) Instância deve ser maior que zero.");

      

      return () =>
      {
        var fileReader = FileReader.getInstance();
        var existingNeurons = fileReader.existingNeurons(rows, columns, args[2], args[3], instance);

        var somClassify = new SOMClassification(rows, columns, 1128, instance, distanceMetric, existingNeurons, false);

        var tagRelevances = fileReader.readTagRelevances()
                                      .GroupBy(tr => tr.movie)
                                      .Select(tr => new { movie = tr.Key, tagRels = tr.OrderBy(tr1 => tr1.tag.id) } );

        var moviesClassifies = new List<MovieSOMClassification>();
        foreach (var movieRelevances in tagRelevances)
        {
          var neuron = somClassify.classify(movieRelevances.tagRels.Select(mr => mr.relevance).ToList()).First();
        }
                                      
      };
      


      throw new NotImplementedException();
    }
        
    private Action trainkMeans(IList<string> args)
    {

      throw new NotImplementedException();
    }

    private Action classifykMeans(IList<string> args)
    {

      throw new NotImplementedException();
    }

    
    private Action traincMeans(IList<string> args)
    {

      throw new NotImplementedException();
    }

    private Action classifycMeans(IList<string> args)
    {

      throw new NotImplementedException();
    }
       
    private Action trainBoltzman(IList<string> args)
    {

      throw new NotImplementedException();
    }

    private Action classifyBoltzman(IList<string> args)
    {

      throw new NotImplementedException();
    }
    
  }
}
