using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
  public static class LinqUtil
  {

    public static void Shuffle<T>(this IList<T> list)
    {
      var rng = new Random(DateTime.Now.Millisecond);
      
      var n = list.Count;
      //Fisher–Yates shuffle
      while (n > 1)
      {
        n--;
        int k = rng.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }

    /// <summary>
    /// Retorna o elemento do enumerável que possui o valor mínimo dentre todos os elementos de acordo com a função <paramref name="elFunc"/>.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Se o enumerável for vazio.</exception>
    public static T WhereMin<T, TComp>(this IEnumerable<T> enumeravel, Func<T, TComp> elFunc)
      where TComp : IComparable<TComp>
    {
      if (enumeravel == null)
        throw new ArgumentNullException("enumeravel");
      if (elFunc == null)
        throw new ArgumentNullException("elFunc");

      return enumeravel.Aggregate((T acc, T el) => elFunc(acc).CompareTo(elFunc(el)) <= 0 ? acc : el);
    }

    public static double StdDev<T>(this IEnumerable<T> enumerable, Func<T, double> selector)
    {
      if (enumerable == null)
        throw new ArgumentException("enumerable");      

      int n = 0;
      double sumX = 0;
      double sumXSquared = 0;
      double variance = 0;

      foreach (var item in enumerable)
      {
        n++;
        var currentValue = selector(item);
        sumX += currentValue;
        sumXSquared += currentValue * currentValue;
      }

      if(n > 1)
        variance = 1.0 / n * (sumXSquared - 1.0 / n * sumX * sumX);
        

      return Math.Sqrt(variance);
    }
  }
}
