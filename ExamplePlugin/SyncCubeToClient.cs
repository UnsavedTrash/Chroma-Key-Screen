using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;


class SyncCubeToClients : INetMessage
{
    NetworkInstanceId netId;
    Color color;

    public SyncCubeToClients()
    {

    }

    public SyncCubeToClients(NetworkInstanceId netId, Color color)
    {
        this.netId = netId;
        this.color = color;
    }

    public void Deserialize(NetworkReader reader)
    {
        //DebugClass.Log($"POSITION: {reader.Position}, SIZE: {reader.Length}");

        netId = reader.ReadNetworkId();
        color = reader.ReadColor();
    }

    public void OnReceived()
    {
        //if (NetworkServer.active)
        //    return;


        GameObject ChromaCube = Util.FindNetworkObject(netId);
        if (!ChromaCube)
        {
            DebugClass.Log($"Cube is null!");
        }
        ChromaCube.GetComponent<MeshRenderer>().material.color = color;
        ChromaCube.GetComponent<MeshRenderer>().material.shader = Addressables.LoadAssetAsync<Shader>("RoR2/Base/Shaders/HGStandard.shader").WaitForCompletion();
        ChromaKeyCube.ChromaKeyCubePlugin.ChromaCube = ChromaCube;
        ChromaKeyCube.ChromaKeyCubePlugin.SpawnRot = ChromaCube.transform.localEulerAngles;
        ChromaKeyCube.ChromaKeyCubePlugin.PlayerCam = GameObject.Find("Main Camera(Clone)").GetComponent<CameraRigController>().sceneCam;
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(netId);
        writer.Write(color);
    }
}