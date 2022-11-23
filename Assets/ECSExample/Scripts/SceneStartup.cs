 using System;
using System.Collections;
using System.Collections.Generic;
using ECSExample.Scripts.Static;
using UnityEngine;

public class SceneStartup : MonoBehaviour
{
    [SerializeField] private WorldProvider worldProvider;
    [SerializeField] private EntityCreator entityCreator;

    private void Awake()
    {
        worldProvider.Initialise();

       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            entityCreator.CreateEntities();
            var world = worldProvider.World;

            var entites = world.EntityCount();
            Debug.Log($"Entities {entites} exist");
        }
    }
}
