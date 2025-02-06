using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Starfield.Settings;
using Random = UnityEngine.Random;

namespace Starfield.Generators
{
    public class StarfieldGenerator : MonoBehaviour
    {
        [SerializeField]
        private StarfieldSettings settings;

        private System.Random _rng;
        private float RandomRange(float min, float max)
        {
            return (float)(_rng.NextDouble() * (max - min) + min);
        }

        //Lifetime for each star so that the star travels at full depth of the spawn area
        private float _lifetime;
        
        //Caching particle array tp avoid allocations during every frame update
        private ParticleSystem.Particle[] _particles;

        void Start()
        {
            try
            {
                _rng = new System.Random(settings.Seed);
                _lifetime = settings.SpawnArea.z / settings.StarSpeed;
                //Safely stop the particle system
                settings.StarParticleSystem.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
                
                //Configuring Particle System
                var main = settings.StarParticleSystem.main;
                main.maxParticles = settings.StarCount;
                main.loop = true;
                main.playOnAwake = false;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                main.duration = _lifetime;
                main.startLifetime = _lifetime;

                var renderer = settings.StarParticleSystem.GetComponent<ParticleSystemRenderer>();
                renderer.renderMode = ParticleSystemRenderMode.Mesh;
                renderer.material = settings.StarMaterial;

                Mesh sphereMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
                renderer.mesh = sphereMesh;
                
                //Allocating the particle array based on the star count
                _particles = new ParticleSystem.Particle[settings.StarCount];
                
                //Initialise each particle with a random position and size
                for (int i = 0; i < settings.StarCount; i++)
                {
                    float x = RandomRange(-settings.SpawnArea.x, settings.SpawnArea.x);
                    float y = RandomRange(-settings.SpawnArea.y, settings.SpawnArea.y);
                    float z = RandomRange(1f, settings.SpawnArea.z);
                    
                    Vector3 pos = new Vector3(x, y, z);
                    float size = RandomRange(settings.MinStarSize, settings.MaxStarSize);
                    
                    //Set the initial properties of the star
                    _particles[i].position = pos;
                    _particles[i].startSize = size;
                    _particles[i].startColor = Color.white;
                    _particles[i].startLifetime = _lifetime;
                    _particles[i].remainingLifetime = _lifetime;
                }
                settings.StarParticleSystem.SetParticles(_particles, _particles.Length);
                settings.StarParticleSystem.Play();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in Start: " + ex);
            }
        }
        
        void Update()
        {
            try
            {
                int count = settings.StarParticleSystem.GetParticles(_particles);
                for (int i = 0; i < count; i++)
                {
                    Vector3 pos = _particles[i].position;
                    
                    //Move the particle towards the camera along the negative z-axis
                    pos.z -= settings.StarSpeed * Time.deltaTime;
                    
                    //Object pool
                    if (pos.z < 0f)
                    {
                        pos.x = RandomRange(-settings.SpawnArea.x, settings.SpawnArea.x);
                        pos.y = RandomRange(-settings.SpawnArea.y, settings.SpawnArea.y);
                        pos.z = settings.SpawnArea.z;
                    }
                    _particles[i].position = pos;
                    _particles[i].remainingLifetime = _lifetime;
                }
                settings.StarParticleSystem.SetParticles(_particles, count);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error in Update: " + ex);
            }
        }

    }
}