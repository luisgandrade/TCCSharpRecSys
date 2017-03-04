using System;

namespace UnsupervisedLearning.UserProfiles.Decay
{
  /// <summary>
  /// Fornece uma interface para cálculo de decaimento de peso da avaliação de um usuário para um filme.
  /// </summary>
  public interface IDecayFormula
  {
    /// <summary>
    /// A implementação desse método deve retornar um fator atenuador calculado com base em <paramref name="relativeAge"/>
    /// </summary>
    /// <param name="relativeAge">"idade" da avaliação, i.e., o tempo entre o momento que ela foi feita e o momento de referência</param>
    /// <returns></returns>
    double decay(TimeSpan relativeAge);
    /// <summary>
    /// Uma string identificado da fórmula para impressão.
    /// </summary>
    string decay_display { get; }
  }
}
