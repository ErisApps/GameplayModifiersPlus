﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GamePlayModifiersPlus
{
    public class Cooldowns
    {
        internal static bool globalCoolDown;
        internal static bool speedCoolDown;
        internal static bool healthCoolDown;
        internal static bool daCoolDown;
        internal static bool normalSizeCoolDown;
        internal static bool randomCoolDown;
        internal static bool noArrowsCooldown;
        internal static bool njsRandomCoolDown;
        internal static bool funkyCoolDown;
        internal static bool rainbowCoolDown;
        internal static bool bombCoolDown;
        internal static bool offsetrandomCoolDown;
        internal static bool poisonCoolDown;
        internal static bool mirrorCoolDown;
        internal static bool reverseCoolDown;
        internal static bool rotationCoolDown;
        internal static bool tunnelCoolDown;
        internal static bool mapCoolDown;
        internal static bool chargesCoolDown;
        public void SetCooldown(bool state, string cooldown)
        {
            cooldown = cooldown.ToLower();
            switch (cooldown)
            {
                case "global":
                    globalCoolDown = state;
                    break;
                case "speed":
                    speedCoolDown = state;
                    break;
                case "health":
                    healthCoolDown = state;
                    break;
                case "da":
                    daCoolDown = state;
                    break;
                case "normalsize":
                    normalSizeCoolDown = state;
                    break;
                case "random":
                    randomCoolDown = state;
                    break;
                case "noarrows":
                    noArrowsCooldown = state;
                    break;
                case "njsrandom":
                    njsRandomCoolDown = state;
                    break;
                case "funky":
                    funkyCoolDown = state;
                    break;
                case "rainbow":
                    rainbowCoolDown = state;
                    break;
                case "bombs":
                    bombCoolDown = state;
                    break;
                case "poison":
                    poisonCoolDown = state;
                    break;
                case "mirror":
                    mirrorCoolDown = state;
                    break;
                case "offsetrandom":
                    offsetrandomCoolDown = state;
                    break;
                case "reverse":
                    reverseCoolDown = state;
                    break;
                case "rotation":
                    rotationCoolDown = state;
                    break;
                case "tunnel":
                    tunnelCoolDown = state;
                    break;
                case "map":
                    mapCoolDown = state;
                    break;
                default:
                case "chargescommand":
                    chargesCoolDown = state;
                    break;
            }
        }

        public bool GetCooldown(string cooldown)
        {
            cooldown = cooldown.ToLower();
            switch (cooldown)
            {
                case "global":
                    return globalCoolDown;
                case "speed":
                    return speedCoolDown;
                case "health":
                    return healthCoolDown;
                case "da":
                    return daCoolDown;
                case "normalsize":
                    return normalSizeCoolDown;
                case "random":
                    return randomCoolDown;
                case "noarrows":
                    return noArrowsCooldown;
                case "njsrandom":
                    return njsRandomCoolDown;
                case "funky":
                    return funkyCoolDown;
                case "rainbow":
                    return rainbowCoolDown;
                case "bombs":
                    return bombCoolDown;
                case "poison":
                    return poisonCoolDown;
                case "mirror":
                    return mirrorCoolDown;
                case "offsetrandom":
                    return offsetrandomCoolDown;
                case "reverse":
                    return reverseCoolDown;
                case "rotation":
                    return rotationCoolDown;
                case "tunnel":
                    return tunnelCoolDown;
                case "chargescommand":
                    return chargesCoolDown;
                case "map":
                    return mapCoolDown;
                default:
                    return false;
            }
        }

        public void ResetCooldowns()
        {
            globalCoolDown = false;
            speedCoolDown = false;
            healthCoolDown = false;
            normalSizeCoolDown = false;
            daCoolDown = false;
            njsRandomCoolDown = false;
            rainbowCoolDown = false;
            randomCoolDown = false;
            noArrowsCooldown = false;
            funkyCoolDown = false;
            bombCoolDown = false;
            poisonCoolDown = false;
            mirrorCoolDown = false;
            reverseCoolDown = false;
            offsetrandomCoolDown = false;
            rotationCoolDown = false;
            tunnelCoolDown = false;
            mapCoolDown = false;
        }


        public static System.Collections.IEnumerator CommandCoolDown(string cooldown, float duration)
        {
            Plugin.cooldowns.SetCooldown(true, cooldown);
            yield return new WaitForSeconds(duration);
            Plugin.cooldowns.SetCooldown(false, cooldown);
        }
    }
}
