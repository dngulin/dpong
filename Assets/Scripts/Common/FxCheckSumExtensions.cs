using FxNet.Math;
using FxNet.Random;

namespace DPong.Common {
  public static class FxCheckSumExtensions {
    public static int CalculateCheckSum(this in FxNum value) => value.Raw.GetHashCode();
    public static int CalculateCheckSum(this in FxVec2 value) => (value.X.CalculateCheckSum() * 397) ^ value.Y.CalculateCheckSum();
    public static int CalculateCheckSum(this in FxRandomState value) => -1; // TODO: Impl
  }
}