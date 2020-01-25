using System;

namespace DPong.Save {
  public class SaveSystemException : Exception {
    public SaveSystemException(string msg) : base(msg) { }
  }
}