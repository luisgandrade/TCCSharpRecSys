using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnsupervisedLearning.SelfOrganizingMaps.Network
{
  public class Coordinate
  {

    public int x { get; private set; }

    public int y { get; private set; }    

    public Coordinate(int x, int y)
    {
      if (x < 0 || y < 0)
        throw new ArgumentException("x < 0 || y < 0");

      this.x = x;
      this.y = y;
    }    

    public bool Equals(Coordinate other)
    {
      throw new NotImplementedException();
    }
  }
}
