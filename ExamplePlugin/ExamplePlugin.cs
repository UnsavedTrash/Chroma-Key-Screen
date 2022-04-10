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
            ModSettingsManager.SetModIcon(Assets.Load<Sprite>("@ExampleEmotePlugin_example_emotes:assets/icon.png"));
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@ExampleEmotePlugin_example_emotes:assets/backflip.anim"), false); //Most standard emote here, non-looping, no sounds
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@ExampleEmotePlugin_example_emotes:assets/t pose.anim"), true, visible: false); //Similar to previous, but this is a looping emote which is also hidden from the regular emote picker. Has to be invoked from a function as shown below.
            CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged; //Lets you know when a new emote is played and it's name

            //CustomEmotesAPI.CreateNameTokenSpritePair("customSurvivorNameToken", sprite);    For the circle in the middle when you are choosing an emote. 



            //CustomEmotesAPI.GetLocalBodyAnimationClip()      If you don't wanna just use the hook used above.


            //https://youtu.be/c_G3G4RzCFA        a walkthrough of importing a bodyprefab into game. All vanilla survivors are already imported, this is for modded characters
            //If you want to import your custom survivor so it can use the emotes. bodyPrefab is just that. underskeleton is essentially a copy of the skeleton from the bodyPrefab except it HAS to be a humanoid skeleton in unity.
            //I know this one is weird, feel free to @ me in discord if you need help with this. You will need access to the bodyPrefab used in game and we should be able to go from there.    
            //On.RoR2.SurvivorCatalog.Init += (orig) =>      
            //{
            //    orig();
            //    foreach (var item in SurvivorCatalog.allSurvivorDefs)
            //    {
            //        DebugClass.Log($"----------bodyprefab: [{item.bodyPrefab}]");
            //        if (item.bodyPrefab.name == "MageBody")
            //        {
            //            var skele = Assets.Load<GameObject>("@ExampleEmotePlugin_example_emotes:assets/artificer.prefab");
            //            CustomEmotesAPI.ImportArmature(item.bodyPrefab, underskeleton);
            //        }
            //    }
            //};




            //more examples here, animation setup from moisture upset. Some stuff here will cause errors due to missing resources so leave it commented
            /*
            All possible inputs for a AddCustomAnimation:
            AnimationClip animationClip                     //Default animation
            bool looping                                    //Whether or not animationClip loops
            string _wwiseEventName = ""                     //Event to post when animation starts
            string _wwiseStopEvent = ""                     //Event to post when animation stops
            HumanBodyBones[] rootBonesToIgnore = null       //All bones specified and any child bones will be ignored by the animation
            HumanBodyBones[] soloBonesToIgnore = null       //All bones specified will be ignored by the animation
            AnimationClip secondaryAnimation = null         //Animation to play after the primary animation. Use this if you have a non-looping-into-looping animation
            bool dimWhenClose = false                       //Create an audio dimming sphere around the emotee which will dim normal music when you approach them
            bool stopWhenMove = false                       //Stops the animation if moving
            bool stopWhenAttack = false                     //Stops the animation if attacking
            bool visible = true                             //Dictates if emote will show up in the normal list.
            bool syncAnim = false                           //Dictates if emote will sync the animation.
            bool syncAudio = false                          //Dictates if emote will sync audio (requires wwise start and stop events)




            You can declare parts of a humanoid body. Insert these declared parts into a function and you can have bones be ignored by animations, like if you have an upper-body only animation
            HumanBodyBones[] upperLegs = new HumanBodyBones[] { HumanBodyBones.LeftUpperLeg, HumanBodyBones.RightUpperLeg };
            HumanBodyBones[] hips = new HumanBodyBones[] { HumanBodyBones.Hips };
            HumanBodyBones[] Facepalm = new HumanBodyBones[] { HumanBodyBones.LeftUpperLeg, HumanBodyBones.RightUpperLeg, HumanBodyBones.LeftUpperArm };



            Yes, we use wwise here :)
            EnemyReplacements.LoadBNK("Emotes");
            



            Loser anim, loops first anim, wwise start event, wwise stop event, dims audio when close
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/Loser.anim"), true, "Loser", "LoserStop", dimWhenClose: true);
            
            
            
            Headspin anim, doesn't loop first anim, secondary anim which loops
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/HeadSpin.anim"), false, secondaryAnimation: Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/headspinloop.anim"));
            
            
            
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/Dab.anim"), false, "Dab", "DabStop", upperLegs, hips);
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/Floss.anim"), true, "Floss", "FlossStop", dimWhenClose: true);
            I do this one differently because I'm special
            var test = Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/Default Dance.anim");
            CustomEmotesAPI.AddCustomAnimation(test, false, "DefaultDance", "DefaultDanceStop", dimWhenClose: true);
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/Facepalm.anim"), false, "", "", Facepalm, hips);
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/Orange Justice.anim"), true, "OrangeJustice", "OrangeJusticeStop", dimWhenClose: true);
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/nobones.anim"), true);
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/ThumbsUp.anim"), false, rootBonesToIgnore: upperLegs, soloBonesToIgnore: hips);
            CustomEmotesAPI.AddCustomAnimation(Assets.Load<AnimationClip>("@MoistureUpset_moisture_animationreplacements:assets/animationreplacements/ThumbsDown.anim"), false, rootBonesToIgnore: upperLegs, soloBonesToIgnore: hips);
             */
        }

        private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            currentAnim = newAnimation;
        }

        bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
        bool GetKeyUp(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (Input.GetKeyUp(item))
                {
                    return true;
                }
            }
            return Input.GetKeyUp(entry.Value.MainKey);
        }
        private void Update()
        {
            if (GetKeyPressed(TPoseButton))
            {
                CustomEmotesAPI.PlayAnimation("T Pose"); //You can call animations manually, even if they are hidden like here. Consider naming your hidden emotes something very unique so there isn't any conflicts
            }
            else if (GetKeyUp(TPoseButton))
            {
                if (currentAnim == "T Pose")
                {
                    CustomEmotesAPI.PlayAnimation("none");
                }
            }
        }
    }
}
