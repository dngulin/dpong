namespace DPong.Meta.Navigation {
  public readonly struct NavigationToken {
    public readonly uint Id;
    public NavigationToken(uint id) => Id = id;
  }
}