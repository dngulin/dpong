namespace DPong.Level.State {
  public static class LevelStateCheckSumExtensions {
    public static unsafe int CalculateCheckSum(in this LevelState state) {
      // MurMurHash3
      const int length = LevelState.SizeOf / sizeof(uint); // LevelState always have paddings
      var h = 180591u; // seed

      unchecked {
        fixed (LevelState* ptr = &state) {
          var data = (uint*) ptr;

          for (var i = 0; i < length; i++) {
            var k = data[i];

            k *= 0xcc9e2d51;
            k = (k << 15) | (k >> 17);
            k *= 0x1b873593;

            h ^= k;
            h = (h << 13) | (h >> 19);
            h = h * 5 + 0xe6546b64;
          }
        }

        h ^= LevelState.SizeOf;
        h ^= h >> 16;
        h *= 0x85ebca6b;
        h ^= h >> 13;
        h *= 0xc2b2ae35;
        h ^= h >> 16;

        return (int) h;
      }
    }
  }
}