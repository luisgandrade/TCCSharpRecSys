using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions
{
  /// <summary>
  /// Essa implementação da função de taxa de aprendizado segue as sugestões propostas por Haykin no livro Redes Neurais: Princípios e Prática.
  ///
  /// </summary>
  public class LearningRateFunction : ILearningRateFunction
  {
    /// <summary>
    /// Taxa de aprendizado inicial.
    /// </summary>
    private double initial_learning_rate;
    /// <summary>
    /// Taxa de decaimento da função de taxa de aprendizado durante a fase de ordenação.
    /// </summary>
    private double time_constant;
    /// <summary>
    /// Limiar para transição para a fase de convergência. Após início da fase de convergência, este valor será sempre retornado
    /// como taxa de aprendizado.
    /// </summary>
    private double convergence_phase_threshold;
    /// <summary>
    /// Iteração em que o algoritmo passará da fase de ordenação para a fase de convergência.
    /// </summary>
    private int advance_phase_threshold;
    
    public double apply(int iteration)
    {
      if (iteration > advance_phase_threshold)
        return convergence_phase_threshold;
      return initial_learning_rate * Math.Exp(-(iteration / time_constant));
    }
    
    /// <summary>
    /// Configura uma função de taxa de aprendizado com a taxa de aprendizado, constante de decaimento e limiar de transição de fase informados.
    /// </summary>
    public LearningRateFunction(double initialLearningRate, double timeConstant, double convergencePhaseThreshold)
    {
      if (initialLearningRate <= 0)
        throw new InvalidOperationException("Taxa de aprendizado inicial deve ser maior que zero.");
      if (timeConstant <= 0)
        throw new InvalidOperationException("Constante de decaimento deve ser maior que zero.");
      if (convergencePhaseThreshold <= 0)
        throw new InvalidOperationException("Limiar para transição para fase de convergência deve ser maior que zero.");
      if (convergencePhaseThreshold >= initialLearningRate)
        throw new InvalidOperationException("Limiar para transição para fase de convergência deve ser menor que a taxa de aprendizado inicial.");

      initial_learning_rate = initialLearningRate;
      time_constant = timeConstant;
      convergence_phase_threshold = convergencePhaseThreshold;
      advance_phase_threshold = (int)(- timeConstant * Math.Log(convergence_phase_threshold / initialLearningRate));
    }

    /// <summary>
    /// Configura uma função de taxa de aprendizado com a taxa de aprendizado, constante de tempo e limiar de transição de fase sugeridos por
    /// Haykin em Redes Neurais: Princípios e Prática(taxa de aprendizado inical = 0.1; constante de tempo = 1000; limiar de transição de fase = 0.01)
    /// </summary>
    public LearningRateFunction()
      :this(0.1, 1000, 0.01)
    { }
  }
}
