namespace DPong.Meta.Validation {
  public static class PlayerDataValidator {
    private const int NaxNickLength = 10;
    private const string DefaultNickName = "Player";

    public static string ValidateNickName(string name) {
      name = name.Trim();

      if (string.IsNullOrEmpty(name))
        return DefaultNickName;

      if (name.Length > NaxNickLength)
        return name.Substring(0, NaxNickLength);

      return name;
    }
  }
}