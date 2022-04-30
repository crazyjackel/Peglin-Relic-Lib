# Peglin Relic Lib
A Library for creating Relics in Peglin

## How to Use:
Create a Regular BepinEx Plugin and add
[BepinDependency("io.github.crazyjackel.RelicLib")] 
on top of your function.

```
RelicEffect effect = RelicRegister.Instance.RegisterRelic(new RelicDataModel()
                {
                    Rarity = RelicRarity.COMMON,
                    AssemblyPath = m_path,
                    BundlePath = "relic",
                    SpriteName = "knife",
                    LocalKey = "knifeCrit",
                    GUID = "io.github.crazyjackel.criticalknife",
                });

                //The Effect is randomly Assigned, However GUID can be used to access effect after it has been registered.
                RelicEffect effect2 = RelicRegister.Instance["io.github.crazyjackel.criticalknife"];
```
