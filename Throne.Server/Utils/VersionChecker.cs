namespace Throne.Shared.VersionsChecker;

public class VersionChecker(int expectedMajor, int expectedMinor, int expectedPatch)
{
  public bool Check(int major, int minor, int revision)
  {
    return major == expectedMajor && minor == expectedMinor && revision == expectedPatch;
  }
}