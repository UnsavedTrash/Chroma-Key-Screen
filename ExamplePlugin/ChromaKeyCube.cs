using BepInEx;
using BepInEx.Configuration;
using EmotesAPI;
using R2API;
using R2API.Networking;
using R2API.Networking.Interfaces;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ChromaKeyCube
{
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI")]
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class ChromaKeyCubePlugin : BaseUnityPlugin

    {
        public const string PluginGUID = "com.unsavedtrash.ChromaKeyCube";
        public const string PluginAuthor = "Unsaved Trash";
        public const string PluginName = "Chroma Key Cube";
        public const string PluginVersion = "1.0.0";

        public static ConfigEntry<KeyboardShortcut> North;
        public static ConfigEntry<KeyboardShortcut> South;
        public static ConfigEntry<KeyboardShortcut> West;
        public static ConfigEntry<KeyboardShortcut> East;
        public static ConfigEntry<KeyboardShortcut> Up;
        public static ConfigEntry<KeyboardShortcut> Down;
        public static ConfigEntry<KeyboardShortcut> TranslateModifier;
        public static ConfigEntry<KeyboardShortcut> RotateModifier;
        public static ConfigEntry<KeyboardShortcut> ScaleModifier;
        public static ConfigEntry<KeyboardShortcut> RotateReset;
        public static ConfigEntry<Color> ChromaCubeColor;
        internal static Vector3 SpawnRot;
        internal static Camera PlayerCam;
        private static bool CubeOwner;
        internal static Color CurrentColor;
        public enum RTSmode
        {
            None,
            Translation,
            Rotation,
            Scale
        }
        public static RTSmode CurrentMode = RTSmode.None;
        internal static int ChromaCubeInt = -1;
        public static GameObject ChromaCube;
        public void Awake()
        {
            North = Config.Bind<KeyboardShortcut>("Controls", "North Button", new KeyboardShortcut(KeyCode.I), "Rotate/Translate \"Northwards\"");
            South = Config.Bind<KeyboardShortcut>("Controls", "South Button", new KeyboardShortcut(KeyCode.K), "Rotate/Translate \"Southwards\"");
            West = Config.Bind<KeyboardShortcut>("Controls", "West Button", new KeyboardShortcut(KeyCode.J), "Rotate/Translate \"Westwards\"");
            East = Config.Bind<KeyboardShortcut>("Controls", "East Button", new KeyboardShortcut(KeyCode.L), "Rotate/Translate \"Eastwards\"");
            Up = Config.Bind<KeyboardShortcut>("Controls", "Up Button", new KeyboardShortcut(KeyCode.U), "Rotate/Translate/Scale \"Upwards\"");
            Down = Config.Bind<KeyboardShortcut>("Controls", "Down Button", new KeyboardShortcut(KeyCode.O), "Rotate/Translate/Scale \"Downwards\"");
            TranslateModifier = Config.Bind<KeyboardShortcut>("Controls", "Translate Modifier", new KeyboardShortcut(KeyCode.LeftControl), "While held, translation mode is enabled for the ChromaCube");
            RotateModifier = Config.Bind<KeyboardShortcut>("Controls", "Rotate Modifier", new KeyboardShortcut(KeyCode.LeftAlt), "While held, rotation mode is enabled for the ChromaCube");
            ScaleModifier = Config.Bind<KeyboardShortcut>("Controls", "Scale Modifier", new KeyboardShortcut(KeyCode.CapsLock), "While held, scale mode is enabled for the ChromaCube. Press \"Up\" binding to scale up and \"Down\" binding to scale down.");
            RotateReset = Config.Bind<KeyboardShortcut>("Controls", "Rotation Reset", new KeyboardShortcut(KeyCode.Backspace), "This resets rotation for the ChromaCube when pressed with the rotation modifier");
            ChromaCubeColor = Config.Bind<Color>("Greephix", "Chroma Cube Color", new Color(0, 177f / 255f, 64f / 255f), "This sets the color for the Chroma Cube :)");

            ModSettingsManager.AddOption(new KeyBindOption(North));
            ModSettingsManager.AddOption(new KeyBindOption(South));
            ModSettingsManager.AddOption(new KeyBindOption(West));
            ModSettingsManager.AddOption(new KeyBindOption(East));
            ModSettingsManager.AddOption(new KeyBindOption(Up));
            ModSettingsManager.AddOption(new KeyBindOption(Down));
            ModSettingsManager.AddOption(new KeyBindOption(TranslateModifier));
            ModSettingsManager.AddOption(new KeyBindOption(RotateModifier));
            ModSettingsManager.AddOption(new KeyBindOption(ScaleModifier));
            ModSettingsManager.AddOption(new KeyBindOption(RotateReset));
            ModSettingsManager.AddOption(new ColorOption(ChromaCubeColor));
            Assets.AddBundle($"cube");
            ModSettingsManager.SetModIcon(Assets.Load<Sprite>("assets/icon.png")); //I think I'm supposed to change this but none of the programmers are around to help :pepehands:
            CustomEmotesAPI.AddNonAnimatingEmote("Spawn ChromaCube");
            CustomEmotesAPI.BlackListEmote("Spawn ChromaCube");
            CustomEmotesAPI.animChanged += CustomEmotesAPI_animChanged;
            GameObject peepee = Assets.Load<GameObject>("Assets/Smooth Chroma Cube.prefab");
            ChromaCubeInt = CustomEmotesAPI.RegisterWorldProp(peepee, new JoinSpot[0]);
            NetworkingAPI.RegisterMessageType<SyncCubeToClients>();
            NetworkingAPI.RegisterMessageType<SyncTransformToHost>();
            NetworkingAPI.RegisterMessageType<SyncTransformToClient>();
            NetworkingAPI.RegisterMessageType<HostPlsSpawn>();
        }



        private void CustomEmotesAPI_animChanged(string newAnimation, BoneMapper mapper)
        {
            if (newAnimation == "Spawn ChromaCube")
            {
                if (!NetworkServer.active)
                {
                    ChromaCube = null;
                }
                
                CubeOwner = mapper == CustomEmotesAPI.localMapper;
                if (CubeOwner)
                {
                    //tell client to host to spawn cube of color
                    new HostPlsSpawn(mapper.transform.position, ChromaCubeColor.Value, mapper.transform.eulerAngles.y).Send(NetworkDestination.Server);
                }

                
            }
        }
#pragma warning disable IDE0051 // Remove unused private members
        void Update()
#pragma warning restore IDE0051 // Remove unused private members
        {
            if (!ChromaCube || !CubeOwner)
            {
                    return;
            }

            CurrentMode = RTSmode.None;
            Vector3 Delta = Vector3.zero;
            float ScaleDelta = 0;
            Vector3 RotationDelta = Vector3.zero;
            if (North.Value.IsPressedInclusive())
            {
                //Delta.z += 10;
                Delta += PlayerCam.transform.forward;
                RotationDelta += PlayerCam.transform.right;
            }
            if (South.Value.IsPressedInclusive())
            {
                Delta -= PlayerCam.transform.forward;
                RotationDelta -= PlayerCam.transform.right;
            }
            if (East.Value.IsPressedInclusive())
            {
                Delta += PlayerCam.transform.right;
                RotationDelta += PlayerCam.transform.forward;
            }
            if (West.Value.IsPressedInclusive())
            {
                Delta -= PlayerCam.transform.right;
                RotationDelta -= PlayerCam.transform.forward;
            }
            Delta.y = 0;
            if (Up.Value.IsPressedInclusive())
            {
                Delta.y += 1;
                ScaleDelta += 10;
                RotationDelta += PlayerCam.transform.up;
            }
            if (Down.Value.IsPressedInclusive())
            {
                Delta.y -= 1;
                ScaleDelta -= 10;
                RotationDelta -= PlayerCam.transform.up;
            }
            Delta *= Time.deltaTime * 10;
            ScaleDelta *= Time.deltaTime;

            if (TranslateModifier.Value.IsPressedInclusive())
            {
                CurrentMode = RTSmode.Translation;
            }
            if (RotateModifier.Value.IsPressedInclusive())
            {
                CurrentMode = RTSmode.Rotation;
            }
            if (ScaleModifier.Value.IsPressedInclusive())
            {
                CurrentMode = RTSmode.Scale;
            }
            switch (CurrentMode)
            {
                case RTSmode.None:
                    break;
                case RTSmode.Translation:
                    ChromaCube.transform.localPosition += Delta;
                    break;
                case RTSmode.Rotation:
                    ChromaCube.transform.localEulerAngles += RotationDelta;
                    break;
                case RTSmode.Scale:
                    ChromaCube.transform.localScale += new Vector3(ScaleDelta, ScaleDelta, ScaleDelta);
                    break;
                default:
                    break;
            }
            if (CurrentMode == RTSmode.Rotation && RotateReset.Value.IsPressedInclusive())
            {
                ChromaCube.transform.eulerAngles = SpawnRot;
            }
            if (CurrentMode != RTSmode.None)
            {
                new SyncTransformToHost(ChromaCube.GetComponent<NetworkIdentity>().netId, ChromaCube.transform.position, ChromaCube.transform.eulerAngles, ChromaCube.transform.localScale).Send(R2API.Networking.NetworkDestination.Server);

            }
            //zach isnt sure, he thinks that all clients are calling this and have authority over their own cube
        }
    }
}
