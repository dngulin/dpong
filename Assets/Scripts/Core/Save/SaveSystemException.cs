using System;

namespace DPong.Core.Save {
  public class SaveSystemException : Exception {
    public SaveSystemException(string msg) : base(msg) { }
  }
}