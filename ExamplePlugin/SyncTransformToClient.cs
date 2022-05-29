using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;


class SyncTransformToClient : INetMessage
{
    NetworkInstanceId netId;
    Vector3 position;
    Vector3 rotation;
    Vector3 scale;
    Color color;

    public SyncTransformToClient()
    {

    }

    public SyncTransformToClient(NetworkInstanceId netId, Vector3 position, Vector3 rotation, Vector3 scale, Color color)
    {
        this.netId = netId;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.color = color;
    }

    public void Deserialize(NetworkReader reader)
    {
        //DebugClass.Log($"POSITION: {reader.Position}, SIZE: {reader.Length}");

        netId = reader.ReadNetworkId();
        position = reader.ReadVector3();
        rotation = reader.ReadVector3();
        scale = reader.ReadVector3();
        color = reader.ReadColor();
    }

    public void OnReceived()
    {
        //DebugClass.Log("Clap");
        //if (NetworkServer.active)
        //    return;


        GameObject ChromaCube = Util.FindNetworkObject(netId);
        if (!ChromaCube)
        {
            DebugClass.Log($"Body is null!!!");
        }
        ChromaCube.transform.position = position;
        ChromaCube.transform.eulerAngles = rotation;
        ChromaCube.transform.localScale = scale;
        ChromaCube.GetComponent<MeshRenderer>().material.color = color;
        //DebugClass.Log(ChromaCube);
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(netId);
        writer.Write(position);
        writer.Write(rotation);
        writer.Write(scale);
        writer.Write(color);
    }
}