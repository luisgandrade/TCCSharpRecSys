using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  public interface IUnsupervisedLearning
  {

    bool iterate(IList<MovieAttribute> instance_attributes, int iteration);

  }
}
