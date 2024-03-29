﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AmbientSoundsTuner2.CommonShared.Configuration;
using AmbientSoundsTuner2.Migration;

namespace AmbientSoundsTuner2.SoundPack.Migration
{
    public class SoundPacksFileMigrator : ConfigMigratorBase<SoundPacksFile>
    {
        public SoundPacksFileMigrator()
        {
            this.MigrationMethods = new Dictionary<uint, Func<object, object>>()
            {
                { 1, this.MigrateFromVersion1 },
            };

            this.VersionTypes = new Dictionary<uint, Type>()
            {
                { 1, typeof(SoundPacksFileV1) },
            };
        }

        protected object MigrateFromVersion1(object oldFile)
        {
            SoundPacksFileV1 file = (SoundPacksFileV1)oldFile;
            SoundPacksFile newFile = new SoundPacksFile();

            var soundPacks = new List<SoundPacksFileV1.SoundPack>();
            foreach (var soundPack in file.SoundPacks)
            {
                var newSoundPack = new SoundPacksFileV1.SoundPack();
                newSoundPack.Name = soundPack.Name;
                newSoundPack.Ambients = soundPack.Ambients;
                newSoundPack.Animals = soundPack.Animals;
                var buildings = new List<SoundPacksFileV1.Audio>();
                foreach (var building in soundPack.Buildings)
                {
                    switch (building.Type)
                    {
                        case "Coal Power Plant":
                            buildings.Add(new SoundPacksFileV1.Audio()
                            {
                                Type = "Oil Power Plant",
                                Name = building.Name,
                                AudioInfo = building.AudioInfo
                            });
                            break;
                        case "Water Outlet":
                            buildings.Add(new SoundPacksFileV1.Audio()
                            {
                                Type = "Water Treatment Plant",
                                Name = building.Name,
                                AudioInfo = building.AudioInfo
                            });
                            break;
                    }
                    buildings.Add(new SoundPacksFileV1.Audio()
                    {
                        Type = building.Type,
                        Name = building.Name,
                        AudioInfo = building.AudioInfo
                    });
                }
                newSoundPack.Buildings = buildings.ToArray();
                var vehicles = new List<SoundPacksFileV1.Audio>();
                foreach (var vehicle in soundPack.Vehicles)
                {
                    string type;
                    switch (vehicle.Type)
                    {
                        case "Aircraft Movement":
                            type = "Aircraft Sound";
                            break;
                        default:
                            type = vehicle.Type;
                            break;
                    }
                    switch (vehicle.Type)
                    {
                        case "Small Car Sound":
                            vehicles.Add(new SoundPacksFileV1.Audio()
                            {
                                Type = "Scooter Sound",
                                Name = vehicle.Name,
                                AudioInfo = vehicle.AudioInfo
                            });
                            break;
                    }
                    vehicles.Add(new SoundPacksFileV1.Audio()
                    {
                        Type = type,
                        Name = vehicle.Name,
                        AudioInfo = vehicle.AudioInfo
                    });
                }
                newSoundPack.Vehicles = vehicles.ToArray();
                newSoundPack.Miscs = soundPack.Miscs;

                soundPacks.Add(newSoundPack);
                
            }
            newFile.SoundPacks = soundPacks.ToArray();

            return newFile;
        }
    }
}
