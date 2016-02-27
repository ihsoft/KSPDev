*** For WINDOWS users

To not deal with project settings make a drive Q: which points to the game's folder. Then, just load the project and it will pickup all the settings.
  subst Q: <your KSP folder path>

E.g. if you have installed your game into:
  D:\Steam\steamapps\common\Kerbal Space Program\
do:
  subst Q: "D:\Steam\steamapps\common\Kerbal Space Program\"


By default on build completion the binary is copied into:
  "Q:\GameData\KSPDev\Plugins"
But only if path "Q:\GameData" exists. So, if you haven't did the subst then there will be no the convinience option.


For the purpose of debugging you may want to copy filters and overrides there.
  cd <your git repository>
  copy LogConsole-filters.cfg Q:\GameData\KSPDev\Plugins
  copy LogInterceptor-source-overrides.cfg Q:\GameData\KSPDev\Plugins

If you don't do this then your newly built binary will use deafult settings.



*** For iOS users

Give your own iDeas ...