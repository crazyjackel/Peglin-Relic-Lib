# Peglin Relic Lib
A Library for creating Relics in Peglin

## How to Use:
Create a Regular BepinEx Plugin and add
[BepinDependency("io.github.crazyjackel.RelicLib")] 
on top of your Plugin and add a reference to your Project.

### Create the Model
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

### Register the Model
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

### Patch with the Effect
You can now make patches wherein you check whether the relicEffect is active according to the RelicManager. You are responsible for getting your own Relic Manager Instances.
```
if (relicManager.RelicEffectActive(effect){
    //Do Something
}
```

### Support Localization
To Add Localization for your Relics, use the built-in LocalizationRegister.
Your Model will include Name and Description Terms, just fill in the blanks and the rest will be taken care of.
```
LocalizationRegister.ImportTerm(new TermDataModel(model.NameTerm)
{
    English = "Critical Knife",
    French = "Dague Pointue"
});
```

### Save Gameplay Information
During the Game, you might want to save certain data as a part of your Save run, such as whether certain relics should have additional special properties.
Or whether certain events were completed to upgrade your relics.
Unfortunately, the Game's native solution for saving gets recognized by Steam as corrupting your Save Path.
Instead, Try Using the ModdedDataSerializer:

Loading Data:
```
if (ModdedDataSerializer.HasKey("io.github.crazyjackel.eventLifeLost"))
{
    int eventLifeLost = ModdedDataSerializer.Load<List<RelicEffect>>("io.github.crazyjackel.eventLifeLost");
    return eventLifeLost;
}
```
Saving Data:
```
ModdedDataSerializer.Save("io.github.crazyjackel.eventLifeLost", eventLifeLost);
```
