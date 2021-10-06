﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public new string name;
    public int hp, maxHp, attack;
    public Sprite sprite;
    public Item drop;
    public Skill[] skills;
}