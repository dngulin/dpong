using DPong.Common;
using FxNet.Math;

namespace DPong.Level.State {
  public struct BlockersState {
    public FxVec2 LeftPosition;
    public FxVec2 RightPosition;

    public int CalculateCheckSum() {
      return (LeftPosition.CalculateCheckSum() * 397) ^ RightPosition.CalculateCheckSum();
    }
  }
}