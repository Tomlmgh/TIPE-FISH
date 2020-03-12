﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Curve
{

    protected List<Polynomial> polyLst = new List<Polynomial>();
    protected List<Vector2> keys = new List<Vector2>();
    
    //évaluter la courbe en x
    public float Get(float x)
    {
        if ( keys.Count == 0 ) {
            return 0;
        }
        
        int id = GetRangeId(x);
        if ( id == -2 ) {
            return keys[0].y;
        } else if ( id == -1 ) {
            return keys[keys.Count-1].y;
        }
        return (id < 0) ? 0 : polyLst[id].Get(x);
    }

    //ajouter une clé
    public void AddKey(float x, float y)
    {
        AddKey(new Vector2(x, y));
    }

    public void AddKey(Vector2 key)
    {
        int insertId = GetRangeId(key.x);
        if (insertId == -1)
        {
            keys.Add(key);
            insertId = keys.Count - 1;
        }
        else
        {
            if (insertId == -2)
            {
                insertId = 0;
            }
            else
            {
                insertId++;
            }
            keys.Insert(insertId, key);
        }

        if (insertId > 0)
        {
            polyLst.Insert(insertId - 1, CreatePoly(insertId-1));
        }
        if (insertId + 1 < keys.Count)
        {
            if ( insertId == 0 ) {
                polyLst.Insert( 0, CreatePoly(insertId));
            } else {
                polyLst[insertId] = CreatePoly(insertId);
            }
        }
    }

    //récupère les clé.
    public List<Vector2> GetKeys()
    {
        return keys;
    }

    
    //modifier la clé a l'id spécifié
    public void ChangeKeyById(int id, float x, float y)
    {
        ChangeKeyById(id, new Vector2(x, y));
    }

    public void ChangeKeyById(int id, Vector2 key)
    {
        if (id >= 0 && id < keys.Count && (id==0 || key.x >= keys[id-1].x) && (id==keys.Count-1 || key.x <= keys[id+1].x) )
        {
            keys[id] = key;
            if (id + 1 < keys.Count)
            {
                polyLst[id] = CreatePoly(id);
            }
            if (id > 0)
            {
                polyLst[id - 1] = CreatePoly(id - 1);
            }

        }
    }
    
    //supprimer une clé
    public bool RemoveKeyAt( int id ) {
        if ( id < 0 || id >= keys.Count  ) {
            return false;
        }
        
        //suppression de la clé
        keys.RemoveAt(id);
        
        //suppression d'un polynome a l'indice de la clé et création d'un nouveau polynome avant
        if ( polyLst.Count != 0 ) {
            polyLst.RemoveAt( (id != polyLst.Count) ? id : id-1 );
        }
        if ( id != 0 && id != keys.Count ) {
            polyLst[id-1] = CreatePoly(id-1);
        }
        
        
        
        return true;
    }
    
    
    
    //opération de base
    
    //addition
    public static Curve operator +( Curve curve, float f ) {
        Curve clone = curve.Clone();
        clone.Add(f);
        return clone;
    }
    
    public void Add(float f) {
        for( int i=0; i+1<keys.Count; i++ ) {
            keys[i] = new Vector2( keys[i].x, keys[i].y + f );
            polyLst[i].Add(f);
        }
        if ( polyLst.Count != 0 ) {
            polyLst[polyLst.Count-1].Add(f);
        }
    }
    
    //multiplication
    public static Curve operator *( Curve curve, float f ) {
        Curve clone = curve.Clone();
        clone.Mul(f);
        return clone;
    }
    
    public void Mul(float f) {
        for( int i=0; i+1<keys.Count; i++ ) {
            keys[i] = new Vector2( keys[i].x, keys[i].y * f );
            polyLst[i].Mul(f);
        }
        if ( polyLst.Count != 0 ) {
            polyLst[polyLst.Count-1].Mul(f);
        }
    }
    

    public abstract Polynomial CreatePoly(int id);

    //retourne l'id du polynome qui doit être évalué en x
    //-1 si x est suppérieur a la dernière clé
    //-2 si x est inférieur a la première clé, ou qu'il n'y a aucun polynome
    private int GetRangeId(float x)
    {
        if (keys.Count == 0 || x > keys[keys.Count - 1].x)
        {
            return -1;
        }
        else if (x < keys[0].x)
        {
            return -2;
        }

        int i = 0;
        while (keys[i + 1].x < x)
        {
            i++;
        }
        return i;
    }
    
    
    //Clone
    public abstract Curve Clone();
    
    
    
    //constructeurs
    public Curve() : this(null) { }


    public Curve(List<Vector2> keys)
    {
        this.keys = new List<Vector2>();
        if (keys != null)
        {
            foreach (Vector2 key in keys)
            {
                AddKey(key);
            }
        }
    }
    
    //un constructeur privé qui ne fait pas de vérif sur les données d'entrée
    protected Curve( List<Vector2> keys, List<Polynomial> polyLst ) {
        this.keys = keys;
        this.polyLst = polyLst;
    }

}