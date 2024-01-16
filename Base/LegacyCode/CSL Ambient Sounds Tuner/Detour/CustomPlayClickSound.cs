using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using AmbientSoundsTuner2.CommonShared.Utils;
using AmbientSoundsTuner2.Sounds;
using ColossalFramework.UI;

namespace AmbientSoundsTuner2.Detour
{
    /// <summary>
    /// This static class detours the calls for playing click sounds so we can have our own volume level.
    /// </summary>
    public static class CustomPlayClickSound
    {
        private static readonly MethodInfo playClickSoundOriginal = typeof(UIComponent).GetMethod("PlayClickSound", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo playClickSoundReplacement = typeof(CustomPlayClickSound).GetMethod("PlayClickSound");
        private static DetourCallsState playClickSoundState;
        private static readonly MethodInfo playDisabledClickSoundOriginal = typeof(UIComponent).GetMethod("PlayDisabledClickSound", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly MethodInfo playDisabledClickSoundReplacement = typeof(CustomPlayClickSound).GetMethod("PlayDisabledClickSound");
        private static DetourCallsState playDisabledClickSoundState;

        public static void Detour()
        {
            try
            {
                playClickSoundState = DetourUtils.RedirectCalls(playClickSoundOriginal, playClickSoundReplacement);
           
            }
            catch (Exception ex)
            {
           
            }

            try
            {
                playDisabledClickSoundState = DetourUtils.RedirectCalls(playDisabledClickSoundOriginal, playDisabledClickSoundReplacement);
          
            }
            catch (Exception ex)
            {
              
            }
        }

        public static void UnDetour()
        {
            try
            {
                DetourUtils.RevertRedirect(playClickSoundOriginal, playClickSoundState);
              
            }
            catch (Exception ex)
            {
              
            }

            try
            {
                DetourUtils.RevertRedirect(playDisabledClickSoundOriginal, playDisabledClickSoundState);
             
            }
            catch (Exception ex)
            {

            }
        }

        private static float uiClickSoundVolume = 1;
        public static float UIClickSoundVolume
        {
            get { return uiClickSoundVolume; }
            set { uiClickSoundVolume = value; }
        }

        public static void PlayClickSound(UIComponent @this, UIComponent comp)
        {
            if (@this.playAudioEvents && comp == @this && UIView.playSoundDelegate != null)
            {
                if (@this.clickSound != null)
                {
                    UIView.playSoundDelegate(@this.clickSound, UIClickSoundVolume);
                    return;
                }
                if (@this.GetUIView().defaultClickSound != null)
                {
                    UIView.playSoundDelegate(@this.GetUIView().defaultClickSound, UIClickSoundVolume);
                }
            }
        }

        private static float disabledUIClickSoundVolume = 1;
        public static float DisabledUIClickSoundVolume
        {
            get { return disabledUIClickSoundVolume; }
            set { disabledUIClickSoundVolume = value; }
        }

        public static void PlayDisabledClickSound(UIComponent @this, UIComponent comp)
        {
            if (@this.playAudioEvents && comp == @this && UIView.playSoundDelegate != null)
            {
                if (@this.disabledClickSound != null)
                {
                    UIView.playSoundDelegate(@this.disabledClickSound, DisabledUIClickSoundVolume);
                    return;
                }
                if (@this.GetUIView().defaultDisabledClickSound != null)
                {
                    UIView.playSoundDelegate(@this.GetUIView().defaultDisabledClickSound, DisabledUIClickSoundVolume);
                }
            }
        }
    }
}
