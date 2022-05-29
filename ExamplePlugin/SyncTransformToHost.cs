using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;


class SyncTransformToHost : INetMessage
{
    NetworkInstanceId netId;
    Vector3 position;
    Vector3 rotation;
    Vector3 scale;

    public SyncTransformToHost()
    {

    }

    public SyncTransformToHost(NetworkInstanceId netId, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        this.netId = netId;
        this.position = position;
        this.scale = scale;
        this.rotation = rotation;
    }

    public void Deserialize(NetworkReader reader)
    {
        //DebugClass.Log($"POSITION: {reader.Position}, SIZE: {reader.Length}");

        netId = reader.ReadNetworkId();
        position = reader.ReadVector3();
        rotation = reader.ReadVector3();
        scale = reader.ReadVector3();
    }

    public void OnReceived()
    {
        //DebugClass.Log("Pu$$y D3str0y3r");
        //if (NetworkServer.active)
        //    return;
        new SyncTransformToClient(netId, position, rotation, scale, ChromaKeyCube.ChromaKeyCubePlugin.CurrentColor).Send(R2API.Networking.NetworkDestination.Clients);
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(netId);
        writer.Write(position);
        writer.Write(rotation);
        writer.Write(scale);
    }
}