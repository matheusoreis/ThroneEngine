namespace Throne.Shared.VersionsChecker;

public class VersionChecker(int expectedMajor, int expectedMinor, int expectedRevision)
{
  private readonly int expectedMajor = expectedMajor;
  private readonly int expectedMinor = expectedMinor;
  private readonly int expectedRevision = expectedRevision;

  public bool CheckAndAlert(int major, int minor, int revision)
  {
    if (major != expectedMajor || minor != expectedMinor || revision != expectedRevision) return false;

    return true;
  }
}
