using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  public interface IClustering
  {

    bool iterate(IList<MovieAttribute> instanceAttributes, int iteration);

  }
}
