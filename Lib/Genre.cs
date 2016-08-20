using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
  [Flags]
  public enum Genre
  {
    [Description("Unknown")]
    unknown = 0,
    [Description("Action")]
    action = 1,
    [Description("Aventure")]
    adventure = 2,
    [Description("Animation")]
    animation = 4,
    [Description("Children")]
    children = 8,
    [Description("Comedy")]
    comedy = 16,
    [Description("Crime")]
    crime = 32,
    [Description("Documentary")]
    documentary = 64,
    [Description("Drama")]
    drama = 128,
    [Description("Fantasy")]
    fantasy = 256,
    [Description("Film-noir")]
    film_noir = 512,
    [Description("Horror")]
    horror = 1024,
    [Description("Musical")]
    musical = 2048,
    [Description("Mystery")]
    mystery = 4096,
    [Description("Romance")]
    romance = 8192,
    [Description("Sci-fi")]
    sci_fi = 16384,
    [Description("Thriller")]
    thriller = 32768,
    [Description("War")]
    war = 65536,
    [Description("Western")]
    western = 131072,

  }
}
