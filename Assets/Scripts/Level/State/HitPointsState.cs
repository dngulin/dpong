namespace DPong.Level.State {
  public struct HitPointsState {
    public int Left;
    public int Right;

    public int CalculateHash() {
      return (Left.GetHashCode() * 397) ^ Right.GetHashCode();
    }
  }
}