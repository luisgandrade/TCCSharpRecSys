using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnsupervisedLearning.SelfOrganizingMaps.Network;

namespace UnsupervisedLearning.SelfOrganizingMaps.NeighborhoodFunctions
{
  public class LinearNeighborhood : INeighborhoodFunction
  {
    private double initial_neighborhood_width;
    private double time_constant;

    private string _print;

    public string print
    {
      get
      {
        if (string.IsNullOrWhiteSpace(_print))
          _print = "linear_" + Regex.Split(initial_neighborhood_width.ToString(), "([0-9]+)\\.([0-9]+)").Last() + "_" + Regex.Split(time_constant.ToString(), "([0-9]+)\\.([0-9]+)").Last();
        return _print;
      }
    }

    public double apply(Neuron bmu, Neuron neighbor, int iteration)
    {
      throw new NotImplementedException();
    }


    public LinearNeighborhood(double initialNeighborhoodWidth, double timeConstant)
    {
      if (initialNeighborhoodWidth <= 0)
        throw new InvalidOperationException("Largura inicial da vizinhança deve ser maior que zero.");

      this.initial_neighborhood_width = initialNeighborhoodWidth;
      this.time_constant = timeConstant / Math.Log(initialNeighborhoodWidth);
    }

    public LinearNeighborhood(double initialWidth)
      : this(initialWidth, 1000) { }
    
  }
}
