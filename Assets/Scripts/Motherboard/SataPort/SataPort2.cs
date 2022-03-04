﻿using System.Collections;
using System.Collections.Generic;
using Ubiq.Messaging;
using Ubiq.XR;
using UnityEngine;


public class SataPort2 : MonoBehaviour, IGraspable, INetworkComponent, INetworkObject
{
    private Hand grasped;

    NetworkId INetworkObject.Id => new NetworkId(3196);

    private NetworkContext context;

    void IGraspable.Grasp(Hand controller)
    {
        grasped = controller;
    }

    void INetworkComponent.ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        transform.localPosition = msg.position;
    }

    void IGraspable.Release(Hand controller)
    {
        grasped = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
    }

    struct Message
    {
        public Vector3 position;
    }

    // Update is called once per frame
    void Update()
    {
        if (grasped)
        {
            transform.position = grasped.transform.position;
            Message message;
            message.position = transform.position;
            context.SendJson(message);
        }
    }
}