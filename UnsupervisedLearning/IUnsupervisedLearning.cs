using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnsupervisedLearning;
using UnsupervisedLearning.KMeans;

namespace UnsupervisedLearning
{
  /// <summary>
  /// Fornece uma interface para um algoritmo de aprendizado não-supervisionado.
  /// </summary>
  public interface IUnsupervisedLearning
  {
    /// <summary>
    /// A implementação desse método deve treinar um agrupamento para classificação baseado nas instâncias <paramref name="instances"/>
    /// informadas.
    /// </summary>
    /// <param name="instances">instâncias para treinamento</param>
    void train(IList<Instance> instances);
    /// <summary>
    /// A implementação deste método deve classificar as instâncias <paramref name="instances"/> de acordo com o algoritmo treinado
    /// e retornar uma implementação <see cref="IMovieClassification"/> adequada a este algoritmo para cada uma das instâncias.
    /// </summary>
    /// <param name="instances">instâncias a serem classificadas</param>
    /// <returns></returns>
    IEnumerable<IMovieClassification> classify_instances(IList<Instance> instances, int number_or_attributes);
    /// <summary>
    /// A implementação deste método deve retornar uma lista de <see cref="IClassLabel"/> ordenadas por uma
    /// distância entre o <see cref="UserProfile"/> e o <see cref="IClassLabel"/>. 
    /// </summary>
    /// <param name="userProfile"></param>
    /// <returns></returns>
    IEnumerable<IClassLabel> best_matching_units(UserProfile userProfile, int number_of_attributes);
    /// <summary>
    /// A implementação deste método deve retornar uma lista de string correspondente a cada uma das unidades de agrupamento do
    /// algoritmo.
    /// </summary>
    IEnumerable<string> printClassifier();
    /// <summary>
    /// A implementação deste método deve retornar uma função que, dado um <see cref="Movie"/> e uma string contendo a classificação
    /// de um filme lida de um arquivo, produza uma implementação adequada de <see cref="IMovieClassification"/>
    /// </summary>
    Func<Movie, string, IMovieClassification> parse_movie_classification();
    /// <summary>
    /// A implementação desse método deve construir os agrupamentos do algoritmo a partir de uma lista de strings onde cada 
    /// string representa uma unidade de agrupamento.
    /// </summary>
    void parse_classifier(IList<string> classLabelConfig);
    /// <summary>
    /// Nome do algoritmo para exposição.
    /// </summary>
    string name { get; }
    /// <summary>
    /// Sub-diretório dentro do diretório base onde os arquivos relacionados à implementação serão salvos e lidos.
    /// </summary>
    string sub_dir { get; }
    /// <summary>
    /// Prefixo identificador de uma implementação desta classe com uma determinada configuração. É importante que 
    /// ele seja exclusivo da configuração.
    /// </summary>
    string file_prefix { get; }
  }
}
