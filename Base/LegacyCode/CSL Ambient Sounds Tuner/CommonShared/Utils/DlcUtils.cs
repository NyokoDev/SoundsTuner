using System;

namespace AmbientSoundsTuner2.CommonShared.Utils
{
    /// <summary>
    /// Contains various utilities regarding DLCs.
    /// </summary>
    public static class DlcUtils
    {
        public static Dlc InstalledDlcs
        {
            get
            {
                Dlc dlcs = Dlc.None;
                if (IsAfterDarkInstalled) dlcs |= Dlc.AfterDark;
                if (IsSnowfallInstalled) dlcs |= Dlc.Snowfall;
                if (IsNaturalDisastersInstalled) dlcs |= Dlc.NaturalDisasters;
                if (IsSunsetHarborInstalled) dlcs |= Dlc.SunsetHarbor;
                if (IsCampusInstalled) dlcs |= Dlc.Campus;
                if (IsGreenCitiesInstalled) dlcs |= Dlc.GreenCities;
                if (IsIndustriesInstalled) dlcs |= Dlc.Industries;
                if (IsMassTransitInstalled) dlcs |= Dlc.MassTransit;
                if (IsParkLifeInstalled) dlcs |= Dlc.ParkLife;
                if (IsPlazasPromenadesInstalled) dlcs |= Dlc.PlazasPromenades;
                if (IsAirportsInstalled) dlcs |= Dlc.Airports;
                if (IsFinancialDistrictsInstalled) dlcs |= Dlc.FinancialDistricts;
                if (IsHotelInstalled) dlcs |= Dlc.Hotel;

                return dlcs;
            }
        }

        public static bool IsAfterDarkInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.AfterDarkDLC);
        public static bool IsSnowfallInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.SnowFallDLC);
        public static bool IsNaturalDisastersInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.NaturalDisastersDLC);
        public static bool IsSunsetHarborInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.UrbanDLC);
        public static bool IsCampusInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.CampusDLC);
        public static bool IsGreenCitiesInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.GreenCitiesDLC);
        public static bool IsIndustriesInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.IndustryDLC);
        public static bool IsMassTransitInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.InMotionDLC);
        public static bool IsParkLifeInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.ParksDLC);
        public static bool IsPlazasPromenadesInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.PlazasAndPromenadesDLC);
        public static bool IsAirportsInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.AirportDLC);
        public static bool IsFinancialDistrictsInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.FinancialDistrictsDLC);
        public static bool IsHotelInstalled => SteamHelper.IsDLCOwned(SteamHelper.DLC.HotelDLC);

        // Define Dlc enum values
        [Flags]
        public enum Dlc
        {
            None = 0,
            AfterDark = 1 << 0,
            Snowfall = 1 << 1,
            NaturalDisasters = 1 << 2,
            SunsetHarbor = 1 << 3,
            Campus = 1 << 4,
            GreenCities = 1 << 5,
            Industries = 1 << 6,
            MassTransit = 1 << 7,
            ParkLife = 1 << 8,
            PlazasPromenades = 1 << 9,
            Airports = 1 << 10,
            FinancialDistricts = 1 << 11,
            Hotel = 1 << 12
            // Add other DLC enum values as needed
        }
    }
}
