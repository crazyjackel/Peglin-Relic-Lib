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

### Config Help

Enable Test Item enables a Test Item and gives it to you when you start the game. These Items are used to make sure core systems work in mod.

Debug Log: A Number used to dictate Logging output. Based on a Flag System using LogType for numbers between 0-31.
We do this by adding the powers of 2 for each option.
Given the Options are Error = 0, Assert = 1, Warning = 2, Log = 3, and Exceptions = 4, We can calculate out the valid number for getting only Errors, Warnings, and Logs as follows:
2^Error + 2^Warning + 2^Log = 1 + 4 + 8 = 13

## Roadmap

- Relic Countdowns. (1.0.6)
- Bug Squashing. (1.0.6) 
- Relic Tagging for Compatibility w/ Lookup. (1.0.7)
- Additional Test Items (1.0.7)
- Localization from CSV File. (1.0.6)
- Better Code Commenting (1.0.6)

Links:
https://github.com/crazyjackel/Peglin-Relic-Lib
