namespace DPong.Level.State {
  public struct HitPointsState {
    public int Left;
    public int Right;

    public int CalculateCheckSum() {
      return (Left.GetHashCode() * 397) ^ Right.GetHashCode();
    }
  }
}