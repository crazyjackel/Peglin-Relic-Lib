# Peglin Relic Lib
A Library for creating Relics in Peglin

## How to Use:
Create a Regular BepinEx Plugin and add
[BepinDependency("io.github.crazyjackel.RelicLib")] 
on top of your function.

To Create A Relic, create a Data Model for Relics
Make sure to call SetAssemblyPath.
Make sure you set GUID for your Relic.

```
RelicDataModel model = new RelicDataModel()
{
    Rarity = RelicRarity.COMMON,
    BundlePath = "relic",
    SpriteName = "knife",
    LocalKey = "knifeCrit",
    GUID = "io.github.crazyjackel.criticalknife",
};
model.SetAssemblyPath(this);
```

Once your Model is created, it is time to register it.
```
RelicEffect effect = RelicRegister.Instance.RegisterRelic(model);
```

Your RelicEffect is your assigned Relic Enum, there is no guarentee of it being a specific value.
You can also access your Relic Effect via:
```
RelicEffect effect = RelicRegister.Instance[GUID];
```
GUIDs are guarenteed to get the same value after registration.

You can now make patches wherein you check whether the relicEffect is active according to the RelicManager. You are responsible for getting your own Relic Manager Instances.
```
if (relicManager.RelicEffectActive(effect){
//Do Something
}
```
