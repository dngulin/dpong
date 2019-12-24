using PGM.ScaledNum;

namespace DPong.Level.State {
  public struct BlockersState {
    public SnVector2 LeftPosition;
    public SnVector2 RightPosition;

    public int CalculateHash() {
      return (LeftPosition.GetHashCode() * 397) ^ RightPosition.GetHashCode();
    }
  }
}