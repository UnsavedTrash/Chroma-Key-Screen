using BepInEx;
using BepInEx.Configuration;
using EmotesAPI;
using R2API;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;

namespace ExamplePlugin
{
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI")]
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency("SoundAPI", "PrefabAPI", "CommandHelper", "LoadoutAPI", "SurvivorAPI", "ResourcesAPI", "LanguageAPI", "NetworkingAPI", "UnlockAPI")]
    public class ExamplePlugin : BaseUnityPlugin
	{
        public const string PluginGUID = "com.weliveinasociety.ExampleEmotes";
        public const string PluginAuthor = "Nunchuk";
        public const string PluginName = "Example Emotes";
        public const string PluginVersion = "1.0.0";

        public static ConfigEntry<KeyboardShortcut> TPoseButton;
        string currentAnim = "";
        public void Awake()
        {
            TPoseButton = CustomEmotesAPI.instance.Config.Bind<KeyboardShortcut>("Controls", "T-Pose Button", new KeyboardShortcut(KeyCode.T), "Hold to T-Pose");
            ModSettingsManager.AddOption(new KeyBindOption(TPoseButton));
            Assets.AddBundle($"example_emotes");
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@ExampleEmotePlugin_example_emotes:assets/backflip.anim"), false);
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@ExampleEmotePlugin_example_emotes:assets/t pose.anim"), true, visible: false);
            CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
        }

        private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            currentAnim = newAnimation;
        }

        private void Update()
        {
            if (Input.GetKeyDown(TPoseButton.Value.MainKey))
            {
                CustomEmotesAPI.PlayAnimation("T Pose");
            }
            if (Input.GetKeyUp(TPoseButton.Value.MainKey))
            {
                if (currentAnim == "T Pose")
                {
                    CustomEmotesAPI.PlayAnimation("none");
                }
            }
        }
    }
}
