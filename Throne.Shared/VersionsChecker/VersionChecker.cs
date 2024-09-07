namespace Throne.Shared.VersionsChecker;

public class VersionChecker
{
  private readonly int expectedMajor;
  private readonly int expectedMinor;
  private readonly int expectedPatch;

  public VersionChecker(int expectedMajor, int expectedMinor, int expectedPatch)
  {
    this.expectedMajor = expectedMajor;
    this.expectedMinor = expectedMinor;
    this.expectedPatch = expectedPatch;
  }

  public bool Check(int major, int minor, int revision)
  {
    if (major != expectedMajor || minor != expectedMinor || revision != expectedPatch)
    {
      return false;
    }

    return true;
  }
}