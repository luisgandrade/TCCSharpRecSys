namespace Lib
{
  public class MovieAttribute
  {

    public int movie_id { get; private set; }

    public int attribute_id { get; private set; }

    public double value { get; private set; }

    public double? normalized_value { get; set; }

    public MovieAttribute(int movie_id, int attribute_id, double value)
    {
      this.movie_id = movie_id;
      this.attribute_id = attribute_id;
      this.value = value;
    }
    
  }
}
