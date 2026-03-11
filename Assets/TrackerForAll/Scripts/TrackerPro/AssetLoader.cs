
using System.Collections;

namespace TrackerPro.Unity
{
  public static class AssetLoader
  {
    private static ResourceManager _ResourceManager;

    public static void Provide(ResourceManager manager)
    {
      _ResourceManager = manager;
    }

    public static IEnumerator PrepareAssetAsync(string name, string uniqueKey, bool overwrite = false)
    {
      if (_ResourceManager == null)
      {
#if UNITY_EDITOR
        Logger.LogWarning("ResourceManager is not provided, so default LocalResourceManager will be used");
        _ResourceManager = new LocalResourceManager();
#else
        throw new System.InvalidOperationException("ResourceManager is not provided");
#endif
      }
      return _ResourceManager.PrepareAssetAsync(name, uniqueKey, overwrite);
    }

    public static IEnumerator PrepareAssetAsync(string name, bool overwrite = false)
    {
      return PrepareAssetAsync(name, name, overwrite);
    }
  }
}
