using System.IO;
using PlainBuffers.CodeGen;
using PlainBuffers.Generators;

namespace DPong.Editor.PlainBuffers
{
  public class UnityCodeGenerator : CSharpUnsafeStructsGenerator {
    public UnityCodeGenerator(string[] namespaces) : base(namespaces) {}

    protected override void WriteHeader(TextWriter writer) {
      base.WriteHeader(writer);
      writer.WriteLine("using Unity.Collections.LowLevel.Unsafe;");
      writer.WriteLine();
    }

    protected override void WriteEqualityOperators(string type, in BlockWriter typeBlock) {
      using (var eqBlock = typeBlock.Sub($"public static bool operator ==(in {type} l, in {type} r)")) {
        using (var rFxd = eqBlock.Sub("fixed (byte* __l = l._buffer, __r = r._buffer)")) {
          rFxd.WriteLine("return UnsafeUtility.MemCmp(__l, __r, SizeOf) == 0;");
        }
      }

      typeBlock.WriteLine($"public static bool operator !=(in {type} l, in {type} r) => !(l == r);");
      typeBlock.WriteLine();
      typeBlock.WriteLine($"public override bool Equals(object obj) => obj is {type} casted && this == casted;");
      typeBlock.WriteLine("public override int GetHashCode() => throw new NotSupportedException();");
    }
  }
}
