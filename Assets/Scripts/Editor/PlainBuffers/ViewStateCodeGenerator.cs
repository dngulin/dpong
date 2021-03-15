using PlainBuffers.CodeGen;
using PlainBuffers.CodeGen.Data;
using PlainBuffers.Generators;

namespace DPong.Editor.PlainBuffers {
  public class ViewStateCodeGenerator : CSharpUnityCodeGenerator {
    private const string CastMethod = "ToViewState";

    public ViewStateCodeGenerator(string[] namespaces) : base(namespaces) {
    }

    protected override void WriteEnum(CodeGenEnum enumType, in BlockWriter nsBlock) {
      // Skip enums
    }

    protected override void WriteArray(CodeGenArray arrayType, in BlockWriter nsBlock) {
      base.WriteArray(arrayType, in nsBlock);
      nsBlock.WriteLine();

      var origType = arrayType.Name.Replace("ViewState", "State");

      using (var typeBlock = nsBlock.Sub($"public static class {origType}ViewExtensions"))
      using (var methodBlock = typeBlock.Sub($"public static {arrayType.Name} {CastMethod}(this in {origType} value)")) {
        methodBlock.WriteLine($"var result = new {arrayType.Name}();");

        using (var forBlock = methodBlock.Sub($"for (var i = 0; i < {arrayType.Name}.Length; i++)")) {
          var castMethod = GetCastingMethodName(arrayType.ItemType);
          forBlock.WriteLine(castMethod == null ?"result[i] = value[i];" : $"result[i] = value[i].{castMethod}();");
        }

        methodBlock.WriteLine("return result;");
      }
    }

    protected override void WriteStruct(CodeGenStruct structType, in BlockWriter nsBlock) {
      base.WriteStruct(structType, in nsBlock);
      nsBlock.WriteLine();

      var origType = structType.Name.Replace("ViewState", "State");

      using (var typeBlock = nsBlock.Sub($"public static class {origType}ViewExtensions"))
      using (var methodBlock = typeBlock.Sub($"public static {structType.Name} {CastMethod}(this in {origType} value)")) {
        methodBlock.WriteLine($"var result = new {structType.Name}();");

        foreach (var field in structType.Fields) {
          var castMethod = GetCastingMethodName(field.Type);
          methodBlock.WriteLine(castMethod == null ?
            $"result.{field.Name} = value.{field.Name};" :
            $"result.{field.Name} = value.{field.Name}.{castMethod}();");
        }

        methodBlock.WriteLine("return result;");
      }
    }

    private static string GetCastingMethodName(string viewTypeName) {
      if (viewTypeName.Contains("ViewState"))
        return CastMethod;

      switch (viewTypeName) {
        case "float": return "ToFloat";
        case "Vector2": return "ToVector2";
      }

      return null;
    }
  }
}