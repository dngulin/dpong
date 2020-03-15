using System.Collections.Generic;
using DPong.InputSource.Data;

namespace DPong.InputSource.Extensions {
  public static class ReadonlyDescriptorListExtensions {
    public static int IndexOf(this IReadOnlyList<InputSourceDescriptor> list, InputSourceDescriptor descriptor) {
      for (var i = 0; i < list.Count; i++)
        if (list[i] == descriptor) return i;

      return -1;
    }
  }
}