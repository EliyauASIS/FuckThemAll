
namespace TrackerPro.Unity
{
  [System.Serializable]
  public enum RunningMode
  {
    Async,
    NonBlockingSync,
    Sync,
  }

  public static class RunningModeExtension
  {
    public static bool IsSynchronous(this RunningMode runningMode)
    {
      return runningMode == RunningMode.Sync || runningMode == RunningMode.NonBlockingSync;
    }
  }
}
