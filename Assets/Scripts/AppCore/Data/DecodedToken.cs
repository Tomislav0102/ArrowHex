using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DecodedToken
{
    public string token_type;
    public int exp;
    public int iat;
    public string jti;
    public int user_id;
    public string role;
    public string username;
    public int currency_gold;
    public int currency_diamond;
    public int experience;
}
