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
bool success = RelicRegister.RegisterRelic(model, out RelicEffect myEffect);
```

Your RelicEffect is your assigned Relic Enum, there is no guarentee of it being a specific value.
You can also access your Relic Effect via:
```
RelicRegister.TryGetCustomRelicEffect("myGUID", out RelicEffect myEffect);
```
GUIDs are guarenteed to get the same value after registration.

### Patch with the Effect
You can now make patches wherein you check whether the relicEffect is active according to the RelicManager. You are responsible for getting your own Relic Manager Instances.
```
if (relicManager.RelicEffectActive(effect){
    //Do Something
}
```

### Other Registers

RelicRegister is one of many Registers to deal with various Enums throughout the game. 

The Following Registers also Help:
- PegTypeRegister: Register New Peg Types and Add Support for various Pegs

### Support Localization
To Add Localization for your Relics, use the built-in LocalizationRegister.
Your Model will include Name and Description Terms, just fill in the blanks and the rest will be taken care of.
```
LocalizationHelper.ImportTerm(new TermDataModel(model.NameTerm)
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

## Roadmap

- Relic Countdowns. (2.1.0)
- Bug Squashing. (2.1.0) 
- Localization from CSV File. (2.1.0)
- Better Code Commenting (2.1.0)

Links:
https://github.com/crazyjackel/Peglin-Relic-Lib
