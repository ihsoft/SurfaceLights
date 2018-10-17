## For WINDOWS users

### Prerequisites
- For building:
  - Get C# runtime of version 4.0 or higher.
  - Create a virtual drive pointing to KSP installation: `subst q: <path to KSP root>`. I.e. if `KSP.exe` lives in `S:\Steam\Kerbal Space Program\` then this is the root.
    - If you choose not to do that or the drive letter is different then you also need to change project file to the correct references and post-build actions.
- For making releases:
  - Python 2.7 or greater.
  - Owner or collaborator permissions in [Github repository](https://github.com/ihsoft/SurfaceLights).
- For development:
  - Install an open source [SharpDevelop](https://en.wikipedia.org/wiki/SharpDevelop). It will pickup existing project settings just fine but at the same time can add some new changes. Please, don't submit them into the trunk until they are really needed to build the project.
  - Get a free copy of [Visual Studio Express](https://www.visualstudio.com/en-US/products/visual-studio-express-vs). It should work but was not tested.

### Versioning
Version number consists of three numbers - X.Y.Z:
- X - MAJOR. It a really huge change to the product that may make a new vision to it. Like releasing a first version: it's always a huge change.
- Y - MINOR. Adding a new functionality or removing an old one (highly discouraged) is that kind of changes.
- Z - PATCH. Bugfixes, small feature requests, and internal cleanup changes.

### Building
- Review file `Tools\make_binary.cmd` and ensure the path to `MSBuild` is right.
- Run `Tools\make_binary.cmd` having folder `Tools` as current.
- Given there were no compile errors the new DLL file can be found in `Source\bin\Release\`.

## For iOS users

Give your own iDeas ...
