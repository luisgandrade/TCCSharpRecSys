using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning
{
  public class InstanceClassification
  {

    public int instance_id { get; private set; }

    public int x { get; private set; }

    public int y { get; private set; }

    public InstanceClassification(int instance_id, int x, int y)
    {
      if (instance_id <= 0 || x < 0 || y < 0)
        throw new InvalidOperationException("instance_id <= 0 || x < 0 || y < 0");

      this.instance_id = instance_id;
      this.x = x;
      this.y = y;
    }

  }
}
