﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBihavior behavior;

    [Range(2, 500)]
    public int startingCount = 250;
    const float AgentDensity = 0.025f;

    [Range(0f, 100f)]
    public float driveFactor = 10f;
    [Range(0f, 10f)]
    public float maxSpeed = 5f;
    [Range(0f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplayer = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplayer * avoidanceRadiusMultiplayer;

        //Crée tous les poissons et donne les noms
        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                Random.insideUnitSphere * startingCount * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f,360f)),
                transform
                );
            newAgent.name = "Agent" + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }

    }

    // Update is called once per frame
    void Update() // changé depuis FixedUpdate
    {
        //Fait bouger les poissons en fonctions des voisins
        foreach(FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            Vector3 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }
            agent.Move(move);
        }
    }

    //Crée une liste de transformation en fct des voisins du poisson
    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);
        foreach(Collider c in contextColliders)
        {
            if ( c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

}