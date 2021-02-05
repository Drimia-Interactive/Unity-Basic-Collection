# Unity Basic Collection
Unity Basic Collection is a Unity package that contains basic scripts for every project.

## Installing

#### Requirements
[Git](https://git-scm.com/) must be installed and added to your path.

#### Setup - Package Manager window
From Unity 2019.3 we can [install packages from git](https://docs.unity3d.com/Manual/upm-ui-giturl.html "Installing from a Git URL")
Just add the following URL according to the manual:

```
https://github.com/Drimia-Interactive/Unity-Basic-Collection.git
```

## Usage
* OnBuild event function - you can use it in any MonoBehaviour like Start/Update functions, and it will be called at the build time for every GameObject in any build scene.
example:
```csharp
public class OnBuildExample : MonoBehaviour
{
    void OnBuild()
    {
        Debug.Log($"OnBuild for {name}");
    }
}
```