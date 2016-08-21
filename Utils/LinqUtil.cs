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
  }
}
