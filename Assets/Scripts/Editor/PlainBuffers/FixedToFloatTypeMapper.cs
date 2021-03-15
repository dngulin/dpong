using System.Collections.Generic;
using PlainBuffers;

namespace DPong.Editor.PlainBuffers {
  public class FixedToFloatTypeMapper : ITypeMapper {
    public string RemapNamespace(string ns) => "DPong.Level.View";

    public string RemapEnumName(string enumName) => enumName;
    public string RemapArrayName(string arrayName) => arrayName.Replace("State", "ViewState");
    public string RemapStructName(string structName) => structName.Replace("State", "ViewState");

    public string RemapMemberType(string memberType, Dictionary<string, string> remappedTypes) {
      if (remappedTypes.TryGetValue(memberType, out var remappedType))
        return remappedType;

      switch (memberType) {
        case "FxNum": return "float";
        case "FxVec2": return "Vector2";
        case "FxRandomState": return memberType;
      }

      return memberType.Replace("State", "ViewState");
    }

    public string RemapMemberDefaultValue(string memberType, string value) {

      switch (memberType) {
        case "FxVec2": return value.ToLower();
      }

      return value;
    }
  }
}