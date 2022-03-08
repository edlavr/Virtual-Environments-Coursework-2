﻿using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using UnityEngine;


public class MoveAndSync : MonoBehaviour, IGraspable, INetworkComponent, INetworkObject
{
    [HideInInspector] public Hand grasped;
    [HideInInspector] public bool locked;
    public uint id;
    NetworkId INetworkObject.Id => new NetworkId(id);

    private NetworkContext context;

    void IGraspable.Grasp(Hand controller)
    {
        if (!locked)
        {
            grasped = controller;
        }
        
    }

    void INetworkComponent.ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        var objTransform = transform;
        
        objTransform.position = msg.Position;
        objTransform.rotation = msg.Rotation;
    }

    public void ForceRelease()
    {
        grasped = null;
        locked = true;
    }

    void IGraspable.Release(Hand controller)
    {
        grasped = null;
    }

    void Start()
    {
        context = NetworkScene.Register(this);
    }

    struct Message
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public Message(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
        }
    }
    
    void Update()
    {
        if (grasped && !locked)
        {
            var objTransform = transform;
            var handTransform = grasped.transform;
            
            objTransform.position = handTransform.position;
            objTransform.rotation = handTransform.rotation;
            
            Message message = new Message(objTransform);
            context.SendJson(message);
        }
    }
}
