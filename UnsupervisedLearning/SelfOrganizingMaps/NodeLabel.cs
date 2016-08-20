using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.SelfOrganizingMaps
{
  public class NodeLabel
  {

    public int x { get; private set; }

    public int y { get; private set; }

    public IList<string> labels { get; private set; }


    public NodeLabel(int x, int y, IList<string> labels)
    {
      if (labels == null)
        throw new ArgumentException("labels");

      this.x = x;
      this.y = y;
      this.labels = labels;
    }
  }
}
