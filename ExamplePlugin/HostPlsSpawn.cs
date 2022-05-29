using EmotesAPI;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;


class HostPlsSpawn : INetMessage
{
    Vector3 position;
    Color colour;
    float why;
    public HostPlsSpawn()
    {




        //fuck me we needed those cooks



    }

    public HostPlsSpawn(Vector3 position, Color colour, float why)
    {
        this.position = position;
        this.colour = colour;
        this.why = why;
    }

    public void Deserialize(NetworkReader reader)
    {
        //DebugClass.Log($"POSITION: {reader.Position}, SIZE: {reader.Length}");

        colour = reader.ReadColor();
        position = reader.ReadVector3();
        why = reader.ReadSingle();
    }

    public void OnReceived()
    {
        //DebugClass.Log("Pu$$y D3str0y3r");
        //if (NetworkServer.active)
        //    return;
        if (NetworkServer.active)
        {
            if (ChromaKeyCube.ChromaKeyCubePlugin.ChromaCube)
            {
                NetworkServer.Destroy(ChromaKeyCube.ChromaKeyCubePlugin.ChromaCube);
            }
            GameObject pussy;
            pussy = CustomEmotesAPI.SpawnWorldProp(ChromaKeyCube.ChromaKeyCubePlugin.ChromaCubeInt);
            pussy.transform.localPosition = position;
            pussy.transform.localEulerAngles = new Vector3(-89.98f, why, 0f);
            NetworkServer.Spawn(pussy);
            ChromaKeyCube.ChromaKeyCubePlugin.CurrentColor = colour;
            new SyncCubeToClients(pussy.GetComponent<NetworkIdentity>().netId, colour).Send(R2API.Networking.NetworkDestination.Clients);
        }
    }

    public void Serialize(NetworkWriter writer)
    {
        writer.Write(colour);
        writer.Write(position);
        writer.Write(why);
    }
}