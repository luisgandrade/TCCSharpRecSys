using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.SelfOrganizingMaps.LearningRateFunctions
{
  /// <summary>
  /// Disponibiliza uma interface para implementações customizadas a função de taxa de aprendizagem. Essa função deve ser variável no tempo pelo menos enquanto estiver na 
  /// fase de ordenação.
  /// </summary>
  public interface ILearningRateFunction
  {    
    /// <summary>
    /// Aplica a função de taxa de aprendizado ao momento atual.
    /// </summary>
    /// <returns>taxa de aprendizado na iteração <paramref name="iteration"/></returns>
    double apply(int iteration);

    string print { get; }
  }
}
